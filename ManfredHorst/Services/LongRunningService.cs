﻿using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ManfredHorst.Services;

public class LongRunningService : DiscordClientService
{
    private readonly IConfiguration config;
    private readonly IProductData productData;
    private readonly IGeizhalsScraper geizhalsScraper;
    private Timer timer;

    public LongRunningService(DiscordSocketClient client, ILogger<DiscordClientService> logger, IConfiguration config, IProductData productData, IGeizhalsScraper geizhalsScraper) : base(client, logger)
    {
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        this.productData = productData ?? throw new ArgumentNullException(nameof(productData));
        this.geizhalsScraper = geizhalsScraper ?? throw new ArgumentNullException(nameof(geizhalsScraper));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);

        timer = new Timer(DoWork, !stoppingToken.IsCancellationRequested, TimeSpan.Zero, TimeSpan.FromMinutes(20));
        stoppingToken.Register(() => { timer.Dispose(); });
    }

    private async void DoWork(object state)
    {
        Logger.LogInformation("Scan starting! {dateTime}\n", DateTime.Now);
        foreach (Alarm alarm in await productData.GetAlarms())
        {
            if (await this.geizhalsScraper.ScrapeGeizhals(alarm))
            {
                if (Client.GetChannel(Convert.ToUInt64(this.config["output:live"])) is IMessageChannel chan)
                {
                    Logger.LogInformation("Alarm {Alias} from {Mention} deleted {DateTime}", alarm.Alias, alarm.Mention, DateTime.Now);
                    await chan.SendMessageAsync($"**{alarm.Alias}** below **{alarm.Price}€**\n{alarm.Url}\n {alarm.Mention} Alarm deleted!");
                    await this.productData.DeleteAlarm(alarm.Alias, alarm.Mention);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        Logger.LogInformation("Scan done! {dateTime}\n", DateTime.Now);
    }
}