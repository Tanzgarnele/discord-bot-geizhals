using Discord;
using Discord.Interactions;
using ManfredHorst.Modules.Modal;
using ManfredHorst.UserData;

namespace ManfredHorst.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("pricealarm", "Add an Url from Geizhals.de to get pinged when its below your desired amount.")]
        public async Task PriceAlarm()
        {
            await RespondAsync("LAK ICH KANN NOCH NICHTS11!1!!!!!!!!!!!!!!");
        }

        [SlashCommand("geizhals", "Teste Button für Geizhals.")]
        public async Task Geizhals()
        {
            ButtonBuilder button = new ButtonBuilder()
            {
                Label = "Add Alarm",
                CustomId = "addalarm",
                Style = ButtonStyle.Primary,
            };

            //var menu = new SelectMenuBuilder()
            //{
            //    CustomId = "menu",
            //    Placeholder = "Show options"
            //};

            //menu.AddOption("Add Alarm", "addalarm");
            //menu.AddOption("Delete Alarm", "deletealarm");

            ComponentBuilder component = new ComponentBuilder();
            component.WithButton(button);
            //component.WithSelectMenu(menu);

            await RespondAsync("Current alarms:", components: component.Build(), ephemeral: true);
        }

        //[ComponentInteraction("menu")]
        //public async Task HandleMenuSelection(String[] inputs)
        //{
        //    await RespondAsync(inputs.FirstOrDefault());
        //}

        [ComponentInteraction("addalarm")]
        public async Task HandleAddAlarmInput()
        {
            await RespondWithModalAsync<GeizhalsAddUrlModal>("add_alarm");
        }

        [ModalInteraction("add_alarm")]
        public async Task ModalResponse(GeizhalsAddUrlModal modal)
        {
            AllowedMentions mentions = new();
            mentions.AllowedTypes = AllowedMentionTypes.Users;

            if (String.IsNullOrWhiteSpace(modal.Url))
            {
                await RespondAsync("Missing url", allowedMentions: mentions, ephemeral: true);
            }
            
            if (String.IsNullOrWhiteSpace(modal.Alias))
            {
                await RespondAsync("Missing Name", allowedMentions: mentions, ephemeral: true);
            }

            SavedAlarms savedAlarms = new SavedAlarms();
            savedAlarms.UserId = Context.User.Username;
            savedAlarms.UrlList.Add(new Urls
            {
                Url = modal.Url,
                Alias = modal.Alias
            });

            await RespondAsync($"{Context.User.Mention} Url = {modal.Url} Alias = {modal.Alias}", allowedMentions: mentions, ephemeral: true);
        }
    }
}