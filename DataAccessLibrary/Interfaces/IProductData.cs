using DataAccessLibrary.Models;

namespace DataAccessLibrary.Interfaces
{
    public interface IProductData
    {
        Task InsertAlarm(Alarm alarm);

        Task InsertUser(User user);

        Task<Int64> GetUserByMention(String mention);

        Task<List<Alarm>> GetAlarmsByMention(String mention);
    }
}