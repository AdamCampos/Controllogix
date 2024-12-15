using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Leitor
{
    public class LeitorL5X
    {
        private readonly string caminhoEntrada = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\L5X";
        private readonly string caminhoSaida = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\CSV\\Dados\\Tipos";

        public LeitorL5X(string caminhoEntrada, string caminhoSaida)
        {
            this.caminhoEntrada = caminhoEntrada;
            this.caminhoSaida = caminhoSaida;
        }

        public LeitorL5X()
        {
        }

        public void Processa()
        {
            // Verificar se o diretório de entrada existe
            if (!Directory.Exists(caminhoEntrada))
            {
                Console.WriteLine($"O diretório de entrada '{caminhoEntrada}' não foi encontrado.");
                return;
            }

            // Verificar se o diretório de saída existe; se não, criar
            if (!Directory.Exists(caminhoSaida))
            {
                Console.WriteLine($"O diretório de saída '{caminhoSaida}' não foi encontrado. Criando...");
                Directory.CreateDirectory(caminhoSaida);
            }

            // Obter todos os arquivos com extensão .L5X no diretório de entrada
            string[] l5xFiles = Directory.GetFiles(caminhoEntrada, "*.L5X");

            if (l5xFiles.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .L5X foi encontrado no diretório de entrada especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {l5xFiles.Length} arquivo(s) .L5X no diretório '{caminhoEntrada}':\n");

            foreach (string caminhoArquivoL5X in l5xFiles)
            {
                Console.WriteLine($"Lendo arquivo: {Path.GetFileName(caminhoArquivoL5X)}\n");

                try
                {
                    CriarCsvPorTag(caminhoArquivoL5X);

                    //Cria o resumo
                    LeitorCSV leitorCSV = new LeitorCSV();
                    leitorCSV.ProcessarEstrutura();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o arquivo {Path.GetFileName(caminhoArquivoL5X)}: {ex.Message}\n");
                }

                Console.WriteLine("\n--- Fim do arquivo ---\n");
            }

            Console.WriteLine("Processamento concluído.");
        }

        private void CriarCsvPorTag(string caminhoArquivoL5X)
        {
            Console.WriteLine($"Processando arquivo L5X: {caminhoArquivoL5X}");

            // Carregar o arquivo L5X como um XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(caminhoArquivoL5X);

            // Diretório de saída
            string outputDirectory = caminhoSaida;
            Directory.CreateDirectory(outputDirectory);

            // Prefixo do nome do arquivo
            string fileNamePrefix = Path.GetFileNameWithoutExtension(caminhoArquivoL5X);

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
    }
}
