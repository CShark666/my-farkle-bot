using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SurrenderBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameButtonsBuilder keyboardBuilder,
            GameCallbackDataSerializer callbackDataSerializer,
            GameMessageBuilder messageBuilder,
            ScoreCalculator scoreCalculator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.Surrender;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();

            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var game = await botContext.Games
                        .Include(g => g.CurrentTurn)
                        .Include(g => g.Player1)
                        .Include(g => g.Player2)
                        .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game!.Player1.UserId == query.From.Id || game.Player2.UserId == query.From.Id)
            {
                game.Status = GameStatus.Finished;
                game.Winner = game.GetOpponentWithId(query.From.Id);

                await botContext.SaveChangesAsync();

                response = messageBuilder.BuildSurrenderResponse(game);
            }
            else
            {
                response = messageBuilder.BuildWrongTurnResponse();
            }

            
            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}