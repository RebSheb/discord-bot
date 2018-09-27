using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace NewBotRate.Modules
{
    [Group("tag"), Alias("t")]
    public class Tag : ModuleBase
    {
        [Command("add"), Alias("a","create"), Summary("Add a new tag")]
        public async Task AddTag([Summary("The name of the tag")] string tagTitle, [Summary("The tag to add"), Remainder] string message) =>
            await ReplyAsync(Data.Data.AddTag(Context.Guild.Id, Context.User.Id, tagTitle, message).Result.ToString());

        [Command("list"), Alias("l"), Summary("List tags for this guild")]
        public async Task ListTags([Summary("The user to get tags from")] IUser CtxUser = null, [Summary("The page of the tags to get. By default gets 25 at a time.")] int pageNum = 0)
        {

            var userInfo = CtxUser ?? (Context.Message.Author as IGuildUser);
            Task<List<Database.Tag>> listTags = Data.Data.GetTags(Context.Guild.Id, CtxUser.Id, pageNum);

            if(listTags == null)
            {
                await ReplyAsync($"No tags found for {CtxUser.Username}");
                return;
            }

            EmbedBuilder embed = new EmbedBuilder();
            foreach(Database.Tag myTag in listTags.Result)
            {
                embed.AddField($"Tag: {myTag.TagName}", myTag.TagMsg);
            }

            try
            {
                embed.AddField("Pages:", $"{pageNum}/{Math.Round((float)(Data.Data.GetTagCountUser(Context.Guild.Id, CtxUser.Id).Result / 24) , MidpointRounding.AwayFromZero)}");
            }
            catch (Exception)
            {
                embed.AddField("Pages:", "No pages");
                await ReplyAsync("", false, embed.Build());
            }
            await ReplyAsync("", false, embed.Build());
        }

        [Command("delete"), Alias("del", "d"), Summary("Delete a tag you own")]
        public async Task DeleteTag([Summary("The name of the tag to delete")] string tagName) => await ReplyAsync(await Data.Data.DeleteTag(Context.Guild.Id, Context.User.Id, tagName));


        [Command("update"), Alias("u"), Summary("Update a tag you own")]
        public async Task UpdateTag([Summary("The name of the tag you want to update")] string tagName, [Summary("Additional text to add.."), Remainder] string updateMsg)
        {
            await ReplyAsync(await Data.Data.UpdateTag(Context.Guild.Id, Context.User.Id, tagName, updateMsg));
        }

        [Command("")]
        public async Task GetTag([Summary("Name of tag to get")] string tagName)
        {
            await ReplyAsync(await Data.Data.GetTag(Context.Guild.Id, tagName));
        }
    }
}
