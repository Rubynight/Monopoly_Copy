using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * Only used in local games due to the limitation and design of the game with JSON
 */
namespace Monopoly.Board
{
    public partial class SellProperty : Form
    {
        private Property property;
        private List<Player> buyers;
        private Player seller;
        private int propertyCost;

        //Method used to sell a particular propertry. Uses one property, a list of buyers (players), and 
        //the sell (owner).
        //Also sets up the form to display the property and it's selling price, along with the players
        //who can buy the property.
        public SellProperty(Property property, List<Player> buyers, Player seller)
        {
            InitializeComponent();

            this.property = property;
            this.buyers = buyers;
            this.seller = seller;
            propertyCost = property.GetPropertyCost() + (property.GetPropertyCost() * property.GetNumOfHouses());
            propertyNameTxt.Text = property.GetPropertyName();
            propertyCostTxt.Text = propertyCost.ToString("C2");

            switch(seller.GetPlayerId())
            {
                case 0:
                    playerOneBtn.Enabled = false;
                    break;
                case 1:
                    playerTwoBtn.Enabled = false;
                    break;
                case 2:
                    playerThreeBtn.Enabled = false;
                    break;
                case 3:
                    playerFourBtn.Enabled = false;
                    break;
            }
        }

        //Method that gives buyer the property and removes the cost from their total.
        //If the buyer doesn't have enough, tells the player they cannot afford it.
        private void BuyProperty(Player buyer)
        {
            if (buyer.DoesPlayerHaveMoney(propertyCost))
            {
                seller.AddMoney(buyer.RemoveMoneyWithReturn(propertyCost));
                seller.TradeProperty(property, buyer);
            }
            else
                MessageBox.Show("Cannot afford " + property.GetPropertyName());
        }

        /*
         * Collection of method related to clicking the player buy buttons.
         * Each method is for each player. Calls the BuyProperty method for that player 
         * assigned to that button.
         */
        private void playerOneBtn_Click(object sender, EventArgs e)
        {
            Player buyer = buyers[0];

            BuyProperty(buyer);
            Close();
        }

        private void playerTwoBtn_Click(object sender, EventArgs e)
        {
            Player buyer = buyers[1];

            BuyProperty(buyer);
            Close();
        }

        private void playerThreeBtn_Click(object sender, EventArgs e)
        {
            Player buyer = buyers[2];

            BuyProperty(buyer);
            Close();
        }

        private void playerFourBtn_Click(object sender, EventArgs e)
        {
            Player buyer = buyers[3];

            BuyProperty(buyer);
            Close();
        }
    }
}
