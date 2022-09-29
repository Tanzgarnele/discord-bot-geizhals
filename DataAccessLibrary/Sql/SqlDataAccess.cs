using Dapper;
using DataAccessLibrary.Interfaces;
using Npgsql;
using System.Data;

namespace DataAccessLibrary.Sql
{
    public class SqlDataAccess : ISqlDataAccess
    {
        public String ConnectionStringName { get; set; } = "User ID=SA;Password=TEST123;Host=192.168.178.77;Port=49153;Database=GeizhalsDiscord;";

        public async Task<Int64> LoadData(String sql)
        {
            String connectionString = this.ConnectionStringName;

            try
            {
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                return await connection.ExecuteScalarAsync<Int64>(sql);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SaveData<T>(String sql, T parameters)
        {
            String connectionString = this.ConnectionStringName;

            try
            {
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}