using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Yaml;
using Discord.Commands;
using Discord;
using System.Diagnostics;

namespace ManfredHorst
{
    public class Program
    {
        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
                .AddYamlFile("config.yml")
                .Build();

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                services
                .AddSingleton(config)
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged,
                    AlwaysDownloadUsers = true
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton(x => new CommandService())
                .AddSingleton<InteractionHandler>()
                .AddSingleton<PrefixHandler>()
                ).Build();

            await RunAsync(host);
        }

        public async Task RunAsync(IHost host)
        {
            using IServiceScope servicescope = host.Services.CreateScope();
            IServiceProvider provider = servicescope.ServiceProvider;

            DiscordSocketClient client = provider.GetRequiredService<DiscordSocketClient>();
            InteractionService commands = provider.GetRequiredService<InteractionService>();

            await provider.GetRequiredService<InteractionHandler>().InitalizeAsync();

            IConfigurationRoot config = provider.GetRequiredService<IConfigurationRoot>();
            

            client.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
            commands.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
            client.Ready += async () =>
            {
                Console.WriteLine("DER DEUTSCHE BÄR IST ONLINE!");
                await commands.RegisterCommandsGloballyAsync(true);
            };

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken", EnvironmentVariableTarget.User));
            await client.StartAsync();

            await Task.Delay(-1);
        }

        static bool IsDebug()
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }


    }
}