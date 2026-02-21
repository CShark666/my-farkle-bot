using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Bot
{
    public class SelectDiceBtnHandler(
        ITelegramBotClient bot,
        BotContext botContext,
        GameKeyboardFactory keyboardBuilder,
        GameCallbackDataSerializer callbackDataSerializer) : IButtonsHandler
    {
        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            // Getting game data
            callbackDataSerializer.Deserialize(query.Data!, out var gameId, out var selectedDiceId);

            var game = await botContext.Games
                .Include(g => g.CurrentTurn)
                .Include(g => g.CurrentTurn!.Player)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            // Add and save selected dice
            var turn = game!.CurrentTurn;
            turn!.AddOrRemoveDiceSelection(selectedDiceId);

            await botContext.SaveChangesAsync();

            // Creating buttons
            var keyboard = keyboardBuilder.BuildDiceSelectionKeyboard(turn);

            // Creating bot response
            var callbackQueryMsg = $"Ви обрали {turn.DiceValue[selectedDiceId]}";
            var textMessage = $"@{turn.Player.UserName}(p1) обрав :{turn.DiceValue[selectedDiceId]}";

            try
            {
                await bot.EditMessageText(
                    chatId: turn.Player.ChatId,
                    messageId: query.Message!.Id,
                    text: textMessage,
                    replyMarkup: keyboard);

                await bot.AnswerCallbackQuery(query.Id, callbackQueryMsg);
            }
            catch (ApiRequestException ex)
                when (ex.Message.Contains("message is not modified"))
            {

            }
        }
    }
}