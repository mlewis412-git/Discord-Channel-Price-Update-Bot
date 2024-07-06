# DiscordPriceBot

DiscordPriceBot is a simple Discord bot that updates the name of a specified voice channel with the current price of Pepecoin (PEP) fetched from an external API.

## Features

- Fetches the latest PEP price from `https://pepeblocks.com/ext/getcurrentprice`.
- Updates a specified voice channel's name with the formatted PEP price.
- Ensures the price is updated every 5 minutes.

## Prerequisites

- .NET SDK (version 5.0 or later)
- A Discord bot token
- Access to a Discord server where you have permission to manage channels

## Required Permissions

When inviting your bot to a Discord server, ensure it has the following permissions:

- View Channels
- Manage Channels
- Connect
- Speak

To generate an invite link with these permissions, use the OAuth2 URL Generator in the Discord Developer Portal with the following settings:

- **Scopes**: `bot`
- **Bot Permissions**:
  - `View Channels`
  - `Manage Channels`
  - `Connect`
  - `Speak`

### Example OAuth2 URL

```
https://discord.com/oauth2/authorize?client_id=YOUR_CLIENT_ID&scope=bot&permissions=3148800
```

Replace `YOUR_CLIENT_ID` with your bot's client ID.

## Setup

### 1. Clone the Repository

```sh
git clone https://github.com/YOUR_USERNAME/DiscordPriceBot.git
cd DiscordPriceBot
```

### 2. Install Dependencies

Ensure you have the necessary .NET packages:

```sh
dotnet add package Discord.Net --version 3.15.2
dotnet add package Newtonsoft.Json
```

### 3. Configure the Bot

Replace the placeholders in the `Program.cs` file with your actual Discord bot token and channel ID:

```csharp
private const ulong ChannelId = YOUR_CHANNEL_ID; // Replace with your channel ID
private const string Token = "YOUR_BOT_TOKEN"; // Replace with your bot token
```

### 4. Build and Run the Bot

Build and run the project:

```sh
dotnet run
```

## Code Overview

The bot connects to Discord, fetches the latest PEP price from an external API, and updates the specified voice channel's name with the formatted price.

### `Program.cs`

```csharp
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
        private const ulong ChannelId = YOUR_CHANNEL_ID; // Replace with your channel ID
        private const string Token = "YOUR_BOT_TOKEN"; // Replace with your bot token
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
                        await channel.ModifyAsync(prop => prop.Name = $"💲PEP Price: ${formattedPrice}");
                        _lastPrice = price;
                        Console.WriteLine($"Channel name updated to: 💲PEP Price: ${formattedPrice}");
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
```

## Contributing

Feel free to submit issues or pull requests if you have any improvements or bug fixes.

## License

This project is licensed under the MIT License.

---

Replace `YOUR_USERNAME` with your GitHub username, and `YOUR_CHANNEL_ID` and `YOUR_BOT_TOKEN` with your actual channel ID and bot token.

By following these steps, anyone should be able to clone the repository, set up the bot, and run it on their own Discord server.
