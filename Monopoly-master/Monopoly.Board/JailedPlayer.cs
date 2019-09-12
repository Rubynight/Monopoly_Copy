using Newtonsoft.Json;

namespace Monopoly.Board
{
    //Used for storing players that have been jailed
    //Basically tracks the player time in jail and check if they can leave
    public class JailedPlayer
    {
        //Properites
        public Player player { get; set; }
        public int timeInJail { get; set; }

        //Construtor that assigns the player that is being jailed and sets
        //There total time to zero.
        [JsonConstructor]
        public JailedPlayer(Player player)
        {
            this.player = player;
            timeInJail = 0;
        }

        //Increases time spent in jail by one
        public void SpendTurnInJail()
        {
                timeInJail++;
        }

        //Checks if the player has being in jail for 3 turns.
        //If they have, they can leave, if not they stay.
        public bool CanPlayerLeaveJail()
        {
            if (timeInJail >= 2)
                return true;
            else
            {
                return false;
            }
        }

        //Gets player in jail
        public Player GetPlayer()
        {
            return player;
        }

    }
}
