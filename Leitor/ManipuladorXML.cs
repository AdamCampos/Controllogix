using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Leitor
{
    public class ManipuladorXML
    {
        private readonly string caminhoSaida;
        private readonly string caminhoEntrada;

        public ManipuladorXML(string caminhoEntrada, string caminhoSaida)
        {
            this.caminhoEntrada = caminhoEntrada;
            this.caminhoSaida = caminhoSaida;
        }

        private void CriarCsvPorTag(XmlDocument xmlDoc)
        {
            // Diretório de saída
            string outputDirectory = Path.Combine(Path.GetDirectoryName(caminhoSaida), "CSV");
            Directory.CreateDirectory(outputDirectory);

            // Prefixo do nome do arquivo
            string fileNamePrefix = Path.GetFileNameWithoutExtension(caminhoSaida);

            // Obtem todas as tags do arquivo XML
            XmlNodeList tagNodes = xmlDoc.GetElementsByTagName("Tag");

            // Dicionário para armazenar todos os cabeçalhos (parâmetros) para cada DataType
            var parametrosPorTipo = new Dictionary<string, HashSet<string>>();
            var valoresPorTipo = new Dictionary<string, List<Dictionary<string, string>>>();

            foreach (XmlNode tagNode in tagNodes)
            {
                // Extrai atributos relevantes
                string tagName = tagNode.Attributes?["Name"]?.Value ?? "";
                string tagDataType = tagNode.Attributes?["DataType"]?.Value ?? "";

                if (string.IsNullOrEmpty(tagName) || string.IsNullOrEmpty(tagDataType)) continue;

                // Garante que o DataType exista nos dicionários
                if (!parametrosPorTipo.ContainsKey(tagDataType))
                {
                    parametrosPorTipo[tagDataType] = new HashSet<string>();
                    valoresPorTipo[tagDataType] = new List<Dictionary<string, string>>();
                }

                // Dicionário para armazenar os valores dessa tag
                var valoresDaTag = new Dictionary<string, string>
        {
            { "TagName", tagName } // Sempre incluir a coluna "TagName"
        };

                // Procura pelos <DataValueMember> na estrutura da tag
                XmlNode structureNode = tagNode.SelectSingleNode("Data/Structure");
                if (structureNode != null)
                {
                    XmlNodeList dataValueMembers = structureNode.SelectNodes("DataValueMember");

                    foreach (XmlNode dataValueMember in dataValueMembers)
                    {
                        string parametro = dataValueMember.Attributes?["Name"]?.Value ?? "";
                        string valor = dataValueMember.Attributes?["Value"]?.Value ?? "";

                        if (!string.IsNullOrEmpty(parametro))
                        {
                            // Adiciona o parâmetro ao conjunto de cabeçalhos
                            parametrosPorTipo[tagDataType].Add(parametro);

                            // Armazena o valor no dicionário de valores da tag
                            valoresDaTag[parametro] = valor;
                        }
                    }
                }

                // Adiciona os valores da tag à lista do tipo correspondente
                valoresPorTipo[tagDataType].Add(valoresDaTag);
            }

            // Gera um CSV para cada DataType
            foreach (var tipo in parametrosPorTipo)
            {
                string dataType = tipo.Key;
                var parametros = tipo.Value;

                string csvFilePath = Path.Combine(outputDirectory, $"{fileNamePrefix}_{dataType}.csv");

                // Cria as linhas do CSV
                var csvLines = new List<string>();

                // Cabeçalho: sempre incluir "TagName" seguido dos parâmetros coletados
                var header = new List<string> { "TagName" };
                header.AddRange(parametros.OrderBy(p => p)); // Ordena os parâmetros alfabeticamente
                csvLines.Add(string.Join(",", header));

                // Adiciona os valores para cada tag
                foreach (var valores in valoresPorTipo[dataType])
                {
                    var linha = new List<string>();

                    foreach (var coluna in header)
                    {
                        // Adiciona o valor correspondente ou uma string vazia se não existir
                        linha.Add(valores.ContainsKey(coluna) ? valores[coluna] : "");
                    }

                    csvLines.Add(string.Join(",", linha));
                }

                // Escreve o CSV no arquivo
                File.WriteAllLines(csvFilePath, csvLines);

                Console.WriteLine($"CSV gerado para DataType '{dataType}': {csvFilePath}");
            }
        }


        private void CriarSQLEstruturaTabelas()
        {
            string tiposDirectory = Path.Combine(Path.GetDirectoryName(caminhoSaida), "CSV");
            string sqlDirectory = Path.Combine(Path.GetDirectoryName(caminhoSaida), "SQL", "Estrutura");
            Directory.CreateDirectory(sqlDirectory);

            string[] csvFiles = Directory.GetFiles(tiposDirectory, "*.csv");

            foreach (string csvFile in csvFiles)
            {
                string tableName = Path.GetFileNameWithoutExtension(csvFile);
                string sqlFilePath = Path.Combine(sqlDirectory, tableName + ".sql");

                var sqlLines = new List<string>();
                sqlLines.Add($"CREATE TABLE [dbo].[{tableName}] (");

                using (var reader = new StreamReader(csvFile))
                {
                    string headerLine = reader.ReadLine();
                    if (!string.IsNullOrEmpty(headerLine))
                    {
                        var columns = headerLine.Split(',');

                        foreach (var column in columns)
                        {
                            string columnName = column.Trim();
                            string columnType = "VARCHAR(255)";

                            if (columnName.StartsWith("I_b") || columnName.StartsWith("O_b"))
                            {
                                columnType = "BIT";
                            }
                            else if (columnName.StartsWith("I_i") || columnName.StartsWith("O_i"))
                            {
                                columnType = "BIGINT";
                            }
                            else if (columnName.StartsWith("I_r") || columnName.StartsWith("O_r"))
                            {
                                columnType = "FLOAT";
                            }

                            if (columnName.Equals("TagName", StringComparison.OrdinalIgnoreCase))
                            {
                                sqlLines.Add($"    [{columnName}] {columnType} PRIMARY KEY,");
                            }
                            else
                            {
                                sqlLines.Add($"    [{columnName}] {columnType},");
                            }
                        }
                    }
                }

                if (sqlLines.Count > 1)
                {
                    sqlLines[sqlLines.Count - 1] = sqlLines[sqlLines.Count - 1].TrimEnd(','); // Remove a última vírgula
                }

                sqlLines.Add(");");

                File.WriteAllLines(sqlFilePath, sqlLines);

                Console.WriteLine($"Arquivo SQL criado: {sqlFilePath}");
            }
        }



        public void CriarInserts()
        {
            // Diretórios de entrada (CSV) e saída (SQL Inserts)
            string csvDirectory = Path.Combine(Path.GetDirectoryName(caminhoSaida), "CSV");
            string insertDirectory = Path.Combine(Path.GetDirectoryName(caminhoSaida), "SQL", "Insert");
            Directory.CreateDirectory(insertDirectory);

            // Lista de arquivos CSV no diretório
            string[] csvFiles = Directory.GetFiles(csvDirectory, "*.csv");

            foreach (string csvFile in csvFiles)
            {
                // Nome da tabela baseado no nome do arquivo CSV
                string tableName = Path.GetFileNameWithoutExtension(csvFile);
                string sqlFilePath = Path.Combine(insertDirectory, tableName + ".sql");

                var sqlLines = new List<string>();

                using (var reader = new StreamReader(csvFile))
                {
                    // Lê a primeira linha para obter os nomes das colunas
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrEmpty(headerLine))
                    {
                        Console.WriteLine($"Arquivo CSV vazio: {csvFile}");
                        continue;
                    }

                    var columns = headerLine.Split(',').Select(column => column.Trim()).ToList();

                    // Processa cada linha do arquivo CSV
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;

                        var values = line.Split(',').Select(value => value.Trim()).ToList();

                        // Monta o comando SQL de INSERT
                        var insert = new StringBuilder();
                        insert.Append($"INSERT INTO [{tableName}] (");
                        insert.Append(string.Join(", ", columns.Select(column => $"[{column}]")));
                        insert.Append(") VALUES (");

                        for (int i = 0; i < values.Count; i++)
                        {
                            if (i > 0) insert.Append(", ");

                            string value = values[i];

                            // Trata valores nulos ou strings
                            if (string.IsNullOrEmpty(value) || value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                            {
                                insert.Append("NULL");
                            }
                            else if (decimal.TryParse(value, out _)) // Verifica se é numérico
                            {
                                insert.Append(value);
                            }
                            else // Trata como string, escapando aspas simples
                            {
                                insert.Append($"'{value.Replace("'", "''")}'");
                            }
                        }

                        insert.Append(");");

                        // Adiciona o comando SQL à lista
                        sqlLines.Add(insert.ToString());
                    }
                }

                // Salva os comandos SQL no arquivo correspondente
                File.WriteAllLines(sqlFilePath, sqlLines);
                Console.WriteLine($"Arquivo SQL criado: {sqlFilePath}");
            }
        }


        private void IdentificarTags(XmlDocument xmlDoc)
        {
            XmlNodeList tagNodes = xmlDoc.GetElementsByTagName("Tag");

            foreach (XmlNode tagNode in tagNodes)
            {
                Console.WriteLine("Parâmetros da tag <Tag>:");

                foreach (XmlAttribute attribute in tagNode.Attributes)
                {
                    Console.WriteLine($" - {attribute.Name}: {attribute.Value}");
                }

                XmlNode structureNode = tagNode.SelectSingleNode("Data/Structure");
                if (structureNode != null)
                {
                    Console.WriteLine("  <Structure>:");

                    XmlNodeList dataValueMembers = structureNode.SelectNodes("DataValueMember");
                    foreach (XmlNode dataValueMember in dataValueMembers)
                    {
                        Console.WriteLine("    <DataValueMember>:");
                        foreach (XmlAttribute attribute in dataValueMember.Attributes)
                        {
                            Console.WriteLine($"      - {attribute.Name}: {attribute.Value}");
                        }
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
