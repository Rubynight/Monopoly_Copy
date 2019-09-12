using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Monopoly.Board
{
    public class CommunuityChestCards
    {
        private static List<CommunuityChestCards> CommunuityCards;
        private const string fileLocation = @"..\..\..\Resources\Board\CommunuityChestCard.txt";
        private static StreamReader reader;

        //Pull Cards From List
        public static List<CommunuityChestCards> LoadCommunityChestCardFromFile()
        {
            String line;
            String[] lineSplit;
            char token = ',';
            reader = new StreamReader(fileLocation);

            int i = 1;
            int x = 0;
            int id;
            CommunuityCards = new List<CommunuityChestCards>();

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                lineSplit = line.Split(token);
                x++;
                id = x;

                CommunuityCards.Add(new CommunuityChestCards(lineSplit[0], int.Parse(lineSplit[1])));

                i++;
            }

            reader.Close();
            return CommunuityCards;
        }

        public void RemoveCommunityCard(int CommunityCardID)
        {
            //CommunuityCards.Remove(CommunuityCards.Single(x => x.Id = CommunityCardID));
            
        }

        public CommunuityChestCards(string CommunuityCardName, int CommunuityCardEffect)
        {
            this.CommunuityChestName = CommunuityCardName;
            this.CommunuityChestEffect = CommunuityCardEffect;
        }

        private string CommunuityChestName;
        private int CommunuityChestEffect;
         
        //Shuffling
        public string GetCommunuityChestName()
        {
            return CommunuityChestName;
        }

        public void SetCommunuityChestName(string CommunuityChestName)
        {
            this.CommunuityChestName = CommunuityChestName;
        }
        public int GetCommunuityChestEffect()
        {
            return CommunuityChestEffect;
        }

        public void SetCommunuityChestEffect(int CommunuityChestEffect)
        {
            this.CommunuityChestEffect = CommunuityChestEffect;
        }

        //Initiate Card Effect
        public void CardEffect(Player player,  CommunuityChestCards card)
        {
            int amount = card.GetCommunuityChestEffect();

            if (amount < 0)
            {
                amount = Math.Abs(amount);
                player.RemoveMoney(amount);
            }
            else
                player.AddMoney(amount);

        }
    }
}
