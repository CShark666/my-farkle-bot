using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class HelloButtonHandler(ITelegramBotClient bot) : IButtonsHandler
    {
        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            var msg = $"You chose {callbackData.Action}";
            await bot.AnswerCallbackQuery(query.Id, msg);
            await bot.EditMessageText(chatId: callbackData.ChatId, messageId: query.Message!.Id, text: msg);
        }
    }
}