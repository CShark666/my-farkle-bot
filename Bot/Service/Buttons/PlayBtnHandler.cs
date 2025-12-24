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
            // InlineKeyboardMarkup inlineKeyboardMarkup = new();
            var btnsCounter = 0;

            var msg = $"Ви обрали {callbackData.ChosenDiceValue}";
            // for (int i = 0; i < dices.Length; i++)
            // {
            //     var newCallbackData = callbackData.DiceEncodeToString(InlineBtnsActions.DicesTesting, callbackData.ChatId, callbackData.UserId, dices, dices[i]);
            //     var btnText = $"{dices[i]} 🔄";

            //     if (dices[i] == callbackData.ChosenDiceValue)
            //     {
            //         btnText = $"{dices[i]} ⏹️";
            //     }
            //     if (btnsCounter % 3 != 0)
            //     {
            //         inlineKeyboardMarkup.AddButton(
            //             InlineKeyboardButton.WithCallbackData(
            //                 btnText,
            //                 newCallbackData));
            //     }
            //     else
            //     {
            //         inlineKeyboardMarkup.AddNewRow(
            //             InlineKeyboardButton.WithCallbackData(
            //                 btnText,
            //                 newCallbackData));
            //     }
            //     btnsCounter++;
            // }
            var builder = new BuilderInlineKeyboardMarkups(callbackData);
            var inlineKeyboardMarkup = builder.BuildDiceKeyboard(dices, callbackData.ChatId, callbackData.UserId);

            await _bot.AnswerCallbackQuery(query.Id, msg);
            await _bot.EditMessageText(chatId: callbackData.ChatId, messageId: query.Message!.Id, text: msg, replyMarkup: inlineKeyboardMarkup);
        }
    }
}