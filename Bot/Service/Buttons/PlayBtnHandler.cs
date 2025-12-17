using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class PlayBtnHandler(ITelegramBotClient bot, Random random) : IButtonsHandler
    {
        private readonly ITelegramBotClient _bot = bot;
        private readonly Random _random = random;
        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            callbackData.DecodeDiceFromString(query.Data!);
            int[] dices = callbackData.Dices;
            InlineKeyboardMarkup inlineKeyboardMarkup = new();

            var msg = $"2 {callbackData.Action}";
            for (int i = 0; i < dices.Length; i++)
            {
                inlineKeyboardMarkup.AddButton($"{dices[i]} 🔄");
            }
            await _bot.AnswerCallbackQuery(query.Id, msg);
            await _bot.EditMessageText(chatId: callbackData.ChatId, messageId: query.Message!.Id, text: msg, replyMarkup: inlineKeyboardMarkup);
        }
    }
}