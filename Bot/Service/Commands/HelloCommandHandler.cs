using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class HelloCommandHandler(ITelegramBotClient bot, CallbackData callbackDataCodec) : ICommandHandler
    {
        private readonly ITelegramBotClient _bot = bot;
        public async Task HandleCommandAsync(User user)
        {
            var msg = "Hiiiii!!!Helow!!!!!!!!";
            InlineKeyboardMarkup inlineKeyboardMarkup =new();
            var first_btn = callbackDataCodec.EncodeToString(InlineBtnsActions.HelloFirst, user.ChatId, user.UserId);
            var second_btn = callbackDataCodec.EncodeToString(InlineBtnsActions.HelloSecond, user.ChatId, user.UserId);


                inlineKeyboardMarkup = new([
                    [
                        InlineKeyboardButton.WithCallbackData(
                            text:"first_btn",
                            callbackData: first_btn),
                        InlineKeyboardButton.WithCallbackData(
                            text: "second_btn",
                            callbackData: second_btn),
                    ],
                ]);

            await _bot.SendMessage(user.ChatId, msg, replyMarkup: inlineKeyboardMarkup); 
        }
    }
}