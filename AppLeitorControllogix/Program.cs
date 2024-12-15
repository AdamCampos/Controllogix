using Leitor;

class Program
{
    static void Main(string[] args)
    {
        string directoryPath = @"C:\RSLogix 5000\Projects\P83";
        L5XReader reader = new L5XReader();
        //reader.ProcessFiles(directoryPath, "CriarSQLEstruturaTabelas");
        reader.ProcessFiles(directoryPath, "CriarTabelas");
        //reader.ProcessFiles(directoryPath, "CriarInserts");
        //reader.ProcessFiles(directoryPath, "ConverterCSV");
        //reader.ProcessFiles(directoryPath, "CriarParametrosCSV");
        //reader.ProcessFiles(directoryPath, "SalvarSQL");
    }
}