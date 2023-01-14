using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Scraper;
using DataAccessLibrary.Sql;
using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using ManfredHorst;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureDiscordHost((context, config) =>
    {
        config.SocketConfig = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 200
        };

        context.Configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
            .AddYamlFile("config.yml")
            .Build();

        config.Token = context.Configuration["tokens:discord"];
    })
    .ConfigureServices((hostContext, services) =>
    {
        IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
            .AddYamlFile("config.yml")
            .Build();

        services.AddSingleton<IConfiguration>(config);

        services.AddHostedService<BotStatusService>();
        services.AddHostedService<LongRunningService>();
        //services.AddHostedService<TimerService>();
        //services.AddSingleton<DiscordService>();
        //services.AddSingleton<TimerService>();
        //services.AddSingleton<DiscordSocketClient>();
        //services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        //services.AddSingleton<InteractionHandler>();
        services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
        services.AddSingleton<IProductData, ProductData>();
        services.AddSingleton<IGeizhalsScraper, GeizhalsScraper>();
    }).Build();

//host.Services.GetRequiredService<InteractionHandler>().InitalizeAsync().GetAwaiter().GetResult();
await host.RunAsync();