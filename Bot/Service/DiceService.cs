namespace Bot
{
    public class DiceService(Random random)
    {
        private readonly Random _random = random;
        public int[] ThrowDice(int[] dice)
        {
            for (int i = 0; i < dice.Length; i++)
            {
                dice[i] = _random.Next(1, 7);
            }
            return dice;
        }
    }
}