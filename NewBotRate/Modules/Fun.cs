﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;


using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HtmlAgilityPack;
using ScrapySharp;
using System.Globalization;

namespace NewBotRate.Modules
{
    #region SongStuff
    public class Artist
    {
        public string name { get; set; }
    }

    public class Lang
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Track
    {
        public string name { get; set; }
        public string text { get; set; }
        public Lang lang { get; set; }
    }

    public class Copyright
    {
        public string notice { get; set; }
        public string artist { get; set; }
        public string text { get; set; }
    }

    public class Result
    {
        public Artist artist { get; set; }
        public Track track { get; set; }
        public Copyright copyright { get; set; }
        public float probability { get; set; }
        public float similarity { get; set; }
    }

    public class RootSongObject
    {
        public Result result { get; set; }
        private string nerror = "<blank>";
        public string error
        {
            get { return this.nerror; }
            set { this.nerror = value; }
        }
    }
    #endregion

    public class Fun : ModuleBase<SocketCommandContext>
    {
        public static readonly List<string> vowels = new List<string> { "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };


        [Command("say"), Alias("echo", "repeat"), Summary("Echos a message")]
        public async Task Say([Remainder, Summary("Text to echo")] string echo)
        {
            await ReplyAsync(echo);
        }

        [Command("mc"), Alias("achget"), Summary("Generate elite MC achievement")]
        public async Task MCAchievment([Remainder, Summary("Text of achievment")] string text)
        {
            using (var response = await NewBotRate.Program.httpClient.GetStreamAsync($"https://mcgen.herokuapp.com/a.php?i=1&h=Achievement-{Context.User.Username}&t={text}"))
            {
                await Context.Channel.SendFileAsync(response, "achievement.png");
            }
        }

        [Command("lyrics"), Alias("getwords"), Summary("Gets lyrics of a song...")]
        public async Task LyricsGet([Summary("The artist name <REQUIRED>")] string artistName = "logic",
            [Summary("The artists' song <REQUIRED>"), Remainder] string artistSong = "gangrelated")
        {
            RootSongObject RSO = null;
            try
            {
                if(Program.LyricsAPIKey == null || Program.LyricsAPIKey == "")
                {
                    await ReplyAsync("There is no lyrics api key configured. Look @ config.json...");
                    return;
                }
                string response = await NewBotRate.Program.httpClient.GetStringAsync($"https://orion.apiseeds.com/api/music/lyric/{Uri.EscapeDataString(artistName)}/{Uri.EscapeDataString(artistSong)}?apikey={Program.LyricsAPIKey}");
                RSO = JsonConvert.DeserializeObject<RootSongObject>(response);

                if(RSO.error.Contains("Lyric no found"))
                {
                    await ReplyAsync("Lyrics for {artistName} - {artistSong} not found... :(");
                }
                
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle(Format.Bold(RSO.result.artist.name + " - " + RSO.result.track.name));
                embed.WithColor(255, 140, 80);

                IEnumerable<string> lyricsmsg = NewBotRate.Utils.HelperFuncs.ChunksUpto(RSO.result.track.text, 1000);

                foreach(string ss in lyricsmsg)
                {
                    embed.AddInlineField("-", ss);
                }
                
                await ReplyAsync("", false, embed.Build());

            } catch(Exception)
            {
                await ReplyAsync($"Lyrics for {artistName} - {artistSong} not found... :(");
                return;
            }
        }

        [Command("emojify"), Alias("emoji"), Summary("Turns text into emojis...")]
        public async Task Emojify([Summary("Text to turn to emoji"), Remainder] string text)
        {
            await Context.Message.DeleteAsync();
            string emojiText = "";
            foreach (char c in text.ToCharArray())
            {
                switch (char.ToLower(c))
                {
                    case 'a':
                        emojiText += "🇦 ";
                        break;
                    case 'b':
                        emojiText += "🇧 ";
                        break;
                    case 'c':
                        emojiText += "🇨 ";
                        break;
                    case 'd':
                        emojiText += "🇩 ";
                        break;
                    case 'e':
                        emojiText += "🇪 ";
                        break;
                    case 'f':
                        emojiText += "🇫 ";
                        break;
                    case 'g':
                        emojiText += "🇬 ";
                        break;
                    case 'h':
                        emojiText += "🇭 ";
                        break;
                    case 'i':
                        emojiText += "🇮 ";
                        break;
                    case 'j':
                        emojiText += "🇯 ";
                        break;
                    case 'k':
                        emojiText += "🇰 ";
                        break;
                    case 'l':
                        emojiText += "🇱 ";
                        break;
                    case 'm':
                        emojiText += "🇲 ";
                        break;
                    case 'n':
                        emojiText += "🇳 ";
                        break;
                    case 'o':
                        emojiText += "🇴 ";
                        break;
                    case 'p':
                        emojiText += "🇵 ";
                        break;
                    case 'q':
                        emojiText += "🇶 ";
                        break;
                    case 'r':
                        emojiText += "🇷 ";
                        break;
                    case 's':
                        emojiText += "🇸 ";
                        break;
                    case 't':
                        emojiText += "🇹 ";
                        break;
                    case 'u':
                        emojiText += "🇺 ";
                        break;
                    case 'v':
                        emojiText += "🇻 ";
                        break;
                    case 'w':
                        emojiText += "🇼 ";
                        break;
                    case 'x':
                        emojiText += "🇽 ";
                        break;
                    case 'y':
                        emojiText += "🇾 ";
                        break;
                    case 'z':
                        emojiText += "🇿 ";
                        break;
                    case '1':
                        emojiText += ":one: ";
                        break;
                    case '2':
                        emojiText += ":two: ";
                        break;
                    case '3':
                        emojiText += ":three: ";
                        break;
                    case '4':
                        emojiText += ":four: ";
                        break;
                    case '5':
                        emojiText += ":five: ";
                        break;
                    case '6':
                        emojiText += ":six: ";
                        break;
                    case '7':
                        emojiText += ":seven: ";
                        break;
                    case '8':
                        emojiText += ":eight: ";
                        break;
                    case '9':
                        emojiText += ":nine: ";
                        break;
                    case '0':
                        emojiText += ":zero: ";
                        break;
                    case '#':
                        emojiText += "#⃣ ";
                        break;
                    case '*':
                        emojiText += "*⃣ ";
                        break;
                    case '+':
                        emojiText += "➕ ";
                        break;
                    case '-':
                        emojiText += "➖ ";
                        break;
                    case '$':
                        emojiText += "💲 ";
                        break;
                    case '!':
                        emojiText += "❗ ";
                        break;
                    case '?':
                        emojiText += "❓ ";
                        break;
                    default:
                        emojiText += c + " ";
                        break;
                }
            }
            await ReplyAsync(emojiText);
            return;
        }

        [Command("reversestring"), Alias("strrev", "rev"), Summary("Reverse a string")]
        public async Task ReverseString([Summary("String to reverse"), Remainder] string text)
        {
            try
            {
                string output = string.Empty;
                for(int index = text.Length; index > 0; index--)
                {
                    output += text[index-1];
                }

                await ReplyAsync(output);
                return;
            } catch(IndexOutOfRangeException e)
            {
                await ReplyAsync(e.Message);
                return;
            }

        }

        [Command("piglatin"), Alias("pygl"), Summary("Perform pig latin.")]
        public async Task PigLatinString([Summary("String to piglatin"), Remainder] string text)
        {
            string output = string.Empty;
            string[] wordAppendChoide = new string[] { "way", "yay" };

            var strArray = text.Split(" ");

            foreach(string word in strArray)
            {
                if ((!vowels.Contains(word[0].ToString())) && (vowels.Contains(word[1].ToString()))) // Begins with a consonant, second letter is a vowel
                {
                    string modWord = word;
                    // Move 1st[0] to end and append "ay"
                    modWord += modWord[0] + "ay";
                    modWord.Remove(0);
                    output += modWord;
                }
                else if((!vowels.Contains(word[0].ToString())) && !(vowels.Contains(word[1].ToString()))) // Begins with two consonants
                {
                    // Move 1st two characters [0] & [1] to the end and append "ay"
                    string modWord = word;
                    modWord += modWord[0].ToString() + modWord[1].ToString();
                    modWord = modWord.Remove(0, 1).Remove(0, 1);
                    output += modWord;
                }
                else if (vowels.Contains(word[0].ToString())) // Starts with a vowel
                {

                    // Add either way or ay on end. RandomChoice it.
                    output += ((Program.RND.Next(0, 1) == 1) ? word + wordAppendChoide[0] : word + wordAppendChoide[1]);
                }
            }
            if (output.Length > 2000)
            {
                await ReplyAsync("String too long to post :(");
                return;
            }

            await ReplyAsync(output);
        }

        
    }
}
