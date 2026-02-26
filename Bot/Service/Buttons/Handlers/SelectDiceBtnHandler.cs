using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Bot
{
    public class SelectDiceBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        GameButtonsBuilder keyboardBuilder,
        GameCallbackDataSerializer callbackDataSerializer,
        ScoreCalculator scoreCalculator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SelectDice;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            // Getting game data
            callbackDataSerializer.Deserialize(query.Data!, out var gameId, out var selectedDiceId);

            var game = await botContext.Games
                        .Include(g => g.CurrentTurn)
                        .Include(g => g.CurrentTurn!.Player)
                        .FirstOrDefaultAsync(g => g.Id == gameId);

            // Add selected dice
            var turn = game!.CurrentTurn;
            turn!.AddOrRemoveDiceSelection(selectedDiceId);

            // Calculate score selected dice
            var selectedDice = turn.SelectedDice.Select(sd => turn.DiceValue[sd]).ToArray();
            turn.CurrentScore = scoreCalculator.Calculate(selectedDice);

            await botContext.SaveChangesAsync();

            // Creating buttons
            var diceKeyboard = keyboardBuilder.BuildDiceSelectionButtons(turn);
            var saveAndRollButton = keyboardBuilder.BuildSaveAndRollButton(turn);
            var saveAndEndButton = keyboardBuilder.BuildSaveAndEndButton(turn);

            diceKeyboard.AddNewRow(saveAndRollButton);
            diceKeyboard.AddNewRow(saveAndEndButton);

            // Creating bot response
            var queryMsg = $"Ви обрали {turn.DiceValue[selectedDiceId]}";
            var textMsg = $"🎲 Хід @{turn.Player.UserName} (p1)\nРахунок за раунд: {turn.TotalScore}\nРахунок вибраних кубиків: {turn.CurrentScore}";

            try
            {
                await bot.EditMessageText(
                    chatId: turn.Player.ChatId,
                    messageId: query.Message!.Id,
                    text: textMsg,
                    replyMarkup: diceKeyboard);

                await bot.AnswerCallbackQuery(query.Id, queryMsg);
            }
            catch (ApiRequestException ex)
                when (ex.Message.Contains("message is not modified"))
            {

            }
        }
    }
}