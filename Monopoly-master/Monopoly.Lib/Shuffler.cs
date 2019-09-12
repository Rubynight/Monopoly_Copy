using System;
using System.Collections.Generic;

namespace Monopoly.Lib
{
    public class Shuffler<T>
    {
        private List<T> shuffleList;
        private Random rand;

        public Shuffler(List<T> list)
        {
            shuffleList = new List<T>();
            shuffleList.AddRange(list);
            rand = new Random();
        }

        public List<T> GetNewShuffle()
        {
            List<T> shuffle = Shuffle();

            return shuffle;
        }

        /*
         * Generic version of the Fisher-Yates Shuffle algorithm.
         * Can shuffle a list of any type.
         * https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
         */
        public List<T> Shuffle()
        {
            int randomIndex;
            int max = shuffleList.Count;
            T temp = default(T);

            for(int i = 0; i < max; i++)
            {
                randomIndex = rand.Next(0, max);
                
                if(randomIndex != i)
                {
                    temp = shuffleList[i];
                    shuffleList[i] = shuffleList[randomIndex];
                    shuffleList[randomIndex] = temp;
                }
            }

            return shuffleList;
        }

    }
}
