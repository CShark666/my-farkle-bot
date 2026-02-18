namespace Bot
{
    public class DiceService()
    {
        public int[] RollDice(int[] dice)
        {
            for (int i = 0; i < dice.Length; i++)
            {
                dice[i] = Random.Shared.Next(1, 7);
            }
            return dice;
        }
    }
}