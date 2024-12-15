using System;
using System.IO;

namespace Leitor
{
    public class LeitorSQL
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

            // Obter todos os arquivos com extensão .SQL no diretório
            string[] sqlFiles = Directory.GetFiles(caminhoEntrada, "*.sql");

            if (sqlFiles.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .SQL foi encontrado no diretório especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {sqlFiles.Length} arquivo(s) .SQL no diretório '{caminhoEntrada}':\n");

            foreach (string filePath in sqlFiles)
            {
                Console.WriteLine($"Lendo arquivo: {Path.GetFileName(filePath)}\n");

               /* try
                {
                    switch (operacao.ToLower())
                    {
                        case "criartabelas":
                            CriarTabelaBanco(filePath);
                            break;
                        default:
                            Console.WriteLine("Operação desconhecida para arquivos SQL.");
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

        private void CriarTabelaBanco(string filePath)
        {
            // Implementação de exemplo para criação de tabelas no banco
            Console.WriteLine($"Criando tabela no banco com base no arquivo: {filePath}");
            // Adicionar lógica específica para esta operação
        }
    }
}
