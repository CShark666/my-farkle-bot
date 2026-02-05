using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class PlayBtnHandler(ITelegramBotClient bot) : IButtonsHandler
    {
        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            callbackData.Decode(query.Data!, out var actionsType, out var chatId, out var userId,
            out var buttonId, out var dices, out var selectedDices);

            var msg = $"Ви обрали {dices[buttonId]} | {string.Join(',', selectedDices)}";
            var newButtons = new InlineKeyboardMarkup();

            for (int i = 0; i < dices.Length; i++)
            {
                List<int> newSelected = new List<int>(selectedDices);
                
                var emoji = "✅";

                if (!selectedDices.Contains(i))
                {
                    emoji = "🔄";
                    newSelected.Add(i);
                }

                var text =$"{dices[i]} {emoji}";

                var newCallbackData = callbackData.DiceEncodeToString(
                    InlineBtnsActionsType.DicesTesting,
                    chatId,
                    userId,
                    buttonId: i,
                    dices,
                    newSelected
                );

                var button = InlineKeyboardButton.WithCallbackData(
                    text, newCallbackData);
                if (i % 3 == 0)
                    newButtons.AddNewRow(button);
                else
                    newButtons.AddButton(button);
            }
            
            await bot.AnswerCallbackQuery(query.Id, msg);
            try
            {
                await bot.EditMessageText(chatId: callbackData.ChatId, messageId: query.Message!.Id, text: msg, replyMarkup: newButtons);
            }
            catch(ApiRequestException ex)
                when(ex.Message.Contains("message is not modified"))
            {
                
            }
        }
    }
}

//         InlineKeyboardButton[] newButtons =
//              Enumerable.Range(0, 6)
//                  .Select(i =>
//                  {
//                      // var isSelected = selectedDices.Contains(i);
//                      // var text = isSelected
//                      //     ? $"{dices[i]} ✅"
//                      //     : $"{dices[i]} 🔄";

//                      // selectedDices.Add(i);

//                      var text = string.Empty;
//                      var newSelected = selectedDices;
//                      if(selectedDices.Contains(i))
//                      {
//                          text =  $"{dices[i]} ✅";
//                      }
//                      else
//                      {
//                          text = $"{dices[i]} 🔄";
//                          newSelected.Add(i);
//                      }
//                      return InlineKeyboardButton.WithCallbackData(
//                          text,
//                          callbackData.DiceEncodeToString(
//                              InlineBtnsActionsType.DicesTesting,
//                              chatId,
//                              userId,
//                              buttonId: i,
//                              dices,
//                              newSelected
//                          )
//                      );
//                  })
//                  .ToArray();
