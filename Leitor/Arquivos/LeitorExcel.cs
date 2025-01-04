using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml; // Certifique-se de ter o pacote EPPlus instalado

namespace Leitor
{
    public class LeitorExcel
    {
        private readonly string diretorioEntrada = "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\XLSX";
        private readonly HashSet<string> listaCabecalhos = new HashSet<string>(); // Lista única de cabeçalhos

        public LeitorExcel()
        {
            ProcessarExcel();
        }

        public void ProcessarExcel()
        {
            // Verificar se o diretório de entrada existe
            if (!Directory.Exists(diretorioEntrada))
            {
                Console.WriteLine($"O diretório de entrada '{diretorioEntrada}' não foi encontrado.");
                return;
            }

            // Obter todos os arquivos XLSX e XLSM no diretório de entrada
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
                    LerCabecalhos(caminhoArquivoExcel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o arquivo {Path.GetFileName(caminhoArquivoExcel)}: {ex.Message}");
                }
            }

            // Exibir a lista única de cabeçalhos no final
            Console.WriteLine("\nLista única de cabeçalhos capturados:");
            foreach (var cabecalho in listaCabecalhos)
            {
                Console.WriteLine($" - {cabecalho}");
            }
        }

        private void LerCabecalhos(string caminhoArquivoExcel)
        {
            using (var pacote = new ExcelPackage(new FileInfo(caminhoArquivoExcel)))
            {
                foreach (var planilha in pacote.Workbook.Worksheets)
                {
                    // Ignorar planilhas que não começam com "SETPOINT"
                    if (!planilha.Name.ToUpper().StartsWith("SETPOINT"))
                    {
                        Console.WriteLine($"Planilha ignorada (nome não começa com 'SETPOINT'): {planilha.Name}");
                        continue;
                    }

                    // Verificar se todas as colunas A a Q possuem conteúdo em pelo menos uma das linhas 6, 7 ou 8
                    var resultadoVerificacao = VerificarLinhas(planilha, new int[] { 6, 7, 8 }, 1, 17);

                    if (!resultadoVerificacao.IsValida)
                    {
                        Console.WriteLine($"Planilha ignorada: {planilha.Name}");
                        Console.WriteLine("Motivo: As seguintes colunas não possuem preenchimento em nenhuma das linhas 6, 7 ou 8:");
                        foreach (var coluna in resultadoVerificacao.ColunasInvalidas)
                        {
                            Console.WriteLine($" - Coluna {coluna}");
                        }
                        continue;
                    }

                    Console.WriteLine($"Processando planilha: {planilha.Name}");

                    // Determinar o número máximo de colunas usadas
                    int totalColunas = Math.Min(planilha.Dimension.Columns, 17); // Limitar de A a Q
                    var cabecalhos = CapturarCabecalhos(planilha, totalColunas);

                    Console.WriteLine($"Cabeçalhos capturados na planilha {planilha.Name}:");
                    for (int i = 0; i < cabecalhos.Length; i++)
                    {
                        string nomeColuna = ObterNomeColuna(i + 1); // Obter o nome da coluna (A, B, etc.)
                        Console.WriteLine($" - Coluna {nomeColuna}: {cabecalhos[i]}");

                        // Adicionar à lista única de cabeçalhos
                        if (!string.IsNullOrEmpty(cabecalhos[i]))
                        {
                            listaCabecalhos.Add(cabecalhos[i]);
                        }
                    }
                }
            }
        }

        private (bool IsValida, List<string> ColunasInvalidas) VerificarLinhas(ExcelWorksheet planilha, int[] linhas, int colunaInicial, int colunaFinal)
        {
            var colunasSemPreenchimento = new List<string>();

            for (int col = colunaInicial; col <= colunaFinal; col++)
            {
                bool temConteudo = false;

                foreach (int row in linhas)
                {
                    if (!string.IsNullOrWhiteSpace(planilha.Cells[row, col].Text))
                    {
                        temConteudo = true;
                        break;
                    }
                }

                if (!temConteudo)
                {
                    string nomeColuna = ObterNomeColuna(col); // Converter índice em nome de coluna
                    colunasSemPreenchimento.Add(nomeColuna);
                }
            }

            return (colunasSemPreenchimento.Count == 0, colunasSemPreenchimento);
        }

        private string[] CapturarCabecalhos(ExcelWorksheet planilha, int totalColunas)
        {
            int linhaInicial = 6; // Linha onde começa o cabeçalho
            int linhaFinal = 7;   // Linha final do cabeçalho
            var cabecalhos = new string[totalColunas];

            for (int col = 1; col <= totalColunas; col++)
            {
                string valorLinha6 = ObterValorCelula(planilha, linhaInicial, col);
                string valorLinha7 = ObterValorCelula(planilha, linhaFinal, col);

                // Concatena os valores se ambos existirem
                if (!string.IsNullOrEmpty(valorLinha6) && !string.IsNullOrEmpty(valorLinha7))
                {
                    cabecalhos[col - 1] = $"{valorLinha6} - {valorLinha7}";
                }
                else
                {
                    cabecalhos[col - 1] = valorLinha6 ?? valorLinha7;
                }
            }

            return cabecalhos;
        }

        private string ObterValorCelula(ExcelWorksheet planilha, int linha, int coluna)
        {
            var celula = planilha.Cells[linha, coluna];
            if (celula.Merge)
            {
                // Captura a faixa mesclada
                string enderecoFaixa = planilha.MergedCells[linha, coluna];
                if (!string.IsNullOrEmpty(enderecoFaixa))
                {
                    // Retorna o valor da célula de início da faixa mesclada
                    var primeiraCelula = planilha.Cells[new ExcelAddress(enderecoFaixa).Start.Row, new ExcelAddress(enderecoFaixa).Start.Column];
                    return primeiraCelula?.Text?.Trim();
                }
            }

            return celula?.Text?.Trim();
        }

        private string ObterNomeColuna(int numeroColuna)
        {
            // Convertendo número da coluna para letra (A, B, ..., Z, AA, AB, etc.)
            string coluna = string.Empty;
            while (numeroColuna > 0)
            {
                numeroColuna--;
                coluna = (char)('A' + (numeroColuna % 26)) + coluna;
                numeroColuna /= 26;
            }
            return coluna;
        }
    }
}
