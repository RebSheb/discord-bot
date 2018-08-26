using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace NewBotRate.Preconditions
{
    public class BotController : PreconditionAttribute
    {
        public static List<ulong> BotControllers = new List<ulong>
        {
            274625792787611649
        };


        public override async Task<PreconditionResult> CheckPermissions(ICommandContext Ctx, CommandInfo Command, IServiceProvider Service)
        {
            return BotControllers.Contains(Ctx.User.Id) ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("You must be within owner whitelist to use this command...");
        }
    }
}
