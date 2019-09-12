using Monopoly.Board;
using Monopoly.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Monoploy.Board
{
    public class ChanceCards
    {
        //private Random randomcard;
        //int RemainingCards = 16;
        private static List<ChanceCards> ChanceCardList;
        private const string fileLocation = @"..\..\..\Resources\Board\ChanceFile.txt";
        private static StreamReader ChanceReader;

        // Method to load all of the different chance cards' descriptions and 'card ids' from txt file 

        public static List<ChanceCards> LoadChanceFromFile()
        {
            String line;
            String[] lineSplit;
            char token = '/';
            ChanceReader = new StreamReader(fileLocation);
            ChanceCardList = new List<ChanceCards>();

            while (!ChanceReader.EndOfStream)
            {
                line = ChanceReader.ReadLine();
                lineSplit = line.Split(token);

                ChanceCardList.Add(new ChanceCards(lineSplit[0], int.Parse(lineSplit[1])));

            }

            ChanceReader.Close();
            return ChanceCardList;
        }

        // Chance card constructor

        public ChanceCards(string ChanceCardDescription, int ChanceCardEffect)
        {
            this.ChanceDescription = ChanceCardDescription;
            this.ChanceEffect = ChanceCardEffect;
        }

        private string ChanceDescription;
        private int ChanceEffect;

        // Chance description getter
        public string GetChanceDescription()
        {
            return ChanceDescription;
        }

        // Chance description setter
        public void SetChanceDescription(string ChanceDescription)
        {
            this.ChanceDescription = ChanceDescription;
        }

        // Chance Effect (card id) getter
        public int GetChanceEffect()
        {
            return ChanceEffect;
        }

        // Chance Effect (card id) setter
        public void SetChanceEffect(int ChanceEffect)
        {
            this.ChanceEffect = ChanceEffect;
        }


        // Switch case method for after chance cards are drawn. This method carries the actions of the chance cards.
        public void CardEffect(Player player, GameBoard board, ChanceCards card, List<Player> players)
        {
            switch (ChanceEffect)
            {
                case 1:
                    player.MovePlayerToSpace(board.GetStartingSpace());
                    player.AddMoney(200);
                    break;
                case 2:
                    //Need to be able to purchase space or activate space effect
                    player.MovePlayerToSpace(board.GetSpaceByName("Fleet Street"));
                    break;
                case 3:
                    //Need to be able to purchase space or activate space effect
                    player.MovePlayerToSpace(board.GetSpaceByName("Pall Mall"));
                    break;
                case 4:
                    //Need to be able to purchase space or activate space effect
                    UtilitySpace utilitySpace = (UtilitySpace)board.GetClosestUtility(player.GetSpaceOccupied().GetSpaceId());
                    player.MovePlayerToSpace(utilitySpace);
                    player.RemoveMoney(utilitySpace.GetUtility().GetCost());
                    break;
                case 5:
                    //Need to be able to purchase space or activate space effect
                    RailroadSpace railroadSpace = (RailroadSpace)board.GetClosestRailroad(player.GetSpaceOccupied().GetSpaceId());
                    player.MovePlayerToSpace(railroadSpace);

                    if (railroadSpace.GetOwner() != null)
                    {
                        int payment = player.RemoveMoneyWithReturn(railroadSpace.GetRailroad().GetCost() * 2);
                        railroadSpace.GetOwner().AddMoney(payment);
                    }
                    break;
                case 6:
                    player.AddMoney(50);
                    break;
                case 7:
                    player.SetGetOutOfJailFreeCard(true);
                    break;
                case 8:
                    //Need to be able to purchase space or activate space effect
                    int spacePosition = player.GetSpaceOccupied().GetSpaceId();
                    spacePosition -= 3;

                    if (spacePosition < 0)
                        spacePosition += 40;

                    player.MovePlayerToSpace(board.GetSpaceById(spacePosition));
                    player.Action(board.GetSpaceById(spacePosition), board, players);
                    break;
                case 9:
                    player.MovePlayerToSpace(board.GetJailSpace());

                    JailSpace jailSpace = (JailSpace)board.GetJailSpace();
                    jailSpace.AddPlayerToJail(player);

                    break;
                case 10:
                    int amount = 25 * board.GetTotalNumOfHouse();
                    player.RemoveMoney(amount);
                    break;
                case 11:
                    player.RemoveMoney(15);
                    break;
                case 12:
                    player.MovePlayerToSpace(board.GetSpaceByName("White Chapel Road"));
                    break;
                case 13:
                    player.MovePlayerToSpace(board.GetSpaceByName("Mayfair"));
                    break;
                case 14:
                    for(int i = 0; i < players.Count;i++)
                    {
                        if (player.IsPlayerBankrupt())
                            break;

                        if(player != players[i])
                            players[i].AddMoney(player.RemoveMoneyWithReturn(50));
                    }
                    break;
                case 15:
                    player.AddMoney(150);
                    break;
                case 16:
                    player.AddMoney(100);
                    break;

            }
        }
    }
}
