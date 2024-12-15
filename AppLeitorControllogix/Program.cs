using Leitor;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Por favor, forneça a operação como argumento.");
            Console.WriteLine("Exemplo: ConverterCSV");
            return;
        }

        string operacao = args[0];

        try
        {
            Roteador roteador = new Roteador();
            roteador.Processar(operacao);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao executar a operação '{operacao}': {ex.Message}");
        }

        //Ao rodar no console, é necessário passar uma string para direcionar a ação.

        /* As opções são:
        
        1) ObterBlocosCSV   |   Converte cada arquivo L5X em vários CSV, sendo um CSV por tipo de dado encontrado no L5X. 
        2) 
        
         */

        //reader.Processa(caminho, "CriarSQLEstruturaTabelas");
        //reader.Processa(caminho, "CriarTabelas");
        //reader.Processa(caminho, "CriarInserts");
        //reader.Processa(caminho, "ConverterCSV");
        //reader.Processa(caminho, "CriarParametrosCSV");
        //reader.Processa(caminho, "SalvarSQL");
    }
}