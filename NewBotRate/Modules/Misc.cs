using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace NewBotRate.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("help"), Summary("Helps... What do you think?")]
        public async Task Help([Summary("The command to search for")] string cmd = null)
        {
            if (cmd == null)
            {
                EmbedBuilder embed = new EmbedBuilder();

                embed.WithColor(163, 214, 227);
                embed.WithTitle("List of commands:");

                string commands = "";

                foreach (CommandInfo commandService in Program.commands.Commands)
                {
                    if (!(await commandService.CheckPreconditionsAsync(Context)).IsSuccess)
                    {
                        continue;
                    }


                    commands += $"{commandService.Name}\n";
                }

                embed.WithDescription(commands);

                await ReplyAsync(string.Empty, false, embed: embed.Build());
            }

            else if (Program.commands.Commands.ToList().Find(e => e.Name == cmd) != null)
            {
                EmbedBuilder embed = new EmbedBuilder();

                CommandInfo reqCmd = Program.commands.Commands.ToList().Find(e => e.Name == cmd);

                if (!(await reqCmd.CheckPreconditionsAsync(Context)).IsSuccess)
                {
                    await ReplyAsync((await reqCmd.CheckPreconditionsAsync(Context)).ErrorReason);
                    return;
                }

                embed.WithTitle(cmd);
                embed.WithDescription("");

                if (reqCmd.Parameters.Count > 0)
                {
                    foreach (ParameterInfo parameter in reqCmd.Parameters)
                    {
                        embed.Title += $" [{parameter.Name}{(parameter.IsOptional ? $" = {Convert.ToString(parameter.DefaultValue)}" : string.Empty)}] ";
                    }
                }

                if (reqCmd.Aliases.Count > 0)
                {
                    embed.Description += "\nAliases: ";
                    embed.Description += reqCmd.Aliases.Aggregate(embed.Description, (current, alias) => current + $"[ {alias} ]");
                }

                if (!string.IsNullOrWhiteSpace(reqCmd.Summary))
                {
                    embed.Description += $"\n{reqCmd.Summary}\n";
                }

                if (reqCmd.Parameters.Count > 0)
                {
                    embed.Description += reqCmd.Parameters.Where(parameter => !string.IsNullOrWhiteSpace(parameter.Summary)).Aggregate(embed.Description, (current, parameter) => current + $"\n{parameter.Name}: {parameter.Summary}");
                }

                await ReplyAsync(string.Empty, false, embed.Build());

            }


        }

        [Command("geninvite"), Summary("Get an invite to a specified server"), RequireOwner]
        public async Task GenerateInvite([Summary("GuildID to get invite for")] ulong GuildID)
        {
            await ReplyAsync($"Generating invite for {GuildID}\n");
            if (Context.Client.Guilds.Where(x => x.Id == GuildID).Count() < 1)
            {
                await ReplyAsync($":x: I am not in this guild ({GuildID})");
                return;
            }
            SocketGuild Guild = Context.Client.Guilds.Where(x => x.Id == GuildID).FirstOrDefault();
            try
            {
                var invites = await Guild.GetInvitesAsync();
                if (invites.Count() < 1)
                {
                    await Guild.TextChannels.First().CreateInviteAsync();
                }
                invites = null;
                invites = await Guild.GetInvitesAsync();
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithAuthor($"Invites for Guild {Guild.Name}:", Guild.IconUrl);
                embed.WithColor(40, 200, 150);
                foreach (var Current in invites)
                {
                    embed.AddInlineField("Invite:", $"{Current.Url}");
                }

                await ReplyAsync("", false, embed.Build());
            } catch (Exception e)
            {
                await ReplyAsync($"Error occured in generating an invite for {GuildID}\n{e.Message}");
            }
        }

        [Command("listguilds"), Summary("List all the guilds the bot is connected to"), RequireOwner]
        public async Task ListGuilds()
        {
            string guildsMsg = "";

            foreach (IGuild iGuild in Context.Client.Guilds)
            {
                if ((guildsMsg + iGuild.Name + ":" + iGuild.Id).Length > 2000)
                {
                    await ReplyAsync(guildsMsg);
                    guildsMsg = "";
                }
                guildsMsg = guildsMsg + iGuild.Name + ":" + iGuild.Id + "\n";
            }

            await ReplyAsync(guildsMsg);
        }

        [Command("complain"), Alias("feedback"), Summary("Give feedback to Rob about bot")]
        public async Task Feedback([Remainder] string Message)
        {
            if (Context.User.IsBot)
            {
                await ReplyAsync(":x: Bot's cannot leave feedback...");
                return;
            }

            Data.Data.AddFeedback(Context.Guild.Id, Context.User.Id, Message);
            await Context.Channel.SendMessageAsync(":thumbsup: Feedback sent!");
            await Context.Message.DeleteAsync();
            return;
        }

        [Command("getfeedback"), Alias("gfb", "getfeedback"), Summary("Gets feedback from Database"), RequireOwner]
        public async Task GetFeedback(int MaxGet = 10, bool IsRead = false)
        {
            List<Database.Feedback> fdbacks = null;
            fdbacks = Data.Data.GetFeedbacks(MaxGet, IsRead);
            if (fdbacks == null)
            {
                await ReplyAsync("No feedbacks to look at...");
                return;
            }

            string msg = "";
            foreach (Database.Feedback fback in fdbacks)
            {
                if ((msg + fback.FeedbackID + ": " + fback.GuildID + "-" + "<@" + fback.UserID + ">" + ": " + fback.Message + "\n").Length > 2000)
                {
                    await ReplyAsync(msg);
                }
                msg = msg + fback.FeedbackID + ": " + fback.GuildID + "-" + "<@" + fback.UserID + ">" + ": " + fback.Message + "\n";
            }
            await ReplyAsync(msg);
        }

        [Command("deletefeedback"), Alias("dfb", "delfb", "deletefeedback"), Summary("Deletes feedback from database"), RequireOwner]
        public async Task DeleteFeedback(int FBackID)
        {
            if (FBackID < 0)
            {
                await ReplyAsync("The value entered must be bigger than 0...");
                return;
            }

            bool Res = await Data.Data.DeleteFeedback(FBackID);
            if (Res)
            {
                await ReplyAsync($"Successfully deleted value {FBackID}!");
                return;
            }
            else
            {
                await ReplyAsync($"The entry for {FBackID} doesn't exist...");
                return;
            }

        }

        [Command("setreadfeedback"), Alias("sfb"), Summary("Set a feedback as read"), RequireOwner]
        public async Task SetFeedbackRead(int FBackID, bool readVal)
        {
            if (FBackID < 0)
            {
                await ReplyAsync("The value entered must be bigger than 0...");
                return;
            }

            Data.Data.EditFeedbackRead(FBackID, readVal);
            await ReplyAsync($"Successfully modified {FBackID}");
            return;
        }

        [Command("additiveinverse"), Alias("addinv"), Summary("Perform additive inverse on a list of numbers, space delimited")]
        public async Task AdditiveInverse([Summary("Ints to do magic on"), Remainder] string integers)
        {
            string modString = integers;

            string replyString = string.Empty;

            foreach(string value in modString.Split(' '))
            {
                int currentInt = 0;
                if(!int.TryParse(value, out currentInt))
                {
                    replyString += value + " ";
                    continue;
                }

                if (currentInt > 0)
                    replyString += (-currentInt).ToString() + " ";
                else if (currentInt <= 0)
                    replyString += Math.Abs(currentInt).ToString() + " ";
                else
                    replyString += "What? ";
            }

            await ReplyAsync(replyString);
            return;
        }

        [Command("exponent"), Alias("pow"), Summary("Find the exponent of two numbers")]
        public async Task Exponent([Summary("Integer base")] int number,
            [Summary("Number to exponent by")] int pow) => await ReplyAsync($"{Math.Pow(number, pow)}");

    }
}
