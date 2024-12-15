using System;
using System.IO;
using System.Linq;

namespace Leitor
{
    public class LeitorCSV
    {
        private readonly string caminhoEntrada = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\CSV\\Dados\\Tipos";
        private readonly string caminhoSaida = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\CSV\\Estrutura\\Resultado.csv";

        public void ProcessarEstrutura()
        {
            // Verificar se o diretório de entrada existe
            if (!Directory.Exists(caminhoEntrada))
            {
                Console.WriteLine($"O diretório de entrada '{caminhoEntrada}' não foi encontrado.");
                return;
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
                File.WriteAllLines(caminhoSaida, outputLines);
                Console.WriteLine($"Arquivo consolidado salvo em: {caminhoSaida}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar o arquivo consolidado: {ex.Message}\n");
            }

            Console.WriteLine("Processamento concluído.");
        }
    }
}
