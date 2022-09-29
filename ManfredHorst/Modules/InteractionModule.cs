using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Discord;
using Discord.Interactions;
using ManfredHorst.Modules.Modal;

namespace ManfredHorst.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        private IProductData? productData;

        public InteractionModule(IProductData productData)
        {
            this.productData = productData;
        }

        [SlashCommand("pricealarm", "Add an Url from Geizhals.de to get pinged when its below your desired amount.")]
        public async Task PriceAlarm()
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

            await RespondAsync(components: component.Build());
        }

        //[ComponentInteraction("menu")]
        //public async Task HandleMenuSelection(String[] inputs)
        //{
        //    await RespondAsync(inputs.FirstOrDefault());
        //}

        [ComponentInteraction("addalarm")]
        public async Task HandleAddAlarmInput()
        {
            await RespondWithModalAsync<AddAlarmModal>("add_alarm");
        }

        [ModalInteraction("add_alarm")]
        public async Task ModalResponse(AddAlarmModal modal)
        {
            AllowedMentions mentions = new AllowedMentions
            {
                AllowedTypes = AllowedMentionTypes.Users
            };

            if (String.IsNullOrWhiteSpace(modal.Alias))
            {
                await RespondAsync($"{Context.User.Mention}Missing Name", allowedMentions: mentions, ephemeral: true);
            }

            if (String.IsNullOrWhiteSpace(modal.Url))
            {
                await RespondAsync($"{Context.User.Mention} Missing url", allowedMentions: mentions, ephemeral: true);
            }

            if (!Uri.TryCreate(uriString: modal.Url, uriKind: UriKind.Absolute, result: out Uri uriResult)
            || uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                await RespondAsync($"{Context.User.Mention} Not an Url", allowedMentions: mentions, ephemeral: true);
            }

            if (!modal.Url.Contains("geizhals.de"))
            {
                await RespondAsync($"{Context.User.Mention} Nur Geizhals.de!!!!!!!!!!!!", allowedMentions: mentions, ephemeral: true);
            }

            //SavedAlarms savedAlarms = new SavedAlarms
            //{
            //    UserId = Context.User.Username,
            //    Price = modal.Price
            //};

            //savedAlarms.UrlList.Add(new Urls
            //{
            //    Url = modal.Url,
            //    Alias = modal.Alias
            //});
            User user = new User
            {
                Mention = Context.User.Mention,
                Username = Context.User.Username,
                LastSeen = DateTime.Now
            };

            Alarm alarm = new Alarm();
            alarm.Url = modal.Url;
            alarm.Alias = modal.Url;
            alarm.Price = modal.Price;
            alarm.UserId = await this.productData.GetUserByMention(user.Mention);

            await this.productData.InsertUser(user);
            await this.productData.InsertAlarm(alarm);

            await RespondAsync($"{Context.User.Mention} Url = {modal.Url} Alias = {modal.Alias}", allowedMentions: mentions, ephemeral: true);
        }
    }
}