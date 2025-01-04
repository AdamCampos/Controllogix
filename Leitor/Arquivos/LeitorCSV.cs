using System;
using System.IO;
using System.Linq;

namespace Leitor
{
    public class LeitorCSV
    {
        private readonly string caminhoEntrada = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\CSV\\Dados\\Tipos";
        private readonly string caminhoSaidaResumo = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\CSV\\Estrutura\\Resultado.csv";
        private readonly string caminhoSaidaDadosSQL = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\SQL\\Dados";
        private readonly string caminhoSaidaEstruturaSQL = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\SQL\\Estrutura";

        public void ProcessarEstrutura()
        {
            // Verificar se o diretório de entrada existe
            if (!Directory.Exists(caminhoEntrada))
            {
                Console.WriteLine($"O diretório de entrada '{caminhoEntrada}' não foi encontrado.");
                return;
            }

            // Criar o diretório de saída se ele não existir
            string pastaSaida = Path.GetDirectoryName(caminhoSaidaResumo);
            if (!Directory.Exists(pastaSaida))
            {
                Directory.CreateDirectory(pastaSaida);
                Console.WriteLine($"Diretório de saída '{pastaSaida}' criado.");
            }

            // Obter todos os arquivos CSV no diretório de entrada
            string[] csvFiles = Directory.GetFiles(caminhoEntrada, "*.csv");

            if (csvFiles.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .CSV foi encontrado no diretório de entrada especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {csvFiles.Length} arquivo(s) .CSV no diretório '{caminhoEntrada}'.\n");

            var outputLines = new System.Collections.Generic.List<string>();

            // Adiciona cabeçalho ao arquivo de saída
            outputLines.Add("FileName,TagName");

            foreach (string filePath in csvFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                Console.WriteLine($"Processando arquivo: {fileName}");

                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        // Lê o cabeçalho original para identificar a coluna "TagName"
                        var headerLine = reader.ReadLine();
                        if (string.IsNullOrEmpty(headerLine))
                        {
                            Console.WriteLine($"O arquivo {fileName} está vazio.");
                            continue;
                        }

                        var headers = headerLine.Split(',');
                        int tagNameIndex = Array.IndexOf(headers, "TagName");

                        if (tagNameIndex == -1)
                        {
                            Console.WriteLine($"A coluna 'TagName' não foi encontrada no arquivo {fileName}.");
                            continue;
                        }

                        // Lê cada linha do arquivo e adiciona ao novo formato
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line)) continue;

                            var values = line.Split(',');
                            if (tagNameIndex < values.Length)
                            {
                                string tagNameValue = values[tagNameIndex];
                                outputLines.Add($"{fileName},{tagNameValue}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o arquivo {fileName}: {ex.Message}\n");
                }
            }

            try
            {
                // Escreve o arquivo de saída único
                File.WriteAllLines(caminhoSaidaResumo, outputLines);
                Console.WriteLine($"Arquivo consolidado salvo em: {caminhoSaidaResumo}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar o arquivo consolidado: {ex.Message}\n");
            }

            Console.WriteLine("Processamento concluído.");
        }

        public void GerarScriptCriacaoTabelas()
        {
            // Criar diretório de saída para os scripts de estrutura SQL, se não existir
            if (!Directory.Exists(caminhoSaidaEstruturaSQL))
            {
                Directory.CreateDirectory(caminhoSaidaEstruturaSQL);
                Console.WriteLine($"Diretório de saída para estrutura SQL '{caminhoSaidaEstruturaSQL}' criado.");
            }

            string[] csvFiles = Directory.GetFiles(caminhoEntrada, "*.csv");

            foreach (string filePath in csvFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string sqlFilePath = Path.Combine(caminhoSaidaEstruturaSQL, fileName + ".sql");

                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        var headerLine = reader.ReadLine();
                        if (string.IsNullOrEmpty(headerLine))
                        {
                            Console.WriteLine($"O arquivo {fileName} está vazio.");
                            continue;
                        }

                        var headers = headerLine.Split(',');
                        var sqlLines = new System.Collections.Generic.List<string>
                        {
                            $"CREATE TABLE {fileName} ("
                        };

                        foreach (var header in headers)
                        {
                            sqlLines.Add($"    [{header}] NVARCHAR(MAX),");
                        }

                        // Remove a última vírgula e fecha o comando
                        sqlLines[sqlLines.Count - 1] = sqlLines[sqlLines.Count - 1].TrimEnd(',');
                        sqlLines.Add(");");

                        File.WriteAllLines(sqlFilePath, sqlLines);
                        Console.WriteLine($"Arquivo SQL para criação de tabela '{fileName}' criado em {sqlFilePath}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao gerar script de criação para o arquivo {fileName}: {ex.Message}");
                }
            }
        }

        public void GerarScriptInsercaoDados()
        {
            // Criar diretório de saída para os scripts de dados SQL, se não existir
            if (!Directory.Exists(caminhoSaidaDadosSQL))
            {
                Directory.CreateDirectory(caminhoSaidaDadosSQL);
                Console.WriteLine($"Diretório de saída para dados SQL '{caminhoSaidaDadosSQL}' criado.");
            }

            string[] csvFiles = Directory.GetFiles(caminhoEntrada, "*.csv");

            foreach (string filePath in csvFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string sqlFilePath = Path.Combine(caminhoSaidaDadosSQL, fileName + ".sql");

                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        var headerLine = reader.ReadLine();
                        if (string.IsNullOrEmpty(headerLine))
                        {
                            Console.WriteLine($"O arquivo {fileName} está vazio.");
                            continue;
                        }

                        var headers = headerLine.Split(',');
                        var sqlLines = new System.Collections.Generic.List<string>();

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line)) continue;

                            var values = line.Split(',').Select(v => $"'{v.Replace("'", "''")}'").ToArray();
                            sqlLines.Add($"INSERT INTO {fileName} ({string.Join(", ", headers)}) VALUES ({string.Join(", ", values)});");
                        }

                        File.WriteAllLines(sqlFilePath, sqlLines);
                        Console.WriteLine($"Arquivo SQL para inserção de dados '{fileName}' criado em {sqlFilePath}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao gerar script de inserção para o arquivo {fileName}: {ex.Message}");
                }
            }
        }
    }
}
