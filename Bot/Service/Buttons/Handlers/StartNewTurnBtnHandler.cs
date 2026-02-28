using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class StartNewTurnBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameButtonsBuilder keyboardBuilder,
            GameCallbackDataSerializer callbackDataSerializer,
            GameMessageBuilder messageBuilder,
            ScoreCalculator scoreCalculator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.StartTurn;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();

            if (callbackData.MatchesId(query.From.Id))
            {
                response = messageBuilder.BuildWrongTurnResponse();
            }
            else
            {
                callbackDataSerializer.Deserialize(query.Data!, out var gameId);
                var game = await botContext.Games
                            .Include(g => g.CurrentTurn)
                            .Include(g => g.Player1)
                            .Include(g => g.Player2)
                            .FirstOrDefaultAsync(g => g.Id == gameId);

                var player = game!.GetOpponent();
                game.StartTurn(player);
                game.CurrentTurn!.RollDice();

                await botContext.SaveChangesAsync();


                if (scoreCalculator.IsFarkle(game.CurrentTurn!.DiceValue))
                {
                    response = messageBuilder.BuildFarkleResponse(game.CurrentTurn);
                    keyboard = InlineKeyboardMarkup.Empty();
                }
                else
                {
                    keyboard = keyboardBuilder.BuildTurnKeyboard(game.CurrentTurn);
                    response = messageBuilder.BuildStartTurnResponse(game);
                }

                response = messageBuilder.BuildSaveAndRollResponse(game.CurrentTurn);
                keyboard = keyboardBuilder.BuildTurnKeyboard(game.CurrentTurn);
            }
            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}