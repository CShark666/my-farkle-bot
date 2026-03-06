using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public record BotResponse(string? Text = null, string? QueryMessage = null, InlineKeyboardMarkup? Keyboard = null);
    public class GameResponseFactory(GameButtonsFactory keyboardFactory)
    {
        public BotResponse BuildWrongTurnResponse() =>
            new(string.Empty,
                "Це не ваш хід!");

        public BotResponse BuildWrongPlayerResponse() =>
                new(string.Empty,
                "Ви не можете грати проти себе!");

        public BotResponse BuildFarkleResponse(Game game) =>
            new($"💥 FARKLE!\n" +
                $"@{game.CurrentTurn!.Player.UserName}, ви втратили {game.CurrentTurn.TotalScore} очок цього раунду.\n" +
                $"🎲 Хід переходить до @{game.GetOpponent().UserName}.",
                "Невдала комбінація — очки згоріли.",
                keyboardFactory.BuildEndTurnKeyboard(game));

        public BotResponse BuildStartTurnResponse(Game game) =>
            new($"⚔ @{game.Player2.UserName} -прийняв виклик- @{game.Player1.UserName}!\n"+
                $"🎲 Гру розпочато.\n" +
                $"Перший хід за @{game.CurrentTurn!.Player.UserName}. Оберіть кубики.",
                $"Ви прийняли виклик{game.CurrentTurn!.Player.UserName}",
                keyboardFactory.BuildTurnKeyboard(game.CurrentTurn));

        public BotResponse BuildSaveAndRollResponse(Turn turn) =>
            new($"🎲 Хід @{turn.Player.UserName} (очки за гру: {turn.Player.TotalScore})\n" +
                $"📊 Очки раунду: {turn.TotalScore}\n" +
                $"Бажаєте кинути ще раз чи зберегти результат?",
                $"Супер! +{turn.CurrentScore} очок.",
                keyboardFactory.BuildTurnKeyboard(turn));

        public BotResponse BuildSelectDiceResponse(Turn turn, int diceId) =>
            new($"🎯 @{turn.Player.UserName} (очки за гру: {turn.Player.TotalScore})\n" +
                $"📊 Поточний рахунок раунду: {turn.TotalScore}\n"+
                $"💰 Очки комбінації: {turn.CurrentScore}\n",
                $"Ви обрали {turn.DiceValue[diceId]}",
                keyboardFactory.BuildTurnKeyboard(turn));

        public BotResponse BuildSaveAndEndResponse(Game game) =>
            new($"🛑 @{game.CurrentTurn!.Player.UserName} завершив хід.\n" +
                $"🏆 Загальний рахунок: {game.CurrentTurn.Player.TotalScore}\n"+
                $"💰 За раунд: {game.CurrentTurn.TotalScore}\n\n"+
                $"🎲 Наступний хід за @{game.GetOpponent().UserName}. Продовжуємо?",
                "Хід успішно завершено.",
                keyboardFactory.BuildEndTurnKeyboard(game));

        public BotResponse BuildSurrenderResponse(Game game) =>
            new($"🏆 Переможець: @{game.Winner!.UserName}!\n"+
                "🏳 Суперник здався.",
                "Ви здалися. Гру завершено.");

        public BotResponse BuildFinishGameResponse(Game game) =>
            new($"🏁 Гру завершено!\n" +
                $"🏆 Переможець: @{game.Winner!.UserName} - {game.Winner.TotalScore} очок\n"+
                $"🥈 @{game.GetOpponent().UserName} - {game.GetOpponent().TotalScore} очок\n\n" +
                $"Дякуємо за гру!",
                $"Перемога @{game.Winner!.UserName}!");

        public BotResponse BuildGameIsFinished() =>
            new($"🏁Ця гра вже завершено!",
                "🏁Ця гра вже завершено!");

        public BotResponse BuildInvalidUserGamesStatus() =>
            new(string.Empty,
                "❌ Хтось з гравців має незавершену гру!");

        public BotResponse BuildSelectedDiceIsNull() =>
            new(string.Empty,
                "🎲 Ви маєте обрати кубик 🎲");
    }
}