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
                long chatId, int messageId,string callbackQueryId,
                BotResponse response)
            {
                try
                {
                    if (response.Text != string.Empty)
                        await bot.EditMessageText(chatId, messageId, response.Text!, replyMarkup: response.Keyboard);

                    await bot.AnswerCallbackQuery(callbackQueryId, response.QueryMessage);
                }
                catch (ApiRequestException ex)
                    when (ex.Message.Contains("message is not modified"))
                {

                }
            }
        }
    }
}