using System;
using System.IO;

namespace Leitor
{
    public class LeitorCSV
    {
        private readonly string caminhoEntrada = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\L5X";
        private readonly string caminhoSaida = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\CSV\\Dados\\Tipos";

        public void Processa()
        {
            // Verificar se o diretório existe
            if (!Directory.Exists(caminhoEntrada))
            {
                Console.WriteLine($"O diretório '{caminhoEntrada}' não foi encontrado.");
                return;
            }

            // Obter todos os arquivos com extensão .CSV no diretório
            string[] csvFiles = Directory.GetFiles(caminhoEntrada, "*.csv");

            if (csvFiles.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .CSV foi encontrado no diretório especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {csvFiles.Length} arquivo(s) .CSV no diretório '{caminhoEntrada}':\n");

            foreach (string filePath in csvFiles)
            {
                Console.WriteLine($"Lendo arquivo: {Path.GetFileName(filePath)}\n");

                /*try
                {
                    switch (operacao.ToLower())
                    {
                        case "criarparametroscsv":
                            CriarParametrosCSV(filePath);
                            break;
                        default:
                            Console.WriteLine("Operação desconhecida para arquivos CSV.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o arquivo {Path.GetFileName(filePath)}: {ex.Message}\n");
                }*/

                Console.WriteLine("\n--- Fim do arquivo ---\n");
            }

            Console.WriteLine("Leitura concluída.");
        }

        private void CriarParametrosCSV(string filePath)
        {
            // Implementação de exemplo para criação de parâmetros CSV
            Console.WriteLine($"Processando arquivo CSV para gerar parâmetros: {filePath}");
            // Adicionar lógica específica para esta operação
        }
    }
}
