namespace Bot
{
    public class ScoreCalculator
    {
        private readonly List<IRule> _rules =
        [
            new StraightRule(),
            new ThreeOfAKindRule(),
            new SingleOneRule(),
            new SingleFiveRule(),
        ];
        public int Calculate(int[] dice)
        {
            var total = 0;
            var remaining = dice.ToList();

            foreach (var rule in _rules)
            {
                var score = rule.Check(remaining.ToArray(), out int[] usedDice);
                total += score;

                foreach (var die in usedDice)
                    remaining.Remove(die);
            }
            if (remaining.Count > 0)
                return 0;

            return total;
        }
        public int CalculateRaw(int[] dice) =>
            _rules.Sum(rule => rule.Check(dice, out int[] usedDice));
    }
}