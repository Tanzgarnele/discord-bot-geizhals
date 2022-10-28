namespace DataAccessLibrary.Models;

public class Alarm
{
    public String Url { get; set; }
    public String Alias { get; set; }
    public Double Price { get; set; }
    public String Mention { get; set; }

    public String ProductUrl { get; set; }
    public String ProductName { get; set; }
    public Double ProductPrice { get; set; }
}