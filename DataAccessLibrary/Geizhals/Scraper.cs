using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Geizhals
{
    public class Scraper
    {
        public GeizhalsProduct GetProducts(IHtmlDocument document)
        {
            GeizhalsProduct product = new GeizhalsProduct();
            product.Name = document.All.Where(x => x.ClassName == "variant__header__headline").FirstOrDefault().TextContent.Trim();
            product.Price = document.All.Where(x => x.ClassName == "gh_price").FirstOrDefault().TextContent.Replace("ab ", String.Empty).Replace("€ ", String.Empty).Trim();

            return product;
        }
    }
}