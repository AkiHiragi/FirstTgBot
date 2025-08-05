using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FirstTgBot.Services;

public class BotService(TelegramBotClient bot, YandereService yandereService)
{
    public async Task HandleMessageAsync(Message msg, UpdateType type)
    {
        var (command, args) = ParseCommand(msg.Text);

        switch (command)
        {
            case "/help":
                await SendHelpAsync(msg.Chat);
                break;

            case "/image":
            case "/random":
            case "/safe":
                await SendImageAsync(msg.Chat, args, "s");
                break;

            case "/questionable":
                await SendImageAsync(msg.Chat, args, "q");
                break;
        }
    }

    public async Task HandleCallbackAsync(CallbackQuery query)
    {
        await bot.AnswerCallbackQuery(query.Id);
        await SendImageAsync(query.Message!.Chat, query.Data!, "s");
    }

    private async Task SendHelpAsync(Chat chat)
    {
        await bot.SendMessage(chat,
            "🖼️ <b>Anime Image Bot</b>\n\n" +
            "Команды:\n" +
            "• <code>/image [теги]</code> - изображение (safe)\n" +
            "• <code>/safe [теги]</code> - безопасный контент\n" +
            "• <code>/questionable [теги]</code> - сомнительный контент\n" +
            "• <code>/random</code> - случайное изображение\n" +
            "• <code>/help</code> - эта справка\n\n" +
            "Популярные теги:",
            ParseMode.Html,
            replyMarkup: new InlineKeyboardButton[][]
            {
                ["1girl", "landscape"],
                ["school_uniform", "scenery"],
                ["original", "touhou"]
            });
    }

    private async Task SendImageAsync(Chat chat, string tags, string rating)
    {
        try
        {
            var post = await yandereService.GetRandomImageAsync(tags, rating);
            if (post != null)
            {
                var ratingText = rating switch
                {
                    "s" => "Safe",
                    "q" => "Questionable",
                    "e" => "Explicit",
                    _ => "Unknown"
                };

                await bot.SendPhoto(chat, post.file_url,
                    $"<b>Rating:</b> {ratingText}\n<b>Tags:</b> {post.tags}\n<a href=\"https://yande.re/post/show/{post.id}\">Source</a>",
                    ParseMode.Html);
            }
            else
            {
                await bot.SendMessage(chat, "Изображения не найдены 😔");
            }
        }
        catch (Exception ex)
        {
            await bot.SendMessage(chat, "Ошибка при получении изображения");
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static (string command, string args) ParseCommand(string? text)
    {
        if (string.IsNullOrEmpty(text) || !text.StartsWith('/'))
            return ("", "");

        var parts = text.Split(' ', 2);
        return (parts[0], parts.Length > 1 ? parts[1] : "");
    }
}