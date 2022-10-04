using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ManfredHorst.Modules.Modal;
using System.Text;

namespace ManfredHorst.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        private IProductData? productData;
        private ComponentBuilder component = new ComponentBuilder();
        private List<Alarm> alarms;

        public InteractionModule(IProductData productData)
        {
            this.productData = productData;
        }

        [SlashCommand("show-alarms", "Shows your current alarms")]
        public async Task ShowAlarms()
        {
            alarms = await this.productData.GetAlarmsByMention(Context.User.Mention);
            await RespondAsync(embed: this.BuildEmbed().Build());
        }

        [SlashCommand("pricealarm", "Shows Current Alarms and lets you Add or Delete an Url from Geizhals.de")]
        public async Task PriceAlarm()
        {
            alarms = await this.productData.GetAlarmsByMention(Context.User.Mention);
            await BuildAlarmEmbed(await this.productData.GetAlarmsByMention(Context.User.Mention));
        }

        [ComponentInteraction("addalarm")]
        public async Task HandleAddAlarmInput()
        {
            await RespondWithModalAsync<AddAlarmModal>("add_alarm_modal");
        }

        [ComponentInteraction("adddbgalarm")]
        public async Task HandleAddDbgAlarmInput()
        {
            await this.productData.InsertDEBUGAlarm();
            alarms = await this.productData.GetAlarmsByMention(Context.User.Mention);

            var context = Context.Interaction as SocketMessageComponent;
            await context.UpdateAsync(x =>
            {
                x.Content = $"Added new entry in database!";
                x.Embed = this.BuildEmbed().Build();
            });
        }

        [ModalInteraction("add_alarm_modal")]
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

            User user = new User
            {
                Mention = Context.User.Mention,
                Username = Context.User.Username,
                LastSeen = DateTime.Now
            };

            Alarm alarm = new Alarm
            {
                Url = modal.Url,
                Alias = modal.Alias,
                Price = modal.Price,
                UserId = await this.productData.GetUserByMention(user.Mention)
            };

            await this.productData.InsertUser(user);
            await this.productData.InsertAlarm(alarm);

            //await RespondAsync($"added!", allowedMentions: mentions, ephemeral: true);
            alarms = await this.productData.GetAlarmsByMention(Context.User.Mention);
            await DeferAsync();
            SocketModal? interaction = Context.Interaction as SocketModal;
            await interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"New alarm added!";
                x.Embed = this.BuildEmbed().Build();
            });
        }

        [ComponentInteraction("deletealarm")]
        public async Task HandleDeleteAlarmInput()
        {
            await RespondWithModalAsync<DeleteAlarmModal>("delete_alarm_modal");
        }

        [ModalInteraction("delete_alarm_modal")]
        public async Task ModalResponse(DeleteAlarmModal modal)
        {
            try
            {
                await this.productData.DeleteAlarm(modal.Alias, Context.User.Mention);
                alarms = await this.productData.GetAlarmsByMention(Context.User.Mention);
                await DeferAsync();
                SocketModal? interaction = Context.Interaction as SocketModal;
                await interaction.ModifyOriginalResponseAsync(x =>
                {
                    x.Content = $"Added new entry in database!";
                    x.Embed = this.BuildEmbed().Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await RespondAsync($"{Context.User.Mention} Error: could not find the name!");
            }
        }

        public async Task BuildAlarmEmbed(List<Alarm> alarms)
        {
            ButtonBuilder addAlarmButton = new ButtonBuilder()
            {
                Label = "Add",
                CustomId = "addalarm",
                Style = ButtonStyle.Primary,
            };

            //ButtonBuilder addDebugAddButton = new ButtonBuilder()
            //{
            //    Label = "Add Dbg",
            //    CustomId = "adddbgalarm",
            //    Style = ButtonStyle.Secondary,
            //};

            //component.WithButton(addDebugAddButton);
            component.WithButton(addAlarmButton);

            if (alarms.Any() && alarms != null)
            {
                ButtonBuilder deleteAlarmButton = new ButtonBuilder()
                {
                    Label = "Delete",
                    CustomId = "deletealarm",
                    Style = ButtonStyle.Danger,
                };

                component.WithButton(deleteAlarmButton);
            }

            await RespondAsync(embed: this.BuildEmbed().Build(), components: component.Build(), ephemeral: true);
        }

        public EmbedBuilder BuildEmbed()
        {
            EmbedBuilder embed = new EmbedBuilder();
            StringBuilder stringbuilderAliasUrl = new StringBuilder();
            StringBuilder stringbuilderPrice = new StringBuilder();
            StringBuilder stringbuilderIndex = new StringBuilder();

            if (alarms.Any() && alarms != null)
            {
                Int64 index = 0;
                foreach (Alarm alarm in alarms)
                {
                    stringbuilderAliasUrl.Append($"[{alarm.Alias}]({alarm.Url})\n");
                    stringbuilderPrice.Append($"{alarm.Price}€\n");
                    stringbuilderIndex.Append($"{++index}\n");
                }

                embed.AddField("Nr.", $"{stringbuilderIndex}", true)
                    .AddField("Name", $"{stringbuilderAliasUrl}", true)
                    .AddField("Price", $"{stringbuilderPrice}", true)
                    .WithAuthor(Context.Client.CurrentUser)
                    .WithColor(Color.Orange)
                    .WithTitle($"Alarms: {index}")
                .WithCurrentTimestamp();
            }
            else
            {
                embed.Title = "Alarms: 0";
            }
            return embed;
        }
    }
}