using Dapper;
using DataAccessLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLibrary.Sql;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration config;
    public String ConnectionString { get; set; }

    public SqlDataAccess(IConfiguration config)
    {
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        ConnectionString = this.config["database:connectionString"];
    }

    public async Task<Int64> LoadScalarData(String sql)
    {
        try
        {
            using IDbConnection connection = new SqlConnection(this.ConnectionString);
            return await connection.ExecuteScalarAsync<Int64>(sql);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<List<T>> LoadDataList<T, U>(String sql, U parameters)
    {
        using (IDbConnection connection = new SqlConnection(this.ConnectionString))
        {
            IEnumerable<T> data = await connection.QueryAsync<T>(sql, parameters);

            return data.ToList();
        }
    }

    public async Task<T> LoadData<T, U>(String sql, U parameters)
    {
        using (IDbConnection connection = new SqlConnection(this.ConnectionString))
        {
            T data = await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);

            return data;
        }
    }

    public async Task SaveData<T>(String sql, T parameters)
    {
        try
        {
            using IDbConnection connection = new SqlConnection(this.ConnectionString);
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
        try
        {
            using IDbConnection connection = new SqlConnection(this.ConnectionString);
            await connection.ExecuteAsync(sql);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public Boolean IsServerConnected()
    {
        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}