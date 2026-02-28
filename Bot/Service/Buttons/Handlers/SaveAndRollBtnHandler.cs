using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SaveAndRollBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        GameButtonsBuilder keyboardBuilder,
        GameCallbackDataSerializer callbackDataSerializer,
        ScoreCalculator scoreCalculator,
        GameMessageBuilder messageBuilder) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SaveAndRoll;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();

            if (!callbackData.MatchesId(query.From.Id))
            {
                response = messageBuilder.BuildWrongTurnResponse();

            }
            else
            {
                // Getting game data
                callbackDataSerializer.Deserialize(query.Data!, out var gameId);
                var game = await botContext.Games
                            .Include(g => g.CurrentTurn)
                            .Include(g => g.CurrentTurn!.Player)
                            .FirstOrDefaultAsync(g => g.Id == gameId);


                var turn = game!.CurrentTurn;

                // Save score and roll remaining dice 
                turn!.SaveAndRoll();
                await botContext.SaveChangesAsync();

                if (scoreCalculator.IsFarkle(turn.DiceValue))
                {
                    response = messageBuilder.BuildFarkleResponse(game.CurrentTurn!);
                    keyboard = InlineKeyboardMarkup.Empty();
                }
                else
                {
                    keyboard = keyboardBuilder.BuildTurnKeyboard(turn);
                    response = messageBuilder.BuildSaveAndRollResponse(game.CurrentTurn!);
                }
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, response.Text,
                    keyboard, query.Id, response.QueryMessage);
        }
    }
}