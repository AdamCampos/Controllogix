using SQLExecutor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor
{
    public class L5XReader
    {
        public void ProcessFiles(string directoryPath, string operation)
        {
            // Verificar se o diretório existe
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"O diretório '{directoryPath}' não foi encontrado.");
                return;
            }

            // Obter todos os arquivos com extensão .L5X no diretório
            string[] l5xFiles = Directory.GetFiles(directoryPath, "*.L5X");

            if (l5xFiles.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .L5X foi encontrado no diretório especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {l5xFiles.Length} arquivo(s) .L5X no diretório '{directoryPath}':\n");

            foreach (string filePath in l5xFiles)
            {
                Console.WriteLine($"Lendo arquivo: {Path.GetFileName(filePath)}\n");

                try
                {
                    ManipuladorXML manipulador = new ManipuladorXML(filePath);

                    if (operation.Equals("ConverterCSV", StringComparison.OrdinalIgnoreCase))
                    {
                        manipulador.ConverteArquivo();
                    }
                    else if (operation.Equals("CriarInserts", StringComparison.OrdinalIgnoreCase))
                    {
                        manipulador.CriarInserts();
                    }
                    else if (operation.Equals("CriarSQLEstruturaTabelas", StringComparison.OrdinalIgnoreCase))
                    {
                        manipulador.CriarSQLEstruturaTabelas();
                    }
                    else if (operation.Equals("CriarParametrosCSV", StringComparison.OrdinalIgnoreCase))
                    {
                        CriarParametrosCSV(directoryPath);
                    }
                    else if (operation.Equals("CriarTabelas", StringComparison.OrdinalIgnoreCase))
                    {
                        CriarTabelaBanco();
                    }
                    else
                    {
                        Console.WriteLine("Operação desconhecida. Por favor, use 'ConverterCSV', 'SalvarSQL' ou 'CriarParametrosCSV'.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o arquivo {Path.GetFileName(filePath)}: {ex.Message}\n");
                }

                Console.WriteLine("\n--- Fim do arquivo ---\n");
            }

            Console.WriteLine("Leitura concluída.");
        }

        private void CriarTabelaBanco()
        {
            // String de conexão com o banco de dados
            string connectionString = "Server=localhost;Database=Controllogix;User Id=P83;Password=P83;";

            // Diretório onde estão os arquivos .sql
            string sqlDirectory = @"C:\RSLogix 5000\Projects\P83\SQL\Insert";

            // Instancia a classe SqlExecutor e executa os arquivos .sql
            ExecutaSQL sqlExecutor = new ExecutaSQL(connectionString, sqlDirectory);
            sqlExecutor.ExecuteAllSqlFiles();

            Console.WriteLine("Execução de todos os arquivos .sql concluída.");
        }

        private void CriarParametrosCSV(string directoryPath)
        {
            string[] csvFiles = Directory.GetFiles(directoryPath, "*.csv");
            var headerSet = new SortedSet<string>();

            foreach (string csvFile in csvFiles)
            {
                using (var reader = new StreamReader(csvFile))
                {
                    string headerLine = reader.ReadLine();
                    if (!string.IsNullOrEmpty(headerLine))
                    {
                        var headers = headerLine.Split(',');
                        foreach (var header in headers)
                        {
                            headerSet.Add(header.Trim());
                        }
                    }
                }
            }

            string parametrosFilePath = Path.Combine(directoryPath, "parametros.csv");
            File.WriteAllLines(parametrosFilePath, new[] { string.Join(",", headerSet) });

            Console.WriteLine($"Arquivo 'parametros.csv' criado em: {parametrosFilePath}");
        }
    }
}
