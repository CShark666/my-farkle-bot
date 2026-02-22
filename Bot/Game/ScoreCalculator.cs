namespace Bot
{
    public class ScoreCalculator
    {
        private List<IRule> _rules = new List<IRule>
        {
            new StraightRule(),
            new ThreeOfAKindRule(),
            new SingleOneRule(),
            new SingleFiveRule(),
        };
        public int Calculate(int[] dice)
        {
            var score = 0;
            foreach(var rule in _rules)
                score += rule.Check(dice);
            return score;
        }
    }
}