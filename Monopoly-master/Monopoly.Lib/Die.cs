using System;

namespace Monopoly.Lib
{
    //Look into using api for true random
    //Check for doubles for reroll.
    public class Die
    {
        private Random rand;
        private const int MIN = 1;
        private const int MAX = 6;

        public Die()
        {
            rand = new Random();
        }

        public int GetDiceRoll()
        {
            int result = rand.Next(MIN, MAX + 1);
            return result;
        }
    }
}
