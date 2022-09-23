using Discord.Interactions;

namespace ManfredHorst.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("pricealarm", "Add an Url from Geizhals.de to get pinged when its below your desired amount.")]
        public async Task PriceAlarm()
        {
            await RespondAsync("LAK ICH KANN NOCH NICHTS11!1!!!!!!!!!!!!!!");
        }
    }
}
