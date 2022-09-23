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
            await this.commands.AddModulesAsync(Assembly.GetEntryAssembly(), this.services);

            client.InteractionCreated += HandleInteraction;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var context = new SocketInteractionContext(this.client, arg);
                    await this.commands.ExecuteCommandAsync(context, this.services);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
