namespace DataAccessLibrary.Interfaces
{
    public interface ISqlDataAccess
    {
        String ConnectionStringName { get; set; }

        Task SaveData<T>(String sql, T parameters);

        Task<Int64> LoadScalarData(String sql);

        Task<List<T>> LoadData<T, U>(String sql, U parameters);

        Task ExecuteSql(String sql);
    }
}