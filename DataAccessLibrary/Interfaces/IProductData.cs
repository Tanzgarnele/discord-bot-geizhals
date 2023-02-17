using DataAccessLibrary.Models;

namespace DataAccessLibrary.Interfaces;

public interface IProductData
{
    Task InsertAlarm(DatabaseAlarm alarm);

    Task InsertUser(User user);

    Task InsertDEBUGAlarm();

    Task<Int64> GetUserByMention(String mention);

    Task UpdateCurrentPrice(String alias, String mention, Double currentPrice);

    Task UpdateLastPrice(String alias, String mention, Double lastPrice);

    Task<List<DatabaseAlarm>> GetAlarmsByMention(String mention);

    Task DeleteAlarm(String alias, String mention);

    Task<List<Alarm>> GetAlarms();

    Task<Alarm> GetAlarmByAlias(String alias);

    Task UpdateThumbnailUrl(String alias, String mention, String url);
}