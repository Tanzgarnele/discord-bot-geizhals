using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Sql;

public class ProductData : IProductData
{
	private readonly ISqlDataAccess Db;
	private Random rnd = new Random();

	public ProductData(ISqlDataAccess db)
	{
		this.Db = db ?? throw new ArgumentNullException(nameof(db));
	}

	public Task<Int64> GetUserByMention(String mention)
	{
		String sql = $"SELECT id FROM users WHERE mention = '{mention}'";

		return this.Db.LoadScalarData(sql);
	}

	public Task<List<DatabaseAlarm>> GetAlarmsByMention(String mention)
	{
		String sql = $@"Select url, alias, price From urls WHERE userid IN (SELECT id FROM users WHERE mention = '{mention}')";

		return this.Db.LoadData<DatabaseAlarm, dynamic>(sql, new { });
	}

	public Task<List<Alarm>> GetAlarms()
	{
		String sql = $@"SELECT url, alias, price, mention FROM urls INNER JOIN users ON users.id = userid";

		return this.Db.LoadData<Alarm, dynamic>(sql, new { });
	}

	public Task InsertUser(User user)
	{
		String sql = $@"IF NOT EXISTS (SELECT * FROM users WHERE mention = @mention) BEGIN INSERT INTO users (mention, username, entryDate) VALUES (@mention, @username, @entryDate); END ";

		return this.Db.SaveData(sql, user);
	}

	public Task InsertAlarm(DatabaseAlarm alarm)
	{
		String sql = $@"INSERT INTO urls (url, alias, price, userid, entryDate) VALUES (@url, @alias, @price, @userid, @entryDate);";

		return this.Db.SaveData(sql, alarm);
	}

	public Task InsertDEBUGAlarm()
	{
		String sql = $@"INSERT INTO urls (url, alias, price, userid, entryDate) VALUES ('https://geizhals.de/', 'DebugName{rnd.Next()}', {rnd.Next(1, 999)}, 2, {DateTime.Now});";

		return this.Db.ExecuteSql(sql);
	}

	public Task DeleteAlarm(String alias, String mention)
	{
		String sql = $@"DELETE FROM urls WHERE alias = '{alias}' AND userid IN (SELECT id FROM users WHERE mention = '{mention}')";

		return this.Db.ExecuteSql(sql);
	}
}