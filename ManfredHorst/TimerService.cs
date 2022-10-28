using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DataAccessLibrary.Models;
using DataAccessLibrary.Sql;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace ManfredHorst;

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
                List<Alarm> listOfAlarms = new List<Alarm>();
                this.productData = new ProductData(new SqlDataAccess(this.config));
                listOfAlarms = await productData.GetAlarms();

                Console.WriteLine($"Starting scan {DateTime.Now}");

                foreach (Alarm alarm in listOfAlarms)
                {
                    await GetHtmlAsync(alarm);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                //await this.GetHtmlAsync(new Alarm() { UserUrl = @"https://geizhals.de/?cat=ramddr3&xf=15903_DDR5%7E253_32768%7E440_AMD+EXPO%7E5015_6000", UserAlias = "CPU", UserPrice = 111, Mention = "<@!234400000254279680>" });
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

    private async Task GetHtmlAsync(Alarm alarm)
    {
        if (alarm is null)
        {
            throw new ArgumentNullException(nameof(alarm));
        }

        Console.WriteLine($"Scanning {alarm.UserAlias} {DateTime.Now}");

        if (!alarm.UserUrl.Contains("&sort=p"))
        {
            alarm.UserUrl += "&sort=p";
        }

        IHtmlDocument document = await GetHtmlDocument(alarm.UserUrl);

        alarm = await this.GetProduct(document, alarm);
        Console.WriteLine($"Checking item: {alarm.UserAlias}\nAlarm   Price: {alarm.UserPrice}€\nCurrent Price: {alarm.ProductPrice}€\n");
        if (alarm.ProductPrice >= alarm.UserPrice)
        {
            if (client.GetChannel(Convert.ToUInt64(this.config["output:debug"])) is IMessageChannel chan)
            {
                Console.WriteLine($"Alarm {alarm.UserAlias} from {alarm.Mention} deleted {DateTime.Now}");
                await chan.SendMessageAsync($"**{alarm.UserAlias}** below **{alarm.UserPrice}€**\n{alarm.ProductUrl}\n {alarm.Mention} Alarm deleted!");
            }

            await this.productData.DeleteAlarm(alarm.UserAlias, alarm.Mention);
        }
    }

    private async Task<IHtmlDocument> GetHtmlDocument(String url)
    {
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        HttpResponseMessage request = await new HttpClient().GetAsync(url);
        cancellationToken.Token.ThrowIfCancellationRequested();

        HtmlParser parser = new();
        IHtmlDocument document = parser.ParseDocument(await request.Content.ReadAsStreamAsync());
        cancellationToken.Token.ThrowIfCancellationRequested();

        return document;
    }

    public async Task<Alarm> GetProduct(IHtmlDocument document, Alarm alarm)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        if (document.StatusCode == System.Net.HttpStatusCode.OK)
        {
            if (this.GetProductMethod(document).Equals("a"))
            {
                alarm.ProductName = this.GetProductName(document, "h1.variant__header__headline");
                alarm.ProductPrice = this.GetProductPrice(document, "#offer__price-0 .gh_price");
                alarm.ProductUrl = await this.GetProductUrl(document, "#offer__price-0 div.offer__clickout a");
            }
            else if (this.GetProductMethod(document).Equals("cat"))
            {
                alarm.ProductName = this.GetProductName(document, "#product0 div.productlist__item .notrans");
                alarm.ProductPrice = this.GetProductPrice(document, "#product0 div.productlist__price .gh_price");
                alarm.ProductUrl = await this.GetProductUrl(document, "#product0 div.productlist__bestpriceoffer a");
            }
        }
        else
        {
            Console.WriteLine($"Error: {document.StatusCode}");
        }
        return alarm;
    }

    private String GetProductMethod(IHtmlDocument document)
    {
        return document.QuerySelectorAll("body").Select(x => x.GetAttribute("data-what")).FirstOrDefault();
    }

    private Double GetProductPrice(IHtmlDocument document, String selector)
    {
        return Convert.ToDouble(document.QuerySelectorAll(selector)
                                .FirstOrDefault().TextContent
                                .Replace("ab ", String.Empty)
                                .Replace("€ ", String.Empty)
                                .Trim());
    }

    private String GetProductName(IHtmlDocument document, String selector)
    {
        return document.QuerySelectorAll(selector)
                            .FirstOrDefault()
                            .TextContent
                            .Trim();
    }

    private async Task<String> GetProductUrl(IHtmlDocument document, String selector)
    {
        if (selector.Equals("#product0 div.productlist__bestpriceoffer a"))
        {
            document = await this.GetHtmlDocument($@"https://geizhals.de/{document.QuerySelectorAll(selector)
                                .Select(x => x.GetAttribute("href"))
                                .FirstOrDefault()}");

            selector = "#offer__price-0 div.offer__clickout a";
        }

        return $@"https://geizhals.de{document.QuerySelectorAll(selector)
                                .Select(x => x.GetAttribute("href"))
                                .FirstOrDefault()}";
    }
}