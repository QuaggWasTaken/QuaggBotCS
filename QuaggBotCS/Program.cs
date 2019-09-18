using DSharpPlus;
using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace QuaggBotCS
{

    public class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;
        // Program entry point
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
        {
            string jsonData = File.ReadAllText("appSettings.Debug.json");
            JObject jsonObject = JObject.Parse(jsonData);
            JToken jsonConnString = jsonObject["connString"];
            string connString = jsonConnString.ToString();
            DataHandler.URepo = new UserRepo(connString);
            DataHandler.DiscordToken = jsonObject["disKey"].ToString();
            DataHandler.MWKey = jsonObject["mwKey"].ToString();
            DataHandler.OpenWeather = jsonObject["oWeather"].ToString();
            DataHandler.Users = DataHandler.URepo.GetUsers();
            
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = DataHandler.DiscordToken,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "<>",
                CaseSensitive = false
            });
            commands.RegisterCommands<Commands>();
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }

    public static class DataHandler
    {
        public static string DiscordToken { get; set; }
        public static string MWKey { get; set; }
        public static List<User> Users { get; set; }
        public static UserRepo URepo { get; set; }
        public static string OpenWeather { get; set; }
    }
}