using System;
using System.IO;
using System.Threading.Tasks;
using RockwellAutomation.LogixDesigner;

namespace Leitor
{
    public class LeitorACD
    {
        private readonly string caminhoEntrada = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\ACD";

        /// <summary>
        /// Lista todos os arquivos .acd no diretório de entrada especificado.
        /// </summary>
        public void ListarArquivosACD()
        {
            // Verifica se o diretório de entrada existe
            if (!Directory.Exists(caminhoEntrada))
            {
                Console.WriteLine($"O diretório '{caminhoEntrada}' não foi encontrado.");
                return;
            }

            // Obtém todos os arquivos .acd no diretório
            string[] acdFiles = Directory.GetFiles(caminhoEntrada, "*.acd");

            if (acdFiles.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .acd foi encontrado no diretório especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {acdFiles.Length} arquivo(s) .acd no diretório '{caminhoEntrada}':\n");

            // Exibe cada arquivo encontrado e processa o conteúdo
            foreach (string filePath in acdFiles)
            {
                string fileName = Path.GetFileName(filePath);
                Console.WriteLine($" - {fileName}");

                // Processa o arquivo se for o alvo para teste
                if (fileName.Equals("P80_TOPSIDE_PCS03.ACD", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessarArquivoACD(filePath).Wait();
                }
            }
        }

        /// <summary>
        /// Processa o conteúdo de um arquivo ACD utilizando a API da Rockwell.
        /// </summary>
        /// <param name="caminhoArquivo">Caminho completo do arquivo ACD.</param>
        private async Task ProcessarArquivoACD(string caminhoArquivo)
        {
            try
            {
                Console.WriteLine($"Iniciando processamento do arquivo: {Path.GetFileName(caminhoArquivo)}");

                // Carrega o projeto Logix usando a API Rockwell
                using (var myProject = await LogixProject.OpenLogixProjectAsync(caminhoArquivo))
                {
                    Console.WriteLine("Arquivo ACD carregado com sucesso.");

                    // Exemplo de manipulação: listando tags do projeto
                    foreach (var tag in myProject.Tags)
                    {
                        Console.WriteLine($"Tag: {tag.Name}, Tipo: {tag.DataType}");
                    }

                    // Aqui podemos adicionar mais lógicas específicas de conversão
                    Console.WriteLine("Conversão aplicada com sucesso.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar o arquivo ACD: {ex.Message}");
            }
        }
    }
}
