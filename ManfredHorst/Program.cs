using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Scraper;
using DataAccessLibrary.Sql;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ManfredHorst;

public class Program
{
    private readonly IConfiguration config;
    private readonly DiscordSocketClient client;

    public Program(DiscordSocketClient client, IConfiguration config)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
                    .AddYamlFile("config.yml")
                    .Build();

                services.AddSingleton<IConfiguration>(config);

                services.AddSingleton(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged,
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Info,
                    MessageCacheSize = 50,
                });

                services.AddHostedService<TimerService>();
                services.AddSingleton<TimerService>();
                services.AddSingleton<Program>();
                services.AddSingleton<DiscordSocketClient>();
                services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
                services.AddSingleton<InteractionHandler>();
                services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
                services.AddSingleton<IProductData, ProductData>();
                services.AddSingleton<IGeizhalsScraper, GeizhalsScraper>();

            });
    }

    private static void Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        host.Start();
        host.Services.GetRequiredService<InteractionHandler>().InitalizeAsync().GetAwaiter().GetResult();
        host.Services.GetRequiredService<Program>().RunAsync().GetAwaiter().GetResult();
    }


    public async Task RunAsync()
    {
        client.Log += LogAsync;
        Console.WriteLine($"{DateTime.Now} SQL Server is online! Starting the Bot now.");

        await client.LoginAsync(TokenType.Bot, this.config["tokens:discord"]);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private async Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
    }
}