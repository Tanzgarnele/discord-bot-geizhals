using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Sql;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;

namespace ManfredHorst
{
	public class Program
	{
		private readonly IConfiguration config;
		private readonly IServiceProvider services;

		private readonly DiscordSocketConfig socketConfig = new()
		{
			GatewayIntents = GatewayIntents.AllUnprivileged,
			AlwaysDownloadUsers = true,
			LogLevel = LogSeverity.Info,
			MessageCacheSize = 50,
			UseInteractionSnowflakeDate = true,
		};

		public Program()
		{
			this.config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
				.AddYamlFile("config.yml")
				.Build();

			this.services = new ServiceCollection()
				.AddSingleton(this.config)
				.AddSingleton(this.socketConfig)
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
				.AddSingleton(x => new TimerService(x.GetRequiredService<DiscordSocketClient>()))
				.AddSingleton<InteractionHandler>()
				.AddSingleton<ISqlDataAccess, SqlDataAccess>()
				.AddSingleton<IProductData, ProductData>()
				.BuildServiceProvider();
		}

		private static void Main(string[] args)
			=> new Program().RunAsync()
				.GetAwaiter()
				.GetResult();

		public async Task RunAsync()
		{
			DiscordSocketClient client = this.services.GetRequiredService<DiscordSocketClient>();

			client.Log += LogAsync;

			await this.services.GetRequiredService<InteractionHandler>().InitalizeAsync();
			//await this.services.GetRequiredService<TimerService>().InitalizeAsync();

			await client.LoginAsync(TokenType.Bot, this.config["tokens:discord"]);
			await client.StartAsync();

			await Task.Delay(Timeout.Infinite);
		}

		private async Task LogAsync(LogMessage message)
			=> Console.WriteLine(message.ToString());
	}
}