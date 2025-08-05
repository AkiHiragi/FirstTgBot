using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using System.Net.Http;
using FirstTgBot.Services;
using Microsoft.Extensions.Configuration;

using var cts = new CancellationTokenSource();
using var httpClient = new HttpClient();
var configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();

var yandereService = new YandereService(httpClient);
var botToken = configuration["BotToken"] ?? throw new Exception("BotToken not found in configuration");
var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var botService = new BotService(bot, yandereService);
var me = await bot.GetMe();

bot.OnError += OnError;
bot.OnMessage += (msg, type) => botService.HandleMessageAsync(msg, type);
bot.OnUpdate += (update) => update.CallbackQuery != null
    ? botService.HandleCallbackAsync(update.CallbackQuery)
    : Task.CompletedTask;

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel();
return;

async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception);
}