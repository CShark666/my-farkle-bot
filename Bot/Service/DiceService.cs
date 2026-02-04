namespace Bot
{
    public class DiceService(Random random)
    {
        private readonly Random _random = random;
        public int[] ThrowDices(int[] dices)
        {
            for (int i = 0; i < dices.Length; i++)
            {
                dices[i] = _random.Next(1, 7);
            }
            return dices;
        }
    }
}