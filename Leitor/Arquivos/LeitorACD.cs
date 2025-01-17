using RockwellAutomation.LogixDesigner;
using RockwellAutomation.LogixDesigner.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Leitor
{
    /// <summary>
    /// Classe responsável por listar e processar arquivos ACD utilizando o SDK da Rockwell.
    /// </summary>
    public class LeitorACD
    {
        /// <summary>
        /// Caminho do diretório onde os arquivos .acd estão localizados.
        /// </summary>
        private readonly string caminhoEntrada = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\ACD";

        /// <summary>
        /// Caminho do diretório onde os arquivos .l5x serão salvos.
        /// </summary>
        private readonly string caminhoSaida = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\L5X";

        /// <summary>
        /// Lista todos os arquivos .acd no diretório especificado e processa cada um deles.
        /// </summary>
        public void ListarArquivosACD()
        {
            // Verifica se o diretório de entrada existe
            if (!Directory.Exists(caminhoEntrada))
            {
                Console.WriteLine($"O diretório '{caminhoEntrada}' não foi encontrado.");
                return;
            }

            // Verifica se o diretório de saída existe; caso contrário, cria o diretório
            if (!Directory.Exists(caminhoSaida))
            {
                Directory.CreateDirectory(caminhoSaida);
            }

            // Obtém todos os arquivos com extensão .acd no diretório
            string[] arquivosACD = Directory.GetFiles(caminhoEntrada, "*.acd");

            if (arquivosACD.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .acd foi encontrado no diretório especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {arquivosACD.Length} arquivo(s) .acd no diretório '{caminhoEntrada}':\n");

            // Processa cada arquivo encontrado
            foreach (string arquivoACD in arquivosACD)
            {
                string nomeArquivo = Path.GetFileName(arquivoACD);
                Console.WriteLine($" - {nomeArquivo}");
                ProcessarArquivoACD(arquivoACD).Wait();
            }
        }

        /// <summary>
        /// Processa o conteúdo de um arquivo ACD, salvando-o como um arquivo L5X no diretório de saída.
        /// </summary>
        /// <param name="caminhoArquivo">Caminho completo do arquivo ACD a ser processado.</param>
        private async Task ProcessarArquivoACD(string caminhoArquivo)
        {
            try
            {
                // Gera o caminho completo para o arquivo L5X no diretório de saída
                string nomeArquivoL5X = Path.GetFileNameWithoutExtension(caminhoArquivo) + ".l5x";
                string caminhoL5X = Path.Combine(caminhoSaida, nomeArquivoL5X);

                // Abre o projeto utilizando o SDK da Rockwell
                using LogixProject project = await LogixProject.OpenLogixProjectAsync(caminhoArquivo, new StdOutEventLogger());

                // Salva o projeto como um arquivo L5X no caminho especificado
                await project.SaveAsAsync(caminhoL5X, true, false);

                Console.WriteLine($"Arquivo convertido e salvo como: {caminhoL5X}");
            }
            catch (Exception ex)
            {
                // Trata erros ao processar o arquivo
                Console.WriteLine($"..Erro ao processar o arquivo ACD: {ex.Message}");
            }
        }
    }
}
