using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Scraper;
using DataAccessLibrary.Sql;
using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using ManfredHorst;
using ManfredHorst.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static async Task Main(String[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
            .AddYamlFile("config.yml")
            .Build();

        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureDiscordHost((context, discordConfig) =>
            {
                discordConfig.SocketConfig = new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200
                };

                context.Configuration = config;

                discordConfig.Token = context.Configuration["tokens:discord"];
            })
             .UseInteractionService((context, config) =>
             {
                 config.LogLevel = LogSeverity.Info;
                 config.UseCompiledLambda = true;
             })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(config => config.AddConsole());

                services.AddHostedService<BotStatusService>();
                services.AddHostedService<InteractionHandler>();
                services.AddHostedService<LongRunningService>();

                services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                services.AddSingleton<IConfiguration>(config);
                services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
                services.AddSingleton<IProductData, ProductData>();
                services.AddSingleton<IGeizhalsScraper, GeizhalsScraper>();

            }).Build();

        await host.RunAsync();
    }
}