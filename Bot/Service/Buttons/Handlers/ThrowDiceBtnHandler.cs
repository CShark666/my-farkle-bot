using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Bot
{
    public class ThrowDiceBtnHandler(
        ITelegramBotClient bot,
        DiceKeyboardFactory builder,
        DiceCallbackDataSerializer diceCallbackDataSerializer) : IButtonsHandler
    {
        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            diceCallbackDataSerializer.Deserialize(query.Data!,out var actionsType, out var chatId, out var userId,
            out var buttonId, out var dices, out var selectedDices);

            var msg = $"Ви обрали {dices[buttonId]} | {string.Join(',', selectedDices)}";
            var newButtons = builder.BuildDiceSelectionKeyboard(dices, chatId, userId, selectedDices);

            try
            {
                await bot.AnswerCallbackQuery(query.Id, msg);
                await bot.EditMessageText(chatId: callbackData.ChatId, messageId: query.Message!.Id, text: msg, replyMarkup: newButtons);
            }
            catch (ApiRequestException ex)
                when (ex.Message.Contains("message is not modified"))
            {

            }
        }
    }
}