using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Price_Bot
{
    public class Program
    {
        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {
            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                services.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged,
                    AlwaysDownloadUsers = true
                }))).Build();

            await RunAsync(host);
        }

        public async Task RunAsync(IHost host)
        {
            using IServiceScope servicescope = host.Services.CreateScope();
            IServiceProvider provider = servicescope.ServiceProvider;

            DiscordSocketClient client = provider.GetRequiredService<DiscordSocketClient>();

            client.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
            client.Ready += async () =>
            {
                Console.WriteLine("Bot started!");
            };

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken", EnvironmentVariableTarget.User));
            await client.StartAsync();

            await Task.Delay(-1);
        }


    }
}