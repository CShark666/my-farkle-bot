using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public record BotResponse(string Text, string QueryMessage);
    public class GameMessageBuilder
    {
        public BotResponse BuildFarkleResponse(Turn turn) =>
            new($"Ви програли! Та втратили {turn.TotalScore}",
                "Невдача :с");

        public BotResponse BuildStartTurnResponse(Game game) =>
            new($"🎲 @{game.Player2.UserName}(p2) ПРИЙНЯВ ВИКЛИК @{game.Player1.UserName}(p1)!!!\nГра почалась. @{game.Player1.UserName}, оберіть кубики:",
                $"Ви прийняли виклик{game.Player1.UserName}");

        public BotResponse BuildSaveAndRollResponse(Turn turn) =>
            new($"🎲 Хід @{turn.Player.UserName}\nРахунок за раунд: {turn.TotalScore}",
                $"Супер! Ви отримали {turn.CurrentScore} очок!");

        public BotResponse BuildSelectDiceResponse(Turn turn, int diceId) =>
            new($"🎲 Хід @{turn.Player.UserName}\nРахунок за раунд: {turn.TotalScore}\nРахунок комбінації: {turn.CurrentScore}",
            $"Ви обрали {turn.DiceValue[diceId]}");

        public BotResponse BuildSaveAndEndResponse(Turn turn) =>
            new( $"🎲 Хід @{turn.Player.UserName} завершив хід.\nВаш поточний рахунок: {turn.Player.TotalScore}\nЗа цей раунд ви отримали: {turn.TotalScore}\n",
            $"Хід успішно завершено!");
    }
}