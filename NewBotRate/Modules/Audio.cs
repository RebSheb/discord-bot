using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using Discord;
using Discord.Commands;

namespace NewBotRate.Modules
{
    [Group("audio")]
    public class Audio : ModuleBase<SocketCommandContext>
    {
        
        [Command("volumeup"), Summary("Earrape a file")]
        public async Task IncreaseVolume([Summary("The amount to boost the volume by")] float increase)
        {
            await ReplyAsync("Not implemented yet..");

            return;
        }
        
    }
}
