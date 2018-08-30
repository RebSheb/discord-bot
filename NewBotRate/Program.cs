using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NewBotRate
{
    public class BotOptions
    {
        public string Token { get; set; }
        public string CmdPrefix { get; set; }
        public string LyricAPIKey { get; set; }
    }


    public class Program
    {
        private DiscordSocketClient client;
        private IServiceProvider services;
        public static CommandService commands;
        public static readonly HttpClient httpClient = new HttpClient();
        public static string LyricsAPIKey = null;
        public static Random RND = new Random();

        private string prefix = "$";

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
        

        public async Task MainAsync()
        {
            if(!File.Exists("config.json"))
            {
                Console.WriteLine(string.Format("Error, {0} file doesn't exist! Creating now...", "config.json"));
                try
                {
                    using (StreamWriter fs = new StreamWriter("config.json"))
                    {
                        // Write the cfg json here
                        JsonSerializer serializer = new JsonSerializer();
                        BotOptions bo = new BotOptions();
                        bo.Token = "YOURTOKEN";
                        bo.CmdPrefix = "YOURPREFIX";
                        bo.LyricAPIKey = "";
                        serializer.Serialize(fs, bo);
                        fs.Close();
                    }

                    Console.WriteLine("Created config.json");
                    return; 
                } catch(Exception e)
                {
                    Console.WriteLine(string.Format("Error occured {0}", e.Message));
                    return;
                }
                
            }

            BotOptions botOptions;
            // Attempt to read a .json file for bot options
            using (StreamReader fs = new StreamReader("config.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                botOptions = (BotOptions)serializer.Deserialize(fs, typeof(BotOptions));
            }
            this.prefix = botOptions.CmdPrefix;
            NewBotRate.Program.LyricsAPIKey = botOptions.LyricAPIKey;

            // Do all the bot set up in here.
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
            });

            client.Log += Log;
            client.Ready += On_Ready;

            services = new ServiceCollection().AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .BuildServiceProvider();
            commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Verbose,
            });

            await InitializeCommands();

            await client.LoginAsync(TokenType.Bot, botOptions.Token);
            await client.StartAsync();


            await Task.Delay(-1);
        }


        private static Task Log(LogMessage msg)
        {
            var cc = Console.ForegroundColor;

            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message}");
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }


        private async Task InitializeCommands()
        {
            client.MessageReceived += HandleCommand;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task On_Ready()
        {
            await client.SetGameAsync("Oh boi", "https://www.google.com", StreamType.NotStreaming);
        }


        private async Task HandleCommand(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            if(msg == null)
            {
                return;
            }

            var ctx = new SocketCommandContext(client, msg);
            if(ctx.Message.Content == "" || ctx.Message == null)
            {
                return;
            }
            if(ctx.User.IsBot)
            {
                return;
            }

            int argPos = 0;

            if (!(msg.HasStringPrefix(prefix, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos)))
            {
                return;
            }

            var result = await commands.ExecuteAsync(ctx, argPos, services);
            if (!result.IsSuccess)
                Console.WriteLine($"{DateTime.Now} Error occured. Text: {ctx.Message.Content} : Error {result.ErrorReason}");

        }

    }
}
