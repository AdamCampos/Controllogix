using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RockwellAutomation.LogixDesigner.LogixProjectServices;

namespace Leitor
{
    public class LeitorACD
    {
        private readonly string caminhoEntrada = "C:\\RSLogix 5000\\Projects\\P83";
        private readonly string caminhoSaida = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\ACD_L5X";

        public void ProcessarEstrutura()
        {
            // Verificar se o diretório de entrada existe
            if (!Directory.Exists(caminhoEntrada))
            {
                Console.WriteLine($"O diretório de entrada '{caminhoEntrada}' não foi encontrado.");
                return;
            }

            // Obter todos os arquivos CSV no diretório de entrada
            string[] acdFiles = Directory.GetFiles(caminhoEntrada, "*.acd");

            if (acdFiles.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .acd foi encontrado no diretório de entrada especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {acdFiles.Length} arquivo(s) .acd no diretório '{caminhoEntrada}'.\n");

            var outputLines = new System.Collections.Generic.List<string>();

            // Adiciona cabeçalho ao arquivo de saída
            outputLines.Add("FileName,TagName");

            foreach (string filePath in acdFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                Console.WriteLine($"Processando arquivo: {fileName}");

            }
            Console.WriteLine("Processamento concluído.");
        }
        public void ProcessaACD()
        {
            string projectFilePath = caminhoEntrada;
            string saveProjectPath = caminhoSaida;
            Console.WriteLine("Testando leitura ACD");

            if (!File.Exists(projectFilePath))
            {
                Console.WriteLine($"Error: The file '{projectFilePath}' does not exist.");
                return;
            }
  
        }

    }
}
