using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Price_Bot
{
    public class PrefixHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private readonly IConfiguration config;

        public PrefixHandler(DiscordSocketClient client, CommandService commands, IConfiguration config)
        {
            this.client = client;
            this.commands = commands;
            this.config = config;
        }

        public async Task InitializeAsync()
        {
            this.client.MessageReceived += HandleCommandAsync;
        }

        public void AddModule<T>()
        {
            this.commands.AddModuleAsync<T>(null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            SocketUserMessage message = (SocketUserMessage)messageParam;

            if (message is null)
            {
                return;
            }

            int argPos = 0;

            if (!(message.HasCharPrefix(this.config["prefix"][0], ref argPos)) || !message.HasMentionPrefix(this.client.CurrentUser, ref argPos) || message.Author.IsBot)
            {
                return;
            }

            SocketCommandContext context = new SocketCommandContext(this.client, message);

            await this.commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

        }
    }
}
