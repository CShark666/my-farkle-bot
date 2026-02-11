namespace Bot
{
    public class Turn
    {
        public int TurnScore { get; private set; }
        public int[] DiceValue { get; set; } = [6];
        public int RemainingDice { get; private set; } = 6;

        public void Roll()
        {
            if (RemainingDice == 0)
                RemainingDice = 6;

            var DiceValue = ThrowDice(RemainingDice);

            if (FarkleCheck(DiceValue))
                DiceValue = [];
        }

        private int[] ThrowDice(int count)
        {
            var values = new int[count];
            for (int i = 0; i < count; i++)
                values[i] = Random.Shared.Next(1, 7);

            return values;
        }


        private bool FarkleCheck(int[] dice)
        {
            throw new NotImplementedException();
        }

    }
}