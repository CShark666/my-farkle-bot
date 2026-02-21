namespace Bot
{
    public interface IRule
    {
        int Check(int[] dice);
    }
    public class StraightRule : IRule
    {
        public int Check(int[] dice)
        {
            if (dice.Length != 6) return 0;
            var isStraight = dice.Distinct().Count() == 6;
            return isStraight ? 1500 : 0;
        }
    }
    public class ThreeOfAKindRule : IRule
    {
        public int Check(int[] dice)
        {
            var score = 0;
            var groups = dice.GroupBy(d => d);

            foreach (var group in groups)
            {
                var value = group.Key;
                var count = group.Count();

                if (count < 3) continue; // skip groups less than 3

                var baseScore = value == 1 ? 1000 : value * 100;
                var multiplayer = (int)Math.Pow(2, count - 3);

                score += baseScore * multiplayer;
            }

            return score;
        }
    }
    public class SingleOneRule : IRule
    {
        public int Check(int[] dice)
        {
            int once = dice.Count(d => d == 1);
            if (once >= 3) return 0;
            return once * 100;
        }
    }
    public class SingleFiveRule : IRule
    {
        public int Check(int[] dice)
        {
            int five = dice.Count(d => d == 5);
            if (five >= 3) return 0;
            return five * 100;
        }
    }
}