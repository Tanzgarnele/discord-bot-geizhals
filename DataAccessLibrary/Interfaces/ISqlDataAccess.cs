namespace DataAccessLibrary.Interfaces
{
    public interface ISqlDataAccess
    {
        String ConnectionStringName { get; set; }

        Task SaveData<T>(String sql, T parameters);
        Task<Int64> LoadData(String sql);
    }
}