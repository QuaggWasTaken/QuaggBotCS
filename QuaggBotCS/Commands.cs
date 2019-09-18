using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace QuaggBotCS
{
    public class Commands
    {
        [Command("Register"), Description("Registers the user to the bot")]
        public async Task Register(CommandContext ctx)
        {
            Console.WriteLine($"{ctx.Command.Name} command recieved from user {ctx.User.Username}#{ctx.User.Discriminator} in server {ctx.Guild.Name}");
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(CommandHandler.RegisterUser(ctx));
            await DataHandler.URepo.UpdateUsers();
        }

        [Command("Hi"), Description("Hello!")]
        public async Task Hi(CommandContext ctx)
        {
            Console.WriteLine($"{ctx.Command.Name} command recieved from user {ctx.User.Username}#{ctx.User.Discriminator} in server {ctx.Guild.Name}");
            await ctx.RespondAsync($"👋 Hi, {ctx.User.Mention}!");
        }

        [Command("Echo"), Description("Echoes the message sent")]
        public async Task Echo(CommandContext ctx, [Description("Text to echo"), RemainingText] string Echo)
        {
            Console.WriteLine($"{ctx.Command.Name} command recieved from user {ctx.User.Username}#{ctx.User.Discriminator} in server {ctx.Guild.Name}");
            await ctx.RespondAsync(Echo);
        }

        [Command("Roll"), Description("Rolls dice given the format <>roll XdY")]
        public async Task Roll(CommandContext ctx, [Description("XdY, X = amount, Y = sides")]string Roll)
        {
            Console.WriteLine($"{ctx.Command.Name} command recieved from user {ctx.User.Username}#{ctx.User.Discriminator} in server {ctx.Guild.Name}");
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(CommandHandler.Roll(Roll));
        }

        [Command("Awoo"), Description("Awoo!")]
        public async Task Awoo(CommandContext ctx)
        {
            Console.WriteLine($"{ctx.Command.Name} command recieved from user {ctx.User.Username}#{ctx.User.Discriminator} in server {ctx.Guild.Name}");
            await ctx.TriggerTypingAsync();
            var embed = new DiscordEmbedBuilder
            {
                Title = "Awoo!",
                ImageUrl = "https://i.imgur.com/tZx2wVV.jpg"
            };
            await ctx.RespondAsync(embed: embed);
        }

        [Command("Define"), Description("Defines a word using Merriam Webster's Dictionary API")]
        public async Task Define(CommandContext ctx, [Description("Word to be searched for")] string Word, [RemainingText, Description("Ignores words after the first")] string ignore)
        {
            Console.WriteLine($"{ctx.Command.Name} command recieved from user {ctx.User.Username}#{ctx.User.Discriminator} in server {ctx.Guild.Name}");
            Console.WriteLine($"Creating WebRequest For {Word} using Key {DataHandler.MWKey}");
            WebRequest request = WebRequest.Create($"https://dictionaryapi.com/api/v1/references/collegiate/xml/{Word}?key={DataHandler.MWKey}");
            WebResponse response = request.GetResponse();
            string output;
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                output = CommandHandler.Define(responseFromServer);
            }
            // Close the response.  
            response.Close();
            await ctx.RespondAsync(output);
        }

        [Command("Weather"), Description("Get your local weather just from a zip code (and country code if you're outside the US)")]
        public async Task CurrentWeather(CommandContext ctx, [Description("Your Zip Code")] int Zip, [Description("Your Country Code (UK, AU, NZ, etc)")] string Country = "US")
        {
            await CommandHandler.CurrentWeather(ctx, Zip, Country);
        }

        [Command("Wikipedia"), Aliases("Wiki"), Description("Links you to the Wikipedia page for whatever you search")]
        public async Task Wikipedia(CommandContext ctx, [Description("Article to search for")] string Article)
        {
            await ctx.RespondAsync($"http://en.wikipedia.org/wiki/{Article}");
        }
        [Group("Memes", CanInvokeWithoutSubcommand = true), Description("Fresh memes here! Come getcha memes!")]
        public class Memes
        {
            public async Task ExecuteGroupAsync(CommandContext ctx, [Description("Subreddit to pull memes from, optional")] string input = null, [RemainingText] string ignore = null)
            {
                Console.WriteLine($"{ctx.Command.Name} command recieved from user {ctx.User.Username}#{ctx.User.Discriminator} in server {ctx.Guild.Name}");
                await CommandHandler.MemeGenerator(ctx, input);
            }

            [Command("Chuck"), Description("Chuck Norris facts, 100% true every time")]
            public async Task Chuck(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync(CommandHandler.ChuckNorris(ctx));
            }
        } 
    }

    public partial class Meme
    {
        [JsonProperty("postLink")]
        public Uri PostLink { get; set; }

        [JsonProperty("subreddit")]
        public string Subreddit { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Meme
    {
        public static Meme FromJson(string json) => JsonConvert.DeserializeObject<Meme>(json, Converter.Settings);
    }

    public partial class Weather
    {
        [JsonProperty("coord")]
        public Coord Coord { get; set; }

        [JsonProperty("weather")]
        public List<WeatherElement> WeatherWeather { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("main")]
        public MainWeather Main { get; set; }

        [JsonProperty("visibility")]
        public long Visibility { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("clouds")]
        public Clouds Clouds { get; set; }

        [JsonProperty("dt")]
        public long Dt { get; set; }

        [JsonProperty("sys")]
        public Sys Sys { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cod")]
        public long Cod { get; set; }
    }

    public partial class Clouds
    {
        [JsonProperty("all")]
        public long All { get; set; }
    }

    public partial class Coord
    {
        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }
    }

    public partial class MainWeather
    {
        [JsonProperty("temp")]
        public double Temp { get; set; }

        [JsonProperty("pressure")]
        public long Pressure { get; set; }

        [JsonProperty("humidity")]
        public long Humidity { get; set; }

        [JsonProperty("temp_min")]
        public double TempMin { get; set; }

        [JsonProperty("temp_max")]
        public double TempMax { get; set; }
    }

    public partial class Sys
    {
        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("message")]
        public double Message { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("sunrise")]
        public long Sunrise { get; set; }

        [JsonProperty("sunset")]
        public long Sunset { get; set; }
    }

    public partial class WeatherElement
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("main")]
        public string Main { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public partial class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public long Deg { get; set; }

        [JsonProperty("gust")]
        public double Gust { get; set; }
    }

    public partial class Weather
    {
        public static Weather FromJson(string json) => JsonConvert.DeserializeObject<Weather>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Weather self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string ToJson(this Meme self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    public static class CommandHandler
    {
        public static string RegisterUser(CommandContext ctx)
        {
            ulong snow = ctx.User.Id;
            bool userFound = false;
            foreach (User user in DataHandler.Users)
            {
                if (user.Snowflake == snow)
                {
                    userFound = true;
                }
            }
            if (!userFound)
            {
                DataHandler.Users.Add(new User
                {
                    Snowflake = snow,
                    Names = new List<Nickname>() {
                        new Nickname {
                            Name = (ctx.Member.Nickname != null) ? ctx.Member.Nickname : ctx.User.Username,
                            Guild = new Server
                            {
                                ServerID = ctx.Guild.Id,
                                ServerName = ctx.Guild.Name,
                                Update = true,
                                New = true
                            },
                            Update = true,
                            New = true
                        }
                    },
                    Discriminator = ctx.Member.Discriminator,
                    Update = true,
                    New = true
                });
                return "New user verified! Thank you for using QuaggBot!";
            }
            else
            {
                for (int i = 0; i < DataHandler.Users.Count; i++)
                {
                    if (DataHandler.Users[i].Snowflake == snow)
                    {
                        for (int j = 0; j < DataHandler.Users[i].Names.Count; j++)
                        {
                            if (DataHandler.Users[i].Names[j].Guild.ServerID == ctx.Guild.Id)
                            {
                                DataHandler.Users[i].Names[j].Name = (ctx.Member.Nickname != null) ? ctx.Member.Nickname : ctx.User.Username;
                                DataHandler.Users[i].Names[j].Update = true;
                                return "User already exists! Name updated, thank you!";
                            }
                            else if (j == DataHandler.Users[i].Names.Count - 1)
                            {
                                DataHandler.Users[i].Names.Add(new Nickname
                                {
                                    Name = (ctx.Member.Nickname != "") ? ctx.Member.Nickname : ctx.User.Username,
                                    Guild = new Server
                                    {
                                        ServerID = ctx.Guild.Id,
                                        ServerName = ctx.Guild.Name,
                                        Update = true,
                                        New = true
                                    },
                                    Update = true,
                                    New = true
                                });
                                return "User registered in seperate server! New name and server registered, thank you!";
                            }
                        }
                    }
                }
            }
            return "Error, this should not be seen.";
        }

        public static async Task MemeGenerator(CommandContext ctx, string subreddit = null)
        {
            await ctx.TriggerTypingAsync();
            string url = "https://meme-api.herokuapp.com/gimme";
            if (subreddit != null)
            {
                url += "/" + subreddit;
            }
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Meme output;
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = @reader.ReadToEnd();
                output = Meme.FromJson(responseFromServer);
            }
            // Close the response.  
            response.Close();
            var embed = new DiscordEmbedBuilder
            {
                Title = output.Title,
                ImageUrl = output.Url.AbsoluteUri,
            };
            embed.WithFooter(output.PostLink.AbsoluteUri);
            await ctx.RespondAsync(embed: embed);
        }

        public static string ChuckNorris(CommandContext ctx)
        {
            string url = "https://api.chucknorris.io/jokes/random";
            HttpClient client = new HttpClient();
            Task<string> response = client.GetStringAsync(url);
            JObject jsonObject = JObject.Parse(response.Result);
            JToken jsonValue = jsonObject["value"];
            //return JObject.Parse(client.GetStringAsync(url).Result)["value"].ToString();
            return jsonValue.ToString();
        }

        public static string Roll(string input)
        {
            //Define vars
            string numDiceStr;
            string diceSizeStr;
            int rollTotal = 0;
            string output = "";
            //Parse input for vars
            numDiceStr = input.Substring(0, input.IndexOf("d"));
            diceSizeStr = input.Substring(input.IndexOf("d") + 1);

            if (!Int32.TryParse(numDiceStr, out int numDice))
            {
                return "Sorry, that wasnt a valid input, use the format <>roll xdy, with x being the amount of dice and y being the number of sides on the dice";
            }
            if (!Int32.TryParse(diceSizeStr, out int diceSize))
            {
                return "Sorry, that wasnt a valid input, use the format <>roll xdy, with x being the amount of dice and y being the number of sides on the dice";
            }

            for (int i = 0; i < numDice; i++)
            {
                var rand = new Random();
                int currRoll = (rand.Next(1, diceSize) % diceSize) + 1;
                rollTotal += currRoll;

                if (i == 0)
                {
                    output += currRoll.ToString();
                }
                else
                {
                    output += (", " + currRoll.ToString());
                }
            }
            output += "\nYou rolled a total of: " + rollTotal.ToString();
            return output;
        }

        public static string Define(string input)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input);
            string output;
            try
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode("/entry_list/entry/def");
                output = node.SelectSingleNode("dt").InnerText.Replace(":", "");
            }
            catch (Exception except)
            {
                Console.WriteLine(except);
                return "Sorry, that's not a word that Merriam-Webster knows, please try again";
                throw;
            }
            return output;
        }

        public static async Task CurrentWeather(CommandContext ctx, int Zip, string Country)
        {
            await ctx.TriggerTypingAsync();
            string url = $"https://api.openweathermap.org/data/2.5/weather?zip={Zip},{Country}&appid={DataHandler.OpenWeather}";
            HttpClient client = new HttpClient();
            Task<string> response = client.GetStringAsync(url);
            string jsonData = response.Result;
            Weather weather = Weather.FromJson(jsonData);
            await ctx.RespondAsync($"Temperature: {weather.Main.Temp-273.15}ºC\nWeather: {weather.WeatherWeather[0].Description}\nHumidity: {weather.Main.Humidity}%");
        }
    }
}
