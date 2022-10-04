using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Sql
{
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess Db;
        private Random rnd = new Random();

        public ProductData(ISqlDataAccess db)
        {
            this.Db = db;
        }

        public Task<Int64> GetUserByMention(String mention)
        {
            String sql = $"SELECT id FROM users WHERE mention = '{mention}'";

            return this.Db.LoadScalarData(sql);
        }

        public Task<List<Alarm>> GetAlarmsByMention(String mention)
        {
            String sql = $@"Select url, alias, price From urls WHERE userid IN (SELECT id FROM users WHERE mention = '{mention}')";

            return this.Db.LoadData<Alarm, dynamic>(sql, new { });
        }

        public Task<List<UserAlarm>> GetAlarms()
        {
            String sql = $@"SELECT url, alias, price, mention FROM public.urls INNER JOIN users ON users.id = userid";

            return this.Db.LoadData<UserAlarm, dynamic>(sql, new { });
        }

        public Task InsertUser(User user)
        {
            String sql = $@"INSERT INTO users (mention, username, lastseen) VALUES (@mention, @username, @lastseen) ON CONFLICT DO NOTHING;";

            return this.Db.SaveData(sql, user);
        }

        public Task InsertAlarm(Alarm alarm)
        {
            String sql = $@"INSERT INTO Urls (url, alias, price, userid) VALUES (@url, @alias, @price, @userid) ON CONFLICT DO NOTHING;";

            return this.Db.SaveData(sql, alarm);
        }

        public Task InsertDEBUGAlarm()
        {
            String sql = $@"INSERT INTO Urls (url, alias, price, userid) VALUES ('https://geizhals.de/', 'DebugName{rnd.Next()}', {rnd.Next(1, 999)}, 2) ON CONFLICT DO NOTHING;";

            return this.Db.ExecuteSql(sql);
        }

        public Task DeleteAlarm(String alias, String mention)
        {
            String sql = $@"DELETE FROM Urls WHERE alias = '{alias}' AND userid IN (SELECT id FROM users WHERE mention = '{mention}')";

            return this.Db.ExecuteSql(sql);
        }
    }
}