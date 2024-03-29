﻿using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ManfredHorst.Modules.Modal;
using System.Text;

namespace ManfredHorst.Modules;

public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IProductData productData;
    private readonly ComponentBuilder component;
    private List<DatabaseAlarm> alarms;

    public InteractionModule(IProductData productData)
    {
        this.productData = productData ?? throw new ArgumentNullException(nameof(productData));
        component = new ComponentBuilder();
    }

    [SlashCommand("show-alarms", "Shows your current alarms")]
    public async Task ShowAlarms()
    {
        await RespondAsync(embeds: this.BuildEmbed().ToArray());
        Console.WriteLine($"User {Context.User.Username} {Context.User.Mention} used the command /show-alarms {DateTime.Now}");
    }

    [SlashCommand("pricealarm", "Shows Current Alarms and lets you Add or Delete an Url from Geizhals.de")]
    public async Task PriceAlarm()
    {
        await BuildComponents();
        await RespondAsync(embeds: this.BuildEmbed().ToArray(), components: component.Build(), ephemeral: true);
        Console.WriteLine($"User {Context.User.Username} {Context.User.Mention} used the command /pricealarm {DateTime.Now}");
    }

    [ComponentInteraction("addalarm")]
    public async Task HandleAddAlarmInput()
    {
        await RespondWithModalAsync<AddAlarmModal>("add_alarm_modal");
        Console.WriteLine($"User {Context.User.Username} {Context.User.Mention} used the add button {DateTime.Now}");
    }

    [ComponentInteraction("adddbgalarm")]
    public async Task HandleAddDbgAlarmInput()
    {
        await this.productData.InsertDEBUGAlarm();

        SocketMessageComponent context = Context.Interaction as SocketMessageComponent;
        await context.UpdateAsync(x =>
        {
            x.Content = $"Added new entry in database!";
            x.Embeds = this.BuildEmbed().ToArray();
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
            Console.WriteLine($"{Context.User.Mention} Missing Name {DateTime.Now}");
        }

        if (String.IsNullOrWhiteSpace(modal.Url))
        {
            await RespondAsync($"{Context.User.Mention} Missing url", allowedMentions: mentions, ephemeral: true);
            Console.WriteLine($"{Context.User.Mention} Missing Url {DateTime.Now}");
        }

        if (!Uri.TryCreate(uriString: modal.Url, uriKind: UriKind.Absolute, result: out Uri uriResult)
        || uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
        {
            await RespondAsync($"{Context.User.Mention} Not an Url", allowedMentions: mentions, ephemeral: true);
            Console.WriteLine($"{Context.User.Mention} Not an Url {DateTime.Now}");
        }

        if (!modal.Url.Contains("geizhals.de"))
        {
            await RespondAsync($"{Context.User.Mention} only Geizhals.de!", allowedMentions: mentions, ephemeral: true);
            Console.WriteLine($"{Context.User.Mention} Not Geizhals.de {DateTime.Now}");
        }

        if (modal.Url.Contains("redir."))
        {
            await RespondAsync($"{Context.User.Mention} Invalid Url", allowedMentions: mentions, ephemeral: true);
            Console.WriteLine($"{Context.User.Mention} Redirect in url {DateTime.Now}");
        }

        User user = new User
        {
            Mention = Context.User.Mention,
            Username = Context.User.Username,
            EntryDate = DateTime.Now
        };

        await this.productData.InsertUser(user);

        DatabaseAlarm alarm = new DatabaseAlarm
        {
            Url = modal.Url,
            Alias = modal.Alias,
            Price = modal.Price,
            UserId = await this.productData.GetUserByMention(user.Mention),
            EntryDate = DateTime.Now
        };

        await this.productData.InsertAlarm(alarm);

        await DeferAsync();
        await BuildComponents();
        SocketModal interaction = Context.Interaction as SocketModal;
        await interaction.ModifyOriginalResponseAsync(x =>
        {
            x.Content = $"New alarm added!";
            x.Embeds = this.BuildEmbed().ToArray();
            x.Components = component.Build();
        });
        Console.WriteLine($"User {user.Username}-{user.Mention} added Alarm {modal.Alias} {modal.Url} {DateTime.Now}");
    }

    [ComponentInteraction("deletealarm")]
    public async Task HandleDeleteAlarmInput()
    {
        await RespondWithModalAsync<DeleteAlarmModal>("delete_alarm_modal");
        Console.WriteLine($"User {Context.User.Username} {Context.User.Mention} used the delete button {DateTime.Now}");
    }

    [ModalInteraction("delete_alarm_modal")]
    public async Task ModalResponse(DeleteAlarmModal modal)
    {
        try
        {
            await DeferAsync();
            await this.productData.DeleteAlarm(modal.Alias, Context.User.Mention);
            await BuildComponents();

            SocketModal interaction = Context.Interaction as SocketModal;
            await interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"Deleted Alarm!";
                x.Embeds = this.BuildEmbed().ToArray();
                x.Components = component.Build();
            });
            Console.WriteLine($"Alarm {modal.Alias} deleted! {DateTime.Now}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await RespondAsync($"{Context.User.Mention} Error: could not find the name!");
            Console.WriteLine($"{ex.Message} {DateTime.Now}");
        }
    }

    public async Task BuildComponents()
    {
        ButtonBuilder addAlarmButton = new ButtonBuilder()
        {
            Label = "Add",
            CustomId = "addalarm",
            Style = ButtonStyle.Primary,
        };

        component.WithButton(addAlarmButton);

        alarms = await this.productData.GetAlarmsByMention(Context.User.Mention);

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
    }

    public List<Embed> BuildEmbed()
    {
        alarms = this.productData.GetAlarmsByMention(Context.User.Mention).GetAwaiter().GetResult();
        List<EmbedBuilder> embed = new();
        StringBuilder stringbuilderAliasUrl = new StringBuilder();
        StringBuilder stringbuilderPrice = new StringBuilder();
        StringBuilder stringbuilderIndex = new StringBuilder();
        List<Embed> emb = new();

        if (alarms.Any() && alarms != null)
        {
            Int64 index = 0;
            foreach (DatabaseAlarm alarm in alarms)
            {
                stringbuilderAliasUrl.Append($"[{alarm.Alias}]({alarm.Url});");
                stringbuilderPrice.Append($"{alarm.Price}€;");
                stringbuilderIndex.Append($"{++index};");
            }

            String[] aliasUrl = stringbuilderAliasUrl.ToString().TrimEnd(';').Split(';');
            String[] price = stringbuilderPrice.ToString().TrimEnd(';').Split(';');
            String[] stringIndex = stringbuilderIndex.ToString().TrimEnd(';').Split(';');

            int thirdindex = 0;
            for (int i = 0; i <= Math.Ceiling((Double)aliasUrl.Count() / 5); i++)
            {
                StringBuilder fiveAliasUrl = new StringBuilder();
                StringBuilder fivePrice = new StringBuilder();
                StringBuilder fiveStringIndex = new StringBuilder();

                for (int y = 0; y < 5; y++)
                {
                    if (thirdindex != aliasUrl.Count())
                    {
                        fiveAliasUrl.Append($"{aliasUrl[thirdindex]}\n");
                        fivePrice.Append($"{price[thirdindex]}\n");
                        fiveStringIndex.Append($"{stringIndex[thirdindex]}\n");
                        thirdindex++;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (fiveStringIndex.Length != 0)
                {
                    embed.Add(new EmbedBuilder()
                            .AddField("Nr.", $"{fiveStringIndex}", true)
                            .AddField("Name", $"{fiveAliasUrl}", true)
                            .AddField("Price", $"{fivePrice}", true)
                            .WithAuthor(Context.User)
                            .WithColor(Color.Green)
                            .WithCurrentTimestamp());
                }
                else
                {
                    continue;
                }
            }
        }
        else
        {
            return new List<Embed>()
            {
                    new EmbedBuilder()
                        .WithAuthor(Context.Client.CurrentUser)
                        .WithColor(Color.Red)
                        .WithTitle($"Alarms: 0")
                        .WithCurrentTimestamp().Build()
            };
        }
        foreach (EmbedBuilder item in embed)
        {
            emb.Add(item.Build());
        }
        return emb;
    }
}