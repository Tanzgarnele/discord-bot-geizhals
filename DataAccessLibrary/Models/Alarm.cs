namespace DataAccessLibrary.Models;

public class Alarm
{
    public String UserUrl { get; set; }
    public String UserAlias { get; set; }
    public Double UserPrice { get; set; }
    public String Mention { get; set; }

    public String ProductUrl { get; set; }
    public String ProductName { get; set; }
    public Double ProductPrice { get; set; }
}