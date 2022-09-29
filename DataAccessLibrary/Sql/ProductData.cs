using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Sql
{
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess Db;

        public ProductData(ISqlDataAccess db)
        {
            this.Db = db;
        }

        //public Task<Alarm> GetAlarms()
        //{
        //    String sql = $"SELECT * FROM url";

        //    return this.Db.LoadInt64(sql);
        //}

        public Task<Int64> GetUserByMention(String mention)
        {
            String sql = $"SELECT id FROM users WHERE mention = '{mention}'";

            return this.Db.LoadData(sql);
        }

        public Task InsertUser(User user)
        {
            String sql = $@"INSERT INTO users (mention, username, lastseen)
							VALUES (@mention, @username, @lastseen) ON CONFLICT DO NOTHING;";

            return this.Db.SaveData(sql, user);
        }

        public Task InsertAlarm(Alarm alarm)
        {
            String sql = $@"INSERT INTO Urls (url, alias, price, userid)
							VALUES (@url, @alias, @price, @userid) ON CONFLICT DO NOTHING;";

            return this.Db.SaveData(sql, alarm);
        }
    }
}