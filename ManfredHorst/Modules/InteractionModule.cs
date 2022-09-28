﻿using Discord;
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

            await RespondAsync("Your current alarms:", components: component.Build(), ephemeral: true);
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
                await RespondAsync($"{Context.User.Mention}Missing url", allowedMentions: mentions, ephemeral: true);
            }

            if (!Uri.TryCreate(uriString: modal.Url, uriKind: UriKind.Absolute, result: out Uri uriResult)
            || uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                await RespondAsync($"{Context.User.Mention}Not an Url", allowedMentions: mentions, ephemeral: true);
            }

            if (!modal.Url.Contains("geizhals.de"))
            {
                await RespondAsync($"{Context.User.Mention}Nur Geizhals.de!!!!!!!!!!!!", allowedMentions: mentions, ephemeral: true);
            }

            SavedAlarms savedAlarms = new SavedAlarms
            {
                UserId = Context.User.Username,
                Price = modal.Price
            };

            savedAlarms.UrlList.Add(new Urls
            {
                Url = modal.Url,
                Alias = modal.Alias
            });

            await RespondAsync($"{Context.User.Mention} Url = {modal.Url} Alias = {modal.Alias}", allowedMentions: mentions, ephemeral: true);
        }
    }
}