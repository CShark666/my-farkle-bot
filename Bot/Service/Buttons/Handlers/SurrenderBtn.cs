using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SurrenderBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameCallbackDataSerializer callbackDataSerializer,
            GameMessageBuilder messageBuilder,
            GameRepository repository) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.Surrender;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();

            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var validUser = await repository.IsGameUserValidAsync(gameId, query.From.Id);

            if (!validUser)
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

                game!.FinishGame();
                game.Winner = game.GetOpponentWithId(query.From.Id);

                await botContext.SaveChangesAsync();


                response = messageBuilder.BuildSurrenderResponse(game);
            }


            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}