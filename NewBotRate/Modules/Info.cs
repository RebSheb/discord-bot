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
    public class Info : ModuleBase<SocketCommandContext>
    {
        [Command("userinfo")]
        public async Task UserInfo([Summary("The (optional) user to get info for")] IGuildUser user = null)
        {

            var userInfo = user ?? (Context.Client.CurrentUser as IGuildUser);
            await ReplyAsync(
                $"{userInfo.Username}#{userInfo.Discriminator}\n" +
                $"{Format.Bold("Info")}\n" +
                $"- Nickname: {userInfo.Nickname}\n" +
                $"- Is Bot?: {((userInfo.IsBot == false) ? ":thumbsdown:\n" : ":thumbsup:\n")}" + 
                $"- Joined at: {userInfo.JoinedAt}\n" +
                $"- Permissions: {userInfo.GuildPermissions.ToString()}\n");
        }


        [Command("botinfo")]
        [Alias("about", "whoami")]
        public async Task BotInfo()
        {
            var app = await Context.Client.GetApplicationInfoAsync();

            await ReplyAsync(
                $"Rob's bot for w/e usage.\n\n" +
                $"{Format.Bold("Info")}\n" + 
                $"- Author: {app.Owner} ({app.Owner.Id})\n" +
                $"- Library: Discord.Net ({DiscordConfig.Version})" +
                $"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture} " +
                $"({RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture})\n" +
                $"- Uptime: {NewBotRate.Utils.HelperFuncs.GetUpTime()}\n\n" +

                $"{Format.Bold("Stats")}\n" +
                $"- Heap Size: {NewBotRate.Utils.HelperFuncs.GetHeapSize()}MiB\n" +
                $"- Guilds: {Context.Client.Guilds.Count}\n" +
                $"- Channels: {Context.Client.Guilds.Sum(g => g.Channels.Count)}\n" +
$"- Users: {Context.Client.Guilds.Sum(g => g.Users.Count)}\n" +
                $"- Latency: {Context.Client.Latency}ms");

        }


    }
}
