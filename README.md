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

## Setup

### 1. Clone the Repository

git clone https://github.com/YOUR_USERNAME/DiscordPriceBot.git
cd DiscordPriceBot


### 2. Install Dependencies

Ensure you have the necessary .NET packages:

dotnet add package Discord.Net --version 3.15.2
dotnet add package Newtonsoft.Json


### 3. Configure the Bot

Replace the placeholders in the `Program.cs` file with your actual Discord bot token and channel ID:

private const ulong ChannelId = YOUR_CHANNEL_ID; // Replace with your channel ID
private const string Token = "YOUR_BOT_TOKEN"; // Replace with your bot token


### 4. Build and Run the Bot

Build and run the project:

dotnet run


## Contributing

Feel free to submit issues or pull requests if you have any improvements or bug fixes.

## License

This project is licensed under the MIT License.

---

Replace `YOUR_USERNAME` with your GitHub username, and `YOUR_CHANNEL_ID` and `YOUR_BOT_TOKEN` with your actual channel ID and bot token.

By following these steps, anyone should be able to clone the repository, set up the bot, and run it on their own Discord server.
