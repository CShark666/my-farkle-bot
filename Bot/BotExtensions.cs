using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public static class BotExtensions
    {
        extension(TelegramBotClient bot)
        {
            public async Task SafeEditAndAnswerAsync(
                long chatId, int messageId, string text,
                InlineKeyboardMarkup keyboard,
                string callbackQueryId, string queryMsg)
            {
                try
                {
                    if (text != string.Empty)
                        await bot.EditMessageText(chatId, messageId, text, replyMarkup: keyboard);

                    await bot.AnswerCallbackQuery(callbackQueryId, queryMsg);
                }
                catch (ApiRequestException ex)
                    when (ex.Message.Contains("message is not modified"))
                {

                }
            }
        }
    }
}