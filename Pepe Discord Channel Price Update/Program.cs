using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private const ulong ChannelId = xxxxxxxxxxxxxxx; // Replace with your channel ID
        private const string Token = "xxxxxxxxxxxxxx"; // Replace with your bot token
        private decimal _lastPrice;

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages // Adjust based on your needs
            };

            _client = new DiscordSocketClient(config);
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            foreach (var guild in _client.Guilds)
            {
                Console.WriteLine($"Connected to guild: {guild.Name} (ID: {guild.Id})");

                var channel = guild.GetVoiceChannel(ChannelId);
                if (channel != null)
                {
                    Console.WriteLine($"Channel found: {channel.Name} (ID: {channel.Id})");
                    _ = UpdatePriceAsync(channel); // Start the update task without awaiting it
                    return Task.CompletedTask; // Immediately return to avoid blocking
                }
                else
                {
                    Console.WriteLine($"Channel ID {ChannelId} not found in guild: {guild.Name}");
                }
            }

            Console.WriteLine("Channel not found. Please verify the channel ID and bot permissions.");
            return Task.CompletedTask; // Immediately return to avoid blocking
        }

        private async Task UpdatePriceAsync(SocketVoiceChannel channel)
        {
            using var httpClient = new HttpClient();
            while (true)
            {
                try
                {
                    var response = await httpClient.GetStringAsync("https://pepeblocks.com/ext/getcurrentprice");
                    var priceData = JObject.Parse(response);
                    var price = priceData["last_price_usd"].ToObject<decimal>();

                    if (_lastPrice != price)
                    {
                        // Format the price using periods
                        string formattedPrice = price.ToString("0.00000000");
                        await channel.ModifyAsync(prop => prop.Name = $"💲PEP: ${formattedPrice}");
                        _lastPrice = price;
                        Console.WriteLine($"Channel name updated to: 💲PEP: ${formattedPrice}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(5)); // Ensure at least 5 minutes between updates
            }
        }
    }
}
