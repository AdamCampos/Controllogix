using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace Leitor
{
    public class LeitorExcel
    {
        private readonly string diretorioEntrada = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\XLSX";
        private readonly string diretorioSaida = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\CSV";
        private readonly HashSet<string> listaCabecalhosDinamica = new HashSet<string>(); // Lista dinâmica de cabeçalhos únicos

        public LeitorExcel()
        {
            ProcessarExcel();
        }

        public void ProcessarExcel()
        {
            if (!Directory.Exists(diretorioEntrada))
            {
                Console.WriteLine($"O diretório de entrada '{diretorioEntrada}' não foi encontrado.");
                return;
            }

            if (!Directory.Exists(diretorioSaida))
            {
                Directory.CreateDirectory(diretorioSaida);
            }

            string[] excelFiles = Directory.GetFiles(diretorioEntrada, "*.xlsx")
                                            .Concat(Directory.GetFiles(diretorioEntrada, "*.xlsm"))
                                            .ToArray();

            if (excelFiles.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo Excel foi encontrado no diretório especificado.");
                return;
            }

            Console.WriteLine($"Encontrados {excelFiles.Length} arquivo(s) Excel no diretório '{diretorioEntrada}':\n");

            foreach (string caminhoArquivoExcel in excelFiles)
            {
                Console.WriteLine($"Lendo arquivo: {Path.GetFileName(caminhoArquivoExcel)}");
                try
                {
                    ProcessarArquivoExcel(caminhoArquivoExcel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o arquivo {Path.GetFileName(caminhoArquivoExcel)}: {ex.Message}");
                }
            }

            Console.WriteLine("\nLista única de cabeçalhos capturados:");
            foreach (var cabecalho in listaCabecalhosDinamica)
            {
                Console.WriteLine($" - {cabecalho}");
            }

            // Após capturar os cabeçalhos, criar os CSVs
            foreach (string caminhoArquivoExcel in excelFiles)
            {
                try
                {
                    CriarCSVPorPlanilha(caminhoArquivoExcel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar CSV para o arquivo {Path.GetFileName(caminhoArquivoExcel)}: {ex.Message}");
                }
            }
        }

        private void ProcessarArquivoExcel(string caminhoArquivoExcel)
        {
            using (var pacote = new ExcelPackage(new FileInfo(caminhoArquivoExcel)))
            {
                foreach (var planilha in pacote.Workbook.Worksheets)
                {
                    if (!planilha.Name.ToUpper().StartsWith("SETPOINT"))
                    {
                        Console.WriteLine($"Planilha ignorada (nome não começa com 'SETPOINT'): {planilha.Name}");
                        continue;
                    }

                    int totalColunas = Math.Min(planilha.Dimension.Columns, 17); // Limitar de A a Q
                    var cabecalhos = CapturarCabecalhosCompostos(planilha, totalColunas);

                    listaCabecalhosDinamica.UnionWith(cabecalhos.Where(c => !string.IsNullOrEmpty(c)));
                }
            }
        }

        private void CriarCSVPorPlanilha(string caminhoArquivoExcel)
        {
            using (var pacote = new ExcelPackage(new FileInfo(caminhoArquivoExcel)))
            {
                foreach (var planilha in pacote.Workbook.Worksheets)
                {
                    if (!planilha.Name.ToUpper().StartsWith("SETPOINT"))
                    {
                        continue;
                    }

                    string caminhoCSV = Path.Combine(diretorioSaida, $"{planilha.Name}.csv");
                    SalvarPlanilhaComoCSV(planilha, listaCabecalhosDinamica.ToList(), caminhoCSV);
                }
            }
        }

        private void SalvarPlanilhaComoCSV(ExcelWorksheet planilha, List<string> cabecalhos, string caminhoCSV)
        {
            using (var writer = new StreamWriter(caminhoCSV))
            {
                // Escrever cabeçalho no CSV
                writer.WriteLine(string.Join(",", cabecalhos));

                // Obter dados a partir da linha 8
                for (int linha = 8; linha <= planilha.Dimension.Rows; linha++)
                {
                    var valoresLinha = new List<string>();
                    foreach (var cabecalho in cabecalhos)
                    {
                        int col = ObterIndiceColunaComposta(planilha, cabecalho);
                        valoresLinha.Add(col > 0 && col <= 17 ? planilha.Cells[linha, col].Text.Replace(",", ";") : ""); // Limitar a colunas A-Q
                    }
                    writer.WriteLine(string.Join(",", valoresLinha));
                }
            }

            Console.WriteLine($"CSV gerado: {caminhoCSV}");
        }

        private string[] CapturarCabecalhosCompostos(ExcelWorksheet planilha, int totalColunas)
        {
            var cabecalhos = new string[totalColunas];

            for (int col = 1; col <= totalColunas; col++)
            {
                string valorLinha6 = ObterValorCelula(planilha, 6, col);
                string valorLinha7 = ObterValorCelula(planilha, 7, col);

                string cabecalhoComposto = $"{valorLinha6} - {valorLinha7}".Trim();

                // Ignorar cabeçalhos começando com "-"
                if (cabecalhoComposto.StartsWith("-"))
                {
                    cabecalhos[col - 1] = null;
                }
                else
                {
                    cabecalhos[col - 1] = cabecalhoComposto;
                }
            }

            return cabecalhos;
        }

        private int ObterIndiceColunaComposta(ExcelWorksheet planilha, string cabecalho)
        {
            for (int col = 1; col <= Math.Min(planilha.Dimension.Columns, 17); col++) // Limitar a colunas A-Q
            {
                string valorLinha6 = ObterValorCelula(planilha, 6, col);
                string valorLinha7 = ObterValorCelula(planilha, 7, col);

                string cabecalhoComposto = $"{valorLinha6} - {valorLinha7}".Trim();
                if (cabecalhoComposto.Equals(cabecalho, StringComparison.OrdinalIgnoreCase))
                {
                    return col;
                }
            }
            return -1; // Retorna -1 se o cabeçalho não for encontrado
        }

        private string ObterValorCelula(ExcelWorksheet planilha, int linha, int coluna)
        {
            var celula = planilha.Cells[linha, coluna];
            if (celula.Merge)
            {
                // Capturar a faixa mesclada
                string enderecoFaixa = planilha.MergedCells[linha, coluna];
                if (!string.IsNullOrEmpty(enderecoFaixa))
                {
                    // Retornar o valor da primeira célula da faixa mesclada
                    var primeiraCelula = planilha.Cells[new ExcelAddress(enderecoFaixa).Start.Row, new ExcelAddress(enderecoFaixa).Start.Column];
                    return primeiraCelula?.Text?.Trim();
                }
            }
            return celula?.Text?.Trim();
        }
    }
}
