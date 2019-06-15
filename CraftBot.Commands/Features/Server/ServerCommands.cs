﻿using CraftBot.Commands.Features.Server;
using CraftBot.Helper;
using CraftBot.Profiles;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands : BaseCommandModule
    {
        [RequireGuild]
        [Group("server")]
        public partial class ServerCommands : BaseCommandModule
        {
            [Command("emojis")]
            [Aliases("emoji")]
            [RequireGuild]
            public async Task Emojis(CommandContext ctx)
            {
                string response = "";
                foreach (DiscordEmoji emoji in ctx.Guild.Emojis.Values)
                {
                    response += $"{(emoji.IsAnimated ? "A" : "N")} {emoji.Name} ({emoji.Id})\n";
                }
                await ctx.RespondAsync(response);
            }

            [Command("features")]
            [RequireGuild]
            public async Task Features(CommandContext ctx)
            {
                await ctx.RespondAsync(embed: new DiscordEmbedBuilder()
                   .WithAuthor($"{ctx.Guild.Name} / Features", null, ctx.Guild.IconUrl)
                   .WithDescription(ctx.Guild.Features.Count == 0 ? "This server has no features" : string.Join("\n", ctx.Guild.Features))
                   );
            }

            [GroupCommand]
            [Command("info")]
            [RequireGuild]
            public async Task Info(CommandContext context)
            {
                MaterialEmbedBuilder builder = new MaterialEmbedBuilder(context.Client).WithTitle($"{context.Guild.Name} / {context.GetStringAsync("GeneralTerms_Information")}", context.Guild.IconUrl);

                builder.AddSection(null, context.GetStringAsync("GeneralTerms_General"),
                    new MaterialEmbedListTile(
                        context.Guild.IsLarge ? "account-group" : "account-multiple",
                        context.GetStringAsync("Server_Info_Members_Title"),
                        string.Format(context.GetStringAsync("Server_Info_Members_Value"), context.Guild.MemberCount.ToString())),

                    new MaterialEmbedListTile(
                        "calendar-plus",
                        context.GetStringAsync("Server_Info_CreatedOn_Title"),
                        string.Format(context.GetStringAsync("Server_Info_CreatedOn_Value"), context.Guild.CreationTimestamp.ToString()))
                    );

                builder.AddSection(null, context.GetStringAsync("GeneralTerms_BotRelated"),
                    new MaterialEmbedListTile(
                        "earth",
                        context.GetStringAsync("Server_Info_Region"),
                        context.Guild.VoiceRegion.Name),

                    new MaterialEmbedListTile(
                        context.Guild.VoiceRegion.IsVIP,
                        "VIP")
                    );

                //string logFile = $"data\\guild\\{context.Guild.Id}.log.json";
                //int loggedMessages = context.Guild.Logging().Value && File.Exists(logFile) ? JsonConvert.DeserializeObject<List<DiscordMessage>>(File.ReadAllText(logFile)).Count() : 0;

                builder.AddSection(null, context.GetStringAsync("GeneralTerms_BotRelated"),
                    new MaterialEmbedListTile(
                        await context.Guild.IsShitpostingEnabledAsync(),
                        context.GetStringAsync("Server_Info_Shitposting")
                    )
                );

                _ = await context.RespondAsync(embed: builder.Build());
            }
        }
    }
}