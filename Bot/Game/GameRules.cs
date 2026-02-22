namespace Bot
{
    public interface IRule
    {
        int Check(int[] dice, out int[] usedDice);
    }
    public class StraightRule : IRule
    {
        public int Check(int[] dice, out int[] usedDice)
        {
            if (dice.Length != 6)
            {
                usedDice = Array.Empty<int>();
                return 0;
            }

            var isStraight = dice.Distinct().Count() == 6;
            if (isStraight)
            {
                usedDice = dice;
                return 1500;
            }

            usedDice = Array.Empty<int>();
            return 0;
        }
    }

    public class ThreeOfAKindRule : IRule
    {
        public int Check(int[] dice, out int[] usedDice)
        {
            var score = 0;
            var groups = dice.GroupBy(d => d);
            var tempList = new List<int>();

            foreach (var group in groups)
            {
                var value = group.Key;
                var count = group.Count();

                if (count < 3) continue; // skip groups less than 3

                var baseScore = value == 1 ? 1000 : value * 100;
                var multiplayer = (int)Math.Pow(2, count - 3);

                tempList.AddRange(Enumerable.Repeat(value, count).ToList());
                score += baseScore * multiplayer;
            }

            usedDice = tempList.ToArray();
            return score;
        }
    }
    public class SingleOneRule : IRule
    {
        public int Check(int[] dice, out int[] usedDice)
        {
            int once = dice.Count(d => d == 1);
            if (once == 0 || once >= 3)
            {
                usedDice = Array.Empty<int>();
                return 0;
            }

            usedDice = Enumerable.Repeat(1, once).ToArray();
            return once * 100;
        }
    }
    public class SingleFiveRule : IRule
    {
        public int Check(int[] dice, out int[] usedDice)
        {
            int five = dice.Count(d => d == 5);
            if (five == 0 || five >= 3)
            {
                usedDice = Array.Empty<int>();
                return 0;
            }

            usedDice = Enumerable.Repeat(5, five).ToArray();
            return five * 50;
        }
    }
}