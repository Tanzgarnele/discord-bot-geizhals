using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ManfredHorst;

public class TimerService : IHostedService, IDisposable
{
    private Timer timer;
    private readonly DiscordSocketClient client;
    private readonly IConfiguration config;
    private readonly IProductData productData;
    private readonly IGeizhalsScraper geizhalsScraper;

    public TimerService(DiscordSocketClient client, IConfiguration config, IProductData productData, IGeizhalsScraper geizhalsScraper)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        this.productData = productData ?? throw new ArgumentNullException(nameof(productData));
        this.geizhalsScraper = geizhalsScraper ?? throw new ArgumentNullException(nameof(geizhalsScraper));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(20));
        Console.WriteLine("StartAsyncStartAsyncStartAsyncStartAsyncStartAsyncStartAsync");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, 0);
        Console.WriteLine("StopAsyncStopAsyncStopAsyncStopAsyncStopAsyncStopAsyncStopAsync");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
        GC.SuppressFinalize(this);
    }

    private async void DoWork(object state)
    {
        foreach (Alarm alarm in await productData.GetAlarms())
        {
            if (await this.geizhalsScraper.ScrapeGeizhals(alarm))
            {
                if (client.GetChannel(Convert.ToUInt64(this.config["output:live"])) is IMessageChannel chan)
                {
                    Console.WriteLine($"Alarm {alarm.Alias} from {alarm.Mention} deleted {DateTime.Now}");
                    await chan.SendMessageAsync($"**{alarm.Alias}** below **{alarm.Price}€**\n{alarm.Url}\n {alarm.Mention} Alarm deleted!");
                    await this.productData.DeleteAlarm(alarm.Alias, alarm.Mention);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}