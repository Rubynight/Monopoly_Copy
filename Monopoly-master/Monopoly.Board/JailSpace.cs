using Monopoly.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Monopoly.Board
{
    public class JailSpace : Space, IClickToDrawText
    {
        //Properties
        public string spaceText { get; set; }
        public List<JailedPlayer> playersInJail { get; set; }
        public Die die { get; set; }

        //Constructor that sets the text format for Jail space.
        //Creates a list of players in jail and a die for players trying to leave jail.
        //Calls the base constructor (Space) to create the geometry for the go space.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public JailSpace(int spaceId, int x, int y, Color color, Size size, RotateEnum rotate)
            : base(spaceId, x, y, size, rotate)
        {
            playersInJail = new List<JailedPlayer>();
            die = new Die();

            textFormat = new StringFormat();
            textFormat.LineAlignment = StringAlignment.Near;
            textFormat.Alignment = StringAlignment.Center;

            spaceText = "JAIL";
            GenerateQuadrants();
            SetIsClickable(true);
        }

        //Rolls the dice and checks if the player rolls doubles.
        //If player rolls doubles or have being in jail for 3 turns, they are released from jail
        //Else they stay in jail and time spent is increased by one.
        public bool EscapeJail(Player player)
        {
            int rollOne = die.GetDiceRoll();
            int rollTwo = die.GetDiceRoll();
            JailedPlayer jPlayer = playersInJail[GetJailedPlayerIndex(player)];


            if (rollOne == rollTwo || jPlayer.CanPlayerLeaveJail())
            {
                MessageBox.Show("Rolled: " + rollOne + " " + rollTwo + "\n Can leave jail.");
                RemovePlayerFromJail(jPlayer);
                return true;
            }
            else
            {
                MessageBox.Show("Rolled: " + rollOne + " " + rollTwo + "\n Cannot leave jail.");
                jPlayer.SpendTurnInJail();
                return false;
            }
                
        }

        //Adds player to jail via a JailPlayer object
        public void AddPlayerToJail(Player player)
        {
            playersInJail.Add(new JailedPlayer(player));
            player.JailPlayer();
        }

        //Removes JailPlayer from jail
        public void RemovePlayerFromJail(JailedPlayer jPlayer)
        {
            jPlayer.GetPlayer().FreePlayer();
            playersInJail.Remove(jPlayer);
            
        }

        //Removes Player from jail
        public void RemovePlayerFromJail(Player player)
        {
            JailedPlayer jPlayer = playersInJail[GetJailedPlayerIndex(player)];

            RemovePlayerFromJail(jPlayer);
        }

        //Get jail player via a player object, returns index of player if they are found
        //else return -1.
        public int GetJailedPlayerIndex(Player player)
        {
            for (int i = 0; i < playersInJail.Count; i++)
            {
                if (playersInJail[i].GetPlayer() == player)
                    return i;
            }

            return -1;
        }

        //Draws the text on the Go to Jail Space.
        //Calls parent class to draw the actually space.
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);
            RotateDrawText(g, spaceText, GetRectangle(), font, textColor, textFormat);

        }

        //Interface method for drawing text in the bottom left corner
        //Displays how many players are in jail and who.
        public string TextToDraw()
        {
            string jailedPlayers = "Players in Jail - ";

            foreach(JailedPlayer player in playersInJail)
            {
                jailedPlayers += "Player " + player.GetPlayer().GetPlayerId() + ", ";
            }

            return jailedPlayers;
        }
    }
}
