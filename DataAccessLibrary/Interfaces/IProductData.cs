using DataAccessLibrary.Models;

namespace DataAccessLibrary.Interfaces
{
    public interface IProductData
    {
        Task InsertAlarm(Alarm alarm);

        Task InsertUser(User user);

        Task InsertDEBUGAlarm();

        Task<Int64> GetUserByMention(String mention);

        Task<List<Alarm>> GetAlarmsByMention(String mention);

        Task DeleteAlarm(String alias, String mention);

        Task<List<UserAlarm>> GetAlarms();
    }
}