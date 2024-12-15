using System;
using System.Data.SqlClient;
using System.IO;

namespace SQLExecutor
{
    public class ExecutaSQL
    {
        private readonly string _connectionString;
        private readonly string _sqlDirectory;

        /// <summary>
        /// Construtor da classe ExecutaSQL.
        /// </summary>
        /// <param name="connectionString">String de conexão com o banco de dados.</param>
        /// <param name="sqlDirectory">Diretório contendo os arquivos .sql.</param>
        public ExecutaSQL(string connectionString, string sqlDirectory)
        {
            _connectionString = connectionString;
            _sqlDirectory = sqlDirectory;
        }

        /// <summary>
        /// CriarTabelaBanco todos os arquivos .sql no diretório especificado.
        /// </summary>
        public void ExecuteAllSqlFiles()
        {
            try
            {
                if (!Directory.Exists(_sqlDirectory))
                {
                    Console.WriteLine($"Diretório não encontrado: {_sqlDirectory}");
                    return;
                }

                // Lista todos os arquivos .sql no diretório
                string[] sqlFiles = Directory.GetFiles(_sqlDirectory, "*.sql");
                if (sqlFiles.Length == 0)
                {
                    Console.WriteLine("Nenhum arquivo .sql encontrado no diretório especificado.");
                    return;
                }

                Console.WriteLine($"Encontrados {sqlFiles.Length} arquivos .sql no diretório: {_sqlDirectory}");

                foreach (var sqlFile in sqlFiles)
                {
                    string sqlCommandText = File.ReadAllText(sqlFile);
                    Console.WriteLine($"Executando arquivo: {Path.GetFileName(sqlFile)}");

                    ExecuteSql(sqlCommandText);

                    Console.WriteLine($"Arquivo {Path.GetFileName(sqlFile)} executado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar os arquivos .sql: {ex.Message}");
            }
        }

        /// <summary>
        /// CriarTabelaBanco um comando SQL no banco de dados.
        /// </summary>
        /// <param name="sqlCommandText">Texto do comando SQL a ser executado.</param>
        private void ExecuteSql(string sqlCommandText)
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
    }

    /// <summary>
    /// Classe principal para execução.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // String de conexão com o banco de dados
            string connectionString = "Server=localhost;Database=Controllogix;User Id=seu_usuario;Password=sua_senha;";

            // Diretório onde estão os arquivos .sql
            string sqlDirectory = @"C:\RSLogix 5000\Projects\P83\SQL\Estrutura";

            // Instancia a classe ExecutaSQL e executa os arquivos .sql
            ExecutaSQL sqlExecutor = new ExecutaSQL(connectionString, sqlDirectory);
            sqlExecutor.ExecuteAllSqlFiles();

            Console.WriteLine("Execução de todos os arquivos .sql concluída.");
        }
    }
}
