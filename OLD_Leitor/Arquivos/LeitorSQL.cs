using System;
using System.Data.SqlClient;
using System.IO;

namespace Leitor
{
    public class LeitorSQL
    {
        private readonly string _connectionString;
        private readonly string _sqlDirectory;

        /// <summary>
        /// Construtor da classe LeitorSQL.
        /// </summary>
        /// <param name="connectionString">String de conexão com o banco de dados.</param>
        /// <param name="sqlDirectory">Diretório base contendo os arquivos .sql.</param>
        public LeitorSQL(string connectionString, string sqlDirectory)
        {
            _connectionString = connectionString;
            _sqlDirectory = sqlDirectory;
        }

        /// <summary>
        /// Executa os arquivos .sql para criar a estrutura do banco de dados.
        /// </summary>
        public void ExecutarEstrutura()
        {
            string estruturaPath = Path.Combine(_sqlDirectory, "Estrutura");
            ExecutarArquivosSql(estruturaPath);
        }

        /// <summary>
        /// Executa os arquivos .sql para inserir os dados no banco de dados.
        /// </summary>
        public void ExecutarDados()
        {
            string dadosPath = Path.Combine(_sqlDirectory, "Dados");
            ExecutarArquivosSql(dadosPath);
        }

        /// <summary>
        /// Lê e executa todos os arquivos .sql em um diretório.
        /// </summary>
        /// <param name="directory">Diretório contendo os arquivos .sql.</param>
        private void ExecutarArquivosSql(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Diretório não encontrado: {directory}");
                return;
            }

            string[] sqlFiles = Directory.GetFiles(directory, "*.sql");
            if (sqlFiles.Length == 0)
            {
                Console.WriteLine($"Nenhum arquivo .sql encontrado no diretório: {directory}");
                return;
            }

            Console.WriteLine($"Executando {sqlFiles.Length} arquivos .sql no diretório: {directory}");

            foreach (var sqlFile in sqlFiles)
            {
                string sqlCommandText = File.ReadAllText(sqlFile);
                Console.WriteLine($"Executando arquivo: {Path.GetFileName(sqlFile)}");
                ExecutarComandoSql(sqlCommandText);
                Console.WriteLine($"Arquivo {Path.GetFileName(sqlFile)} executado com sucesso.");
            }
        }

        /// <summary>
        /// Executa um comando SQL no banco de dados.
        /// </summary>
        /// <param name="sqlCommandText">Texto do comando SQL a ser executado.</param>
        private void ExecutarComandoSql(string sqlCommandText)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlCommandText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar o comando SQL: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa ambos os métodos de estrutura e inserção de dados.
        /// </summary>
        public void ExecutarTudo()
        {
            Console.WriteLine("Iniciando execução da estrutura...");
            ExecutarEstrutura();

            Console.WriteLine("Iniciando execução dos dados...");
            ExecutarDados();

            Console.WriteLine("Execução de todos os arquivos concluída.");
        }
    }
}
