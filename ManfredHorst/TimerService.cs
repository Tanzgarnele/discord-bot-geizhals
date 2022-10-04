using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DataAccessLibrary.Models;
using DataAccessLibrary.Sql;
using Discord;
using Discord.WebSocket;

namespace ManfredHorst
{
    public class TimerService
    {
        private Timer timer;
        private ProductData productData;
        private readonly DiscordSocketClient client;

        public TimerService(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task InitalizeAsync()
        {
            timer = new Timer(async _ =>
            {
                List<UserAlarm> userAlarms = new List<UserAlarm>();
                this.productData = new ProductData(new SqlDataAccess());
                userAlarms = await productData.GetAlarms();

                foreach (UserAlarm alarm in userAlarms)
                {
                    await GetHtmlAsync(alarm);
                }
            },
            null,
            TimeSpan.FromSeconds(5),
            TimeSpan.FromMinutes(20));
        }

        private async Task GetHtmlAsync(UserAlarm alarm)
        {
            if (alarm is null)
            {
                throw new ArgumentNullException(nameof(alarm));
            }

            CancellationTokenSource cancellationToken = new();
            HttpClient httpClient = new();
            HttpResponseMessage request = await httpClient.GetAsync(alarm.Url);
            cancellationToken.Token.ThrowIfCancellationRequested();

            Stream response = await request.Content.ReadAsStreamAsync();
            cancellationToken.Token.ThrowIfCancellationRequested();

            HtmlParser parser = new();
            IHtmlDocument document = parser.ParseDocument(response);

            GeizhalsProduct product = new();

            if (alarm.Url.Contains("geizhals.de") && alarm.Url.Contains(".html?hloc"))
            {
                product = GetProducts(document);
                product.LatestTime = DateTime.Now;
                product.ProductUrl = alarm.Url;
            }

            if (Convert.ToDouble(product.Price) <= Convert.ToDouble(alarm.Price) && Convert.ToDouble(alarm.Price) != 0 && Convert.ToDouble(product.Price) != 0)
            {
                //IMessageChannel? chan = client.GetChannel(785318419750191114) as IMessageChannel;
                IMessageChannel? chan = client.GetChannel(570446080697827334) as IMessageChannel;

                if (chan != null)
                {
                    await chan.SendMessageAsync($"**{alarm.Alias}** below **{alarm.Price}€**\n{alarm.Url}\n {alarm.Mention} Alarm deleted!");
                }

                await this.productData.DeleteAlarm(alarm.Alias, alarm.Mention);
            }
        }

        public GeizhalsProduct GetProducts(IHtmlDocument document)
        {
            GeizhalsProduct product = new()
            {
                Name = document.All.Where(x => x.ClassName == "variant__header__headline").FirstOrDefault().TextContent.Trim(),
                Price = document.QuerySelectorAll("div.offer__price .gh_price").FirstOrDefault().TextContent.Replace("ab ", String.Empty).Replace("€ ", String.Empty).Trim()
            };

            return product;
        }
    }
}