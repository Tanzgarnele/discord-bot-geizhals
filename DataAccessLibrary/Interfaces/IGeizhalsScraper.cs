using DataAccessLibrary.Models;

namespace ManfredHorst
{
    public interface IGeizhalsScraper
    {
        Task<Boolean> ScrapeGeizhals(Alarm alarm);
    }
}