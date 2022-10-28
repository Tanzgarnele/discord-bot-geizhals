using DataAccessLibrary.Models;

namespace DataAccessLibrary.Interfaces;

public interface IProductData
{
    Task InsertAlarm(DatabaseAlarm alarm);

    Task InsertUser(User user);

    Task InsertDEBUGAlarm();

    Task<Int64> GetUserByMention(String mention);

    Task<List<DatabaseAlarm>> GetAlarmsByMention(String mention);

    Task DeleteAlarm(String alias, String mention);

    Task<List<Alarm>> GetAlarms();
}