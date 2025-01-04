using Leitor;
using RockwellAutomation.LogixDesigner.LogixProjectServices;

class Program
{
    static void Main(string[] args)
    {
        RockwellAutomation.LogixDesigner.LogixProjectServices.IFileReader fileReader;

        // Verifica se um argumento foi fornecido na execução do programa
        if (args.Length < 1)
        {
            Console.WriteLine("Por favor, forneça a operação como argumento.");
            Console.WriteLine("Exemplo de operações válidas: ObterBlocosCSV, CriarTabelas, InserirDados.");
            return;
        }

        // Recupera o argumento da operação
        string operacao = args[0];

        try
        {
            // Instancia o roteador e processa a operação especificada
            Roteador roteador = new Roteador();
            roteador.Processar(operacao);
        }
        catch (Exception ex)
        {
            // Captura e exibe quaisquer erros ocorridos durante o processamento
            Console.WriteLine($"Erro ao executar a operação '{operacao}': {ex.Message}");
        }
    }
}
