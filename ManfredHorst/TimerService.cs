using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DataAccessLibrary.Models;
using DataAccessLibrary.Sql;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace ManfredHorst
{
    public class TimerService
    {
        private ProductData productData;
        private readonly DiscordSocketClient client;
        private Timer timer;
        private readonly IConfiguration config;

        public TimerService(DiscordSocketClient client, IConfiguration config)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task InitalizeAsync()
        {
            try
            {
                timer = new Timer(async _ =>
                {
                    List<UserAlarm> userAlarms = new List<UserAlarm>();
                    this.productData = new ProductData(new SqlDataAccess(this.config));
                    userAlarms = await productData.GetAlarms();

                    //Console.WriteLine($"Starting scan {DateTime.Now}");

                    foreach (UserAlarm alarm in userAlarms)
                    {
                        await GetHtmlAsync(alarm);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                },
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMinutes(20));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task GetHtmlAsync(UserAlarm alarm)
        {
            if (alarm is null)
            {
                throw new ArgumentNullException(nameof(alarm));
            }

            Console.WriteLine($"Scanning {alarm.Alias} {DateTime.Now}");

            CancellationTokenSource cancellationToken = new();
            HttpClient httpClient = new();

            HttpResponseMessage request = await httpClient.GetAsync($"{alarm.Url}&sort=p");
            cancellationToken.Token.ThrowIfCancellationRequested();

            Stream response = await request.Content.ReadAsStreamAsync();
            cancellationToken.Token.ThrowIfCancellationRequested();

            HtmlParser parser = new();
            IHtmlDocument document = parser.ParseDocument(response);

            GeizhalsProduct product = new();

            if (alarm.Url.Contains("geizhals.de") && alarm.Url.Contains(".htm"))
            {
                product = GetSingleProduct(document);
                product.LatestTime = DateTime.Now;
                product.ProductUrl = alarm.Url;
                Console.WriteLine($"Checking single item {alarm.Alias} {alarm.Price}€ Current Price at: {product.Price}€");
            }
            else if (alarm.Url.Contains("geizhals.de") && alarm.Url.Contains("cat="))
            {
                product = GetProductList(document);
                product.LatestTime = DateTime.Now;
                product.ProductUrl = alarm.Url;
                Console.WriteLine($"Checking filter items {alarm.Alias} {alarm.Price}€ Current Price at: {product.Price}€");
            }

            if (Convert.ToDouble(product.Price) <= Convert.ToDouble(alarm.Price) && !String.IsNullOrWhiteSpace(product.Price))
            {
                if (client.GetChannel(570446080697827334) is IMessageChannel chan)
                {
                    Console.WriteLine($"Alarm {alarm.Alias} from {alarm.Mention} deleted {DateTime.Now}");
                    await chan.SendMessageAsync($"**{alarm.Alias}** below **{alarm.Price}€**\n{alarm.Url}\n {alarm.Mention} Alarm deleted!");
                }

                await this.productData.DeleteAlarm(alarm.Alias, alarm.Mention);
            }
        }

        public GeizhalsProduct GetSingleProduct(IHtmlDocument document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            GeizhalsProduct product = new GeizhalsProduct();
            if (document.StatusCode == System.Net.HttpStatusCode.OK)
            {
                product.Name = document.QuerySelectorAll("h1.variant__header__headline")
                    .FirstOrDefault()
                    .TextContent
                    .Trim();

                product.Price = document.QuerySelectorAll("#offer__price-0 .gh_price")
                    .FirstOrDefault().TextContent
                    .Replace("ab ", String.Empty)
                    .Replace("€ ", String.Empty)
                    .Trim();
                Console.WriteLine(product.Price);
            }
            else
            {
                Console.WriteLine($"Error: {document.StatusCode}");
            }
            return product;
        }

        public GeizhalsProduct GetProductList(IHtmlDocument document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            GeizhalsProduct product = new GeizhalsProduct();
            if (document.StatusCode == System.Net.HttpStatusCode.OK)
            {
                product.Name = document.QuerySelectorAll("#product0 div.productlist__item .notrans")
                    .FirstOrDefault()
                    .TextContent
                    .Trim();

                product.Price = document.QuerySelectorAll("#product0 div.productlist__price .gh_price")
                    .FirstOrDefault().TextContent
                    .Replace("ab ", String.Empty)
                    .Replace("€ ", String.Empty)
                    .Trim();
            }
            else
            {
                Console.WriteLine($"Error: {document.StatusCode}");
            }
            return product;
        }
    }
}