using Dapper;
using DataAccessLibrary.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLibrary.Sql
{
    public class SqlDataAccess : ISqlDataAccess
    {
        //public String ConnectionStringName { get; set; } = "User ID=SA;Password=TEST123;Host=192.168.178.77;Port=49153;Database=GeizhalsDiscord;";
        public String ConnectionStringName { get; set; } = "Data Source=192.168.178.55,1433;User ID=sa;Password=Test@Tester1234;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Database=GeizhalsDiscord;";

        public async Task<Int64> LoadScalarData(String sql)
        {
            String connectionString = this.ConnectionStringName;

            try
            {
                using IDbConnection connection = new SqlConnection(connectionString);
                return await connection.ExecuteScalarAsync<Int64>(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<T>> LoadData<T, U>(String sql, U parameters)
        {
            String connectionString = this.ConnectionStringName;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                IEnumerable<T> data = await connection.QueryAsync<T>(sql, parameters);

                return data.ToList();
            }
        }

        public async Task SaveData<T>(String sql, T parameters)
        {
            String connectionString = this.ConnectionStringName;

            try
            {
                using IDbConnection connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task ExecuteSql(String sql)
        {
            String connectionString = this.ConnectionStringName;

            try
            {
                using IDbConnection connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}