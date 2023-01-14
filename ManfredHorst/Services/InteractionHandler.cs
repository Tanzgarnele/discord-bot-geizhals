using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ManfredHorst.Services;

internal class InteractionHandler : DiscordClientService
{
    private readonly IServiceProvider provider;
    private readonly InteractionService interactionService;

    public InteractionHandler(DiscordSocketClient client, ILogger<DiscordClientService> logger, IServiceProvider provider, InteractionService interactionService) : base(client, logger)
    {
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        this.interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.InteractionCreated += HandleInteraction;

        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        await Client.WaitForReadyAsync(stoppingToken);
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            SocketInteractionContext context = new(Client, arg);
            await interactionService.ExecuteCommandAsync(context, provider);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception occurred whilst attempting to handle interaction.");

            if (arg.Type == InteractionType.ApplicationCommand)
            {
                RestInteractionMessage msg = await arg.GetOriginalResponseAsync();
                await msg.DeleteAsync();
            }
        }
    }
}