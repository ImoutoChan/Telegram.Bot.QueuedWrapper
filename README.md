# Telegram.Bot.QueuedWrapper
This library represents a wrapper around [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) . All calls are sent into their respective queues and awaited until allowed time comes. This way you can avoid hitting telegram flood limits. Base principle is the same as described [here](https://github.com/python-telegram-bot/python-telegram-bot/wiki/Avoiding-flood-limits).  

[![NuGet](https://img.shields.io/nuget/v/Telegram.Bot.QueuedWrapper.svg?style=flat-square)](https://www.nuget.org/packages/Telegram.Bot.QueuedWrapper/)

## Usage

The library fully implements ITelegramBotClient interface. The only thing you need to do is to replace TelegramBotClient with QueuedTelegramBotClient.

```C#
static async Task TestApiAsync()
{
    var botClient = new QueuedTelegramBotClient("your API access Token");
    var me = await botClient.GetMeAsync();
    System.Console.WriteLine($"Hello! My name is {me.FirstName}");
}
```

## Installation

Install as [NuGet package](https://www.nuget.org/packages/Telegram.Bot.QueuedWrapper/):

```powershell
Install-Package Telegram.Bot.QueuedWrapper -Version 1.1.1
```
