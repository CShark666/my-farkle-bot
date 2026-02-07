using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Bot
{
    public class RerollButtonHandler(ITelegramBotClient bot, DiceKeyboardFactory builder, DiceService diceService) : IButtonsHandler
    {
        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            callbackData.Deserialize(query.Data!);

            int[] dice = new int[6];
            dice = diceService.ThrowDice(dice);
            var selectedDices = new List<int>();

            var msg = "Ви кинули кубики:";
            var newButtons = builder.BuildDiceSelectionKeyboard(dice, callbackData.ChatId, callbackData.UserId, selectedDices);

            try
            {
                await bot.AnswerCallbackQuery(query.Id, msg);
                await bot.EditMessageText(callbackData.ChatId, messageId: query.Message!.Id, text: msg, replyMarkup: newButtons);
            }
            catch (ApiRequestException ex)
                when (ex.Message.Contains("message is not modified"))
            {

            }
        }
    }
}
