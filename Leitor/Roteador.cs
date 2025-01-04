using System;

namespace Leitor
{
    public class Roteador
    {
        public void Processar(string operacao)
        {
            try
            {
                if (operacao.Equals("SalvarL5X", StringComparison.OrdinalIgnoreCase))
                {
                    var acdReader = new LeitorACD();
                    acdReader.ProcessaACD();
                }
                else if (operacao.Equals("ObterBlocosCSV", StringComparison.OrdinalIgnoreCase))
                {
                    var l5xReader = new LeitorL5X();
                    l5xReader.ProcessaL5X();

                    var csvReader = new LeitorCSV();
                    csvReader.ProcessarEstrutura();
                }
                else if (operacao.Equals("CriarParametrosCSV", StringComparison.OrdinalIgnoreCase))
                {
                    var csvReader = new LeitorCSV();
                    csvReader.ProcessarEstrutura();
                }
                else if (operacao.Equals("CriarTabelasSQL", StringComparison.OrdinalIgnoreCase))
                {
                    var csvReader = new LeitorCSV();
                    csvReader.GerarScriptCriacaoTabelas();
                }
                else if (operacao.Equals("CriarDadosSQL", StringComparison.OrdinalIgnoreCase))
                {
                    var csvReader = new LeitorCSV();
                    csvReader.GerarScriptInsercaoDados();
                }
                else if (operacao.Equals("CriarSQL", StringComparison.OrdinalIgnoreCase))
                {
                    var csvReader = new LeitorCSV();
                    csvReader.GerarScriptCriacaoTabelas();
                    csvReader.GerarScriptInsercaoDados();
                }
                else if (operacao.Equals("ConsolidarEstrutura", StringComparison.OrdinalIgnoreCase))
                {
                    var csvReader = new LeitorCSV();
                    csvReader.ProcessarEstrutura();
                }
                else if (operacao.Equals("ExecutarTabelasSQL", StringComparison.OrdinalIgnoreCase))
                {
                    var sqlReader = new LeitorSQL("Server=10.22.39.23;Database=Controllogix;User Id=P83;Password=P83;", "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\SQL");
                    sqlReader.ExecutarEstrutura();
                }
                else if (operacao.Equals("ExecutarDadosSQL", StringComparison.OrdinalIgnoreCase))
                {
                    var sqlReader = new LeitorSQL("Server=10.22.39.23;Database=Controllogix;User Id=P83;Password=P83;", "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\SQL");
                    sqlReader.ExecutarDados();
                }
                else if (operacao.Equals("ExecutarSQL", StringComparison.OrdinalIgnoreCase))
                {
                    var sqlReader = new LeitorSQL("Server=10.22.39.23;Database=Controllogix;User Id=P83;Password=P83;", "C:\\Projetos\\VisualStudio\\LeitorControllogix\\Controllogix\\Resources\\SQL");
                    sqlReader.ExecutarEstrutura();
                    sqlReader.ExecutarDados();
                }
                else if (operacao.Equals("ProcessarExcel", StringComparison.OrdinalIgnoreCase)) // Novo método para processar arquivos Excel
                {
                    var excelReader = new LeitorExcel();
                    excelReader.ProcessarExcel();
                }
                else
                {
                    Console.WriteLine($"Operação desconhecida: '{operacao}'.");
                    Console.WriteLine("Operações válidas: SalvarL5X, ObterBlocosCSV, CriarParametrosCSV, CriarTabelas, InserirDados, ConsolidarEstrutura, ExecutarTabelasSQL, ExecutarDadosSQL, ProcessarExcel.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar a operação '{operacao}': {ex.Message}");
            }
        }
    }
}
