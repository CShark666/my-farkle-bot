using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SaveAndEndBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameButtonsBuilder keyboardBuilder,
            GameCallbackDataSerializer callbackDataSerializer,
            GameMessageBuilder messageBuilder,
            GameRepository repository) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SaveAndEnd;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);

            if (!callbackData.MatchesId(query.From.Id))
            {
                response = messageBuilder.BuildWrongTurnResponse();
            }
            else if(await repository.IsGameFinishedAsync(gameId))
            {
                response = messageBuilder.BuildGameIsFinished();
            }
            else
            {
                var game = await repository.GetGameAsync(gameId);

                game!.FinishTurn();

                if (game.IsPlayerWins())
                {
                    game.FinishGame();
                    response = messageBuilder.BuildFinishGameResponse(game);
                }
                else
                {
                    keyboard = keyboardBuilder.BuildEndTurnKeyboard(game);
                    response = messageBuilder.BuildSaveAndEndResponse(game);
                }

                await botContext.SaveChangesAsync();
            }

            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}