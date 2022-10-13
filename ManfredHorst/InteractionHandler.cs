using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace ManfredHorst
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient client;
        private readonly InteractionService commands;
        private readonly IServiceProvider services;

        public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            this.client = client;
            this.commands = commands;
            this.services = services;
        }

        public async Task InitalizeAsync()
        {
            this.client.Ready += ReadyAsync;
            this.commands.Log += LogAsync;

            await this.commands.AddModulesAsync(Assembly.GetEntryAssembly(), this.services);

            client.InteractionCreated += HandleInteraction;
        }

        private async Task ReadyAsync()
        {
            await this.commands.RegisterCommandsGloballyAsync(true);
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                SocketInteractionContext context = new SocketInteractionContext(client, arg);

                IResult result = await commands.ExecuteCommandAsync(context, this.services);

                if (!result.IsSuccess)
                {
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            Console.WriteLine(result.Error);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch
            {
                if (arg.Type is InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }

        private async Task LogAsync(LogMessage log)
           => Console.WriteLine(log);
    }
}