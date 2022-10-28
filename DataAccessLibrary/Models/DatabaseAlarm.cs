namespace DataAccessLibrary.Models;

public class DatabaseAlarm
{
    public String Url { get; set; }
    public String Alias { get; set; }
    public Double Price { get; set; }
    public Int64 UserId { get; set; }
    public DateTime EntryDate { get; set; }
}