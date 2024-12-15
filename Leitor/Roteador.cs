using System;
using System.Xml;

namespace Leitor
{
    public class Roteador
    {
        public void Processar(string operacao)
        {
            try
            {
                if (operacao.Equals("ObterBlocosCSV", StringComparison.OrdinalIgnoreCase))
                {
                    // Operações relacionadas a arquivos L5X
                    var l5xReader = new LeitorL5X();
                    l5xReader.Processa();
                }
                /*else if (operacao.Equals("CriarParametrosCSV", StringComparison.OrdinalIgnoreCase))
                {
                    // Operação relacionada a arquivos CSV
                    var csvReader = new LeitorCSV();
                    csvReader.Processa();
                }*/
                /*else if (operacao.Equals("CriarTabelas", StringComparison.OrdinalIgnoreCase))
                {
                    // Operação relacionada a arquivos SQL
                    var sqlReader = new LeitorSQL();
                    sqlReader.Processa();
                }*/
                else
                {
                    Console.WriteLine($"Operação desconhecida: '{operacao}'.");
                    Console.WriteLine("Operações válidas: ConverterCSV, CriarInserts, CriarSQLEstruturaTabelas, CriarParametrosCSV, CriarTabelas.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar a operação '{operacao}': {ex.Message}");
            }
        }
    }
}
