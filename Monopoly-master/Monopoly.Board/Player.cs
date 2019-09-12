using Monoploy.Board;
using Monopoly.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Monopoly.Board
{
    [JsonObject(IsReference = true)]
    public class Player : RectangleObject
    {
        //Properties
        public const int STARTING_MONEY = 1500;
        public int playerId { get; set; }
        public RotateEnum rotateOrientation { get; set; }
        public Space spaceOccupied { get; set; }
        public List<Property> propertysOwned { get; set; }
        public List<Railroad> railroadsOwned { get; set; }
        public int totalMoney { get; set; }
        public bool isJailed { get; set; }
        public bool hasGetOutOfJailCard { get; set; }
        public bool isBankrupt { get; set; }

        //Constructor for the in
        public Player(int playerId, Space space, Color color) :
            base(space.GetX() + (25 * playerId), space.GetY() + space.GetHeight() / 2, color, new Size(space.GetWidth() / 8, space.GetHeight() / 8))
        {
            this.playerId = playerId;
            rotateOrientation = space.GetRotateOrientation();
            MovePlayerToSpace(space);
            propertysOwned = new List<Property>();
            railroadsOwned = new List<Railroad>();
            totalMoney = STARTING_MONEY;
            isJailed = false;
            isBankrupt = false;
            SetIsClickable(true);
        }

        [JsonConstructor]
        public Player(int playerId, int x, int y, Size size, RotateEnum rotate) : base(x, y, size)
        {
            this.playerId = playerId;
            rotateOrientation = rotate;
            propertysOwned = new List<Property>();
            railroadsOwned = new List<Railroad>();
            totalMoney = STARTING_MONEY;
            isJailed = false;
            hasGetOutOfJailCard = false;
            SetIsClickable(true);
        }

        //Rendering player for local games. Excludes the client id since local games do not use
        //client ids.
        public void Draw(Graphics g, int xOffset, int yOffset, int currentPlayer)
        {
            base.Draw(g, xOffset, yOffset);

            if (playerId == currentPlayer)
                g.DrawString("Player " + playerId + " - Total Money - " + totalMoney.ToString("C2"), FormatManager.GetPlayerFont(), new SolidBrush(GetColor()), new PointF(0, 0));
        }

        //Rendering player for server games. Includes the client id since server games use
        //client ids and is used in this method to display the client own infomation along with
        //the current player's information
        public void Draw(Graphics g, int xOffset, int yOffset, int currentPlayer, int clientId)
        {
            base.Draw(g, xOffset, yOffset);

            if (playerId == currentPlayer)
                g.DrawString("Player " + playerId + " - Total Money - " + totalMoney.ToString("C2"), FormatManager.GetPlayerFont(), new SolidBrush(GetColor()), new PointF(0, 0));

            if (playerId == clientId)
            {
                float padding = FormatManager.GetGeneralFont().GetHeight() * 2;
                g.DrawString("You " + "- Total Money - " + totalMoney.ToString("C2"), FormatManager.GetPlayerFont(), new SolidBrush(GetColor()), new PointF(0, padding));

            }

        }

        //Moves player to the space sent
        public void MovePlayerToSpace(Space space)
        {
            spaceOccupied = space;
            RectangleObject[] quadrants = space.GetQuadrants();

            SetX(quadrants[GetPlayerId()].GetX() + quadrants[GetPlayerId()].GetWidth() / 2 - quadrants[GetPlayerId()].GetWidth() / 8);
            SetY(quadrants[GetPlayerId()].GetY() + quadrants[GetPlayerId()].GetHeight() / 2 - quadrants[GetPlayerId()].GetHeight() / 8);

            rotateOrientation = space.GetRotateOrientation();
        }

        //Returns the space occupied space id since it identifies what space you are on.
        public int GetPlayerPosition()
        {
            return spaceOccupied.GetSpaceId();
        }

        //Checks if player has a get of of jail free card. Used to escape jail.
        public bool HasGetOutOfJailFreeCard()
        {
            return hasGetOutOfJailCard;
        }

        //Sets whether a player has a get out of jail free card
        public void SetGetOutOfJailFreeCard(bool jailCard)
        {
            hasGetOutOfJailCard = jailCard;
        }

        //Puts the player in jail by setting the isJailed variable to true.
        public void JailPlayer()
        {
            isJailed = true;
        }

        //Releases the player from jail by setting isJailed to false.
        public void FreePlayer()
        {
            isJailed = false;
        }

        //Checks if the player is jailed by returning the isJailed variable
        public bool IsPlayerJailed()
        {
            return isJailed;
        }

        //Returns the player id
        public int GetPlayerId()
        {
            return playerId;
        }

        //Adds property to player's property list
        public void AddProperty(Property property)
        {
            propertysOwned.Add(property);
        }

        //Gives another player a property that the player owns
        public void TradeProperty(Property property, Player tradePlayer)
        {
            property.SetOwner(tradePlayer);
            tradePlayer.AddProperty(property);
            propertysOwned.Remove(property);
        }

        //Gives another player a railroad that the player owns
        public void TradeRailroad(Railroad railroad, Player tradePlayer)
        {
            railroad.SetOwner(tradePlayer);
            tradePlayer.AddRailraod(railroad);
            railroadsOwned.Remove(railroad);
        }

        //Removes property from property list
        public void RemoveProperty(Property property)
        {
            propertysOwned.Remove(property);
        }

        //Add railroad to railroad list
        public void AddRailraod(Railroad railroad)
        {
            railroadsOwned.Add(railroad);
        }

        //Removes railroad from railroad list
        public void RemoveRailraod(Railroad railroad)
        {
            railroadsOwned.Remove(railroad);
        }

        //Returns the isBankrupt variable for checking if the player is bankrupt
        public bool IsPlayerBankrupt()
        {
            return isBankrupt;
        }

        //Sets the player's color to grey and sets isBankrupt to true.
        //Used when player reaches total money reaches zero.
        public void BankruptPlayer()
        {
            color = Color.Gray;
            isBankrupt = true;
        }

        //Returns the player's total money
        public int GetTotalMoney()
        {
            return totalMoney;
        }

        //Sets the player's total money
        public void SetTotalMoney(int money)
        {
            totalMoney = money;
        }

        //Gives another player all your properties and railroads
        public void TradeAllAssets(Player playerGainingAssets)
        {
            for (int i = 0; i < propertysOwned.Count; i++)
                TradeProperty(propertysOwned[i], playerGainingAssets);

            for (int i = 0; i < railroadsOwned.Count; i++)
                TradeRailroad(railroadsOwned[i], playerGainingAssets);

            playerGainingAssets.AddMoney(this.GetTotalMoney());
            this.SetTotalMoney(0);
        }

        //Checks if a property is in a player's property list
        public bool DoesPlayerOwnProperty(Property property)
        {
            foreach (Property p in propertysOwned)
            {
                if (property == p)
                {
                    return true;
                }
            }

            return false;
        }

        //Checks if a player can afford this property
        public bool DoesPlayerHaveMoney(Property property)
        {
            if (totalMoney >= property.GetPropertyCost())
                return true;
            else
                return false;
        }

        //Checks if a player can afford this railroad
        public bool DoesPlayerHaveMoney(Railroad railroad)
        {
            if (totalMoney >= railroad.GetCost())
                return true;
            else
                return false;
        }

        //Checks if a player can afford this item
        public bool DoesPlayerHaveMoney(int payment)
        {
            if (totalMoney >= payment)
                return true;
            else
                return false;
        }

        //Remove this amount of money from player's total money.
        //Sets the money to zero if money goes below zero.
        public void RemoveMoney(int amount)
        {
            if ((totalMoney - amount) < 0)
                totalMoney = 0;
            else
                totalMoney -= amount;
        }

        //Remove this property's cost from player's total money.
        //Sets the money to zero if money goes below zero.
        public void RemoveMoney(Property property)
        {
            if ((totalMoney - property.GetPropertyCost()) < 0)
                totalMoney = 0;
            else
                totalMoney -= property.GetPropertyCost();
        }

        //Remove this railroad's cost from player's total money.
        //Sets the money to zero if money goes below zero.
        public void RemoveMoney(Railroad railroad)
        {
            if ((totalMoney - railroad.GetCost()) < 0)
                totalMoney = 0;
            else
                totalMoney -= railroad.GetCost();
        }

        //Removes money from player's total money and returns how much was removed.
        public int RemoveMoneyWithReturn(int amount)
        {
            int amountRemoved = 0;

            if ((totalMoney - amount) < 0)
            {
                int negValue = Math.Abs(amount - (totalMoney - amount));
                amountRemoved = amount - negValue;
                totalMoney = 0;
            }
            else
            {
                amountRemoved = amount;
                totalMoney -= amount;
            }

            return amountRemoved;
        }

        //Adds amount to player's total money
        public void AddMoney(int amount)
        {
            totalMoney += amount;
        }

        //Returns space that the player is on
        public Space GetSpaceOccupied()
        {
            return spaceOccupied;
        }

        //Returns the number of railroads the player owns
        public int GetNumOfRailroadsOwned()
        {
            return railroadsOwned.Count;
        }

        //Method that is used to determine what the player action should be.
        //Method depends heavliy on what space is sent over.
        public void Action(Space space, GameBoard board, List<Player> players)
        {
            if (space is PropertySpace)
            {
                PropertySpace temp = (PropertySpace)space;
                PropertySpaceAction(temp);
            }
            else if (space is ChanceSpace)
            {
                //Player draws a chance card and the card effect is applied
                ChanceCards card = ChanceSpace.DrawCard();
                MessageBox.Show(card.GetChanceDescription());
                card.CardEffect(this, board, card, players);

                //Activates the property space action when player is moved to a property space.
                if (card.GetChanceEffect() == 2 || card.GetChanceEffect() == 3 || card.GetChanceEffect() == 13 || card.GetChanceEffect() == 12)
                {
                    PropertySpace temp = (PropertySpace)GetSpaceOccupied();
                    PropertySpaceAction(temp);
                }
            }
            else if (space is CommunityChestSpace)
            {
                //Player draws a community chest card and the card effect is applied
                CommunuityChestCards card = CommunityChestSpace.DrawCard();
                MessageBox.Show(card.GetCommunuityChestName());
                card.CardEffect(this, card);
            }
            else if (space is UtilitySpace)
            {
                UtilityAction(space);
            }
            else if (space is FreeParkingSpace)
            {
                //Gives player who landed on free parking the total money given to free parking.
                FreeParkingSpace fpSpace = (FreeParkingSpace)space;
                MessageBox.Show($"Player {playerId} has collect {fpSpace.GetAccumulatedMoney().ToString("C2")}");
                fpSpace.CollectMoney(this);
            }
            else if (space is RailroadSpace)
            {
                RailroadSpace temp = (RailroadSpace)space;
                RailroadSpaceAction(temp);
            }
            else if (space is GoToJailSpace)
            {
                //Sends player to jail space and jails them
                MessageBox.Show("Sent to Jail");
                MovePlayerToSpace(board.GetJailSpace());
                JailSpace jailSpace = (JailSpace)spaceOccupied;

                if (!isJailed)
                    jailSpace.AddPlayerToJail(this);
                else
                    jailSpace.EscapeJail(this);

            }
            else if (space is JailSpace || IsPlayerJailed() == true)
            {
                //If player is jailed, it allows them to attempt escape or if they 
                //have a get out of jail free card, they can instantly escape.
                JailSpace jailSpace = (JailSpace)board.GetJailSpace();

                if (HasGetOutOfJailFreeCard())
                    jailSpace.RemovePlayerFromJail(this);

                if (isJailed)
                    jailSpace.EscapeJail(this);
            }

            CheckForLost();
        }

        //Action that is perform when a player activates a utility space.
        //Removes utility cost from player.
        private void UtilityAction(Space space)
        {
            UtilitySpace utilitySpace = (UtilitySpace)space;
            MessageBox.Show("This is a utility space - " + utilitySpace.GetUtility().GetName() + ", Cost - " + utilitySpace.GetUtility().GetCost().ToString("C2"));
            utilitySpace.RemoveMoneyFromPlayer(this);
        }

        //Checks if the player's total money is below or equal to zero.
        //If so, bankrupt the player and dispose of all there assests(properties and railroads).
        private void CheckForLost()
        {
            if (totalMoney <= 0)
            {
                MessageBox.Show("Player " + playerId + " has lost all their money.");
                BankruptPlayer();
                LoseAssets();
            }
        }

        //Action that is performed when a player lands on a railroad space.
        //If no one owns this railroad, the player is prompt if they want to buy it
        //else If the space is owned and the player can pay the owner, they can continue playing.
        //Else they cannot pay the owner, they are bankrupt
        private void RailroadSpaceAction(RailroadSpace rSpace)
        {
            if (rSpace.GetOwner() == null && DoesPlayerHaveMoney(rSpace.GetRailroad()))
            {
                if (MessageBox.Show("This is a railroad space - " + rSpace.GetRailroad().GetName() + ", Cost - " + rSpace.GetRailroad().GetCost().ToString("C2"),
                    "Buy?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    railroadsOwned.Add(rSpace.GetRailroad());
                    RemoveMoney(rSpace.GetRailroad());
                    rSpace.SetOwner(this);
                }
            }
            else if (rSpace.GetOwner() != null && DoesPlayerHaveMoney(rSpace.GetRailroad().GetCost() * GetNumOfRailroadsOwned()))
            {

                Player spaceOwner = (Player)rSpace.GetOwner();
                int payment = rSpace.GetRailroad().GetCost() * spaceOwner.GetNumOfRailroadsOwned();

                MessageBox.Show("Player " + spaceOwner.GetPlayerId() + " owns this railroad. You paid them " +
                    payment.ToString("C2") + " in rent");

                spaceOwner.AddMoney(this.RemoveMoneyWithReturn(payment));
            }
            else if (rSpace.GetOwner() != null)
            {
                Player spaceOwner = (Player)rSpace.GetOwner();

                MessageBox.Show("Player " + spaceOwner.GetPlayerId() + " owns this railroad. You cannot pay and have declared bankruptcy");
                BankruptPlayer();

                TradeAllAssets(spaceOwner);
            }
        }

        //Action that is performed when a player lands on a property space.
        //Allows the owner of the space to upgrade the space when landed on.
        //Other players have to pay rent and if they cannot, they become bankrupt.
        //If no one owns the space, the player can purchase it.
        private void PropertySpaceAction(PropertySpace pSpace)
        {
            if (pSpace.GetOwner() == null && DoesPlayerHaveMoney(pSpace.GetProperty()))
            {
                if (MessageBox.Show("Would you like to buy - " + pSpace.GetProperty().GetPropertyName() + ", Cost - " + pSpace.GetProperty().GetPropertyCost().ToString("C2"),
                    "Buy?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    propertysOwned.Add(pSpace.GetProperty());
                    RemoveMoney(pSpace.GetProperty());
                    pSpace.SetOwner(this);
                }
            }
            else if (pSpace.GetOwner() == null && !DoesPlayerHaveMoney(pSpace.GetProperty()))
            {
                MessageBox.Show("Cannot afford " + pSpace.GetProperty().GetPropertyName());
            }
            else if (pSpace.GetOwner() == this && pSpace.GetProperty().GetNumOfHouses() < 5 && DoesPlayerHaveMoney(pSpace.GetProperty().GetUpgradeCost()))
            {
                if (MessageBox.Show("Would you like to upgrade  " + pSpace.GetProperty().GetPropertyName() + ", Cost - " + pSpace.GetProperty().GetPropertyCost().ToString("C2"),
                    "Upgrade?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    pSpace.GetProperty().IncreaseNumOfHouses();
                    RemoveMoney(pSpace.GetProperty().GetUpgradeCost());
                }
            }
            else if (pSpace.GetOwner() == this && pSpace.GetProperty().GetNumOfHouses() == 5)
            {
                MessageBox.Show(pSpace.GetProperty().GetPropertyName() + " is fully upgraded", "WHOA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (pSpace.GetOwner() != null && DoesPlayerHaveMoney(pSpace.GetProperty().CalculatePayment()))
            {
                int payment = pSpace.GetProperty().CalculatePayment();
                Player spaceOwner = (Player)pSpace.GetOwner();

                MessageBox.Show("Player " + spaceOwner.GetPlayerId() + " owns this property. You paid them " +
                    payment.ToString("C2") + " in rent");

                spaceOwner.AddMoney(this.RemoveMoneyWithReturn(payment));
            }
            else if (pSpace.GetOwner() != null && !DoesPlayerHaveMoney(pSpace.GetProperty().CalculatePayment()))
            {
                Player spaceOwner = (Player)pSpace.GetOwner();

                MessageBox.Show("Player " + spaceOwner.GetPlayerId() + " owns this property. You cannot pay and have declared bankruptcy");
                BankruptPlayer();

                TradeAllAssets(spaceOwner);
            }
            else
                MessageBox.Show("If this happens, it's broke.");
        }

        //Resets all player's assets to default (no owner or houses), so other can buy them after
        //the player has been defeated.
        public void LoseAssets()
        {

            for (int i = 0; i < propertysOwned.Count; i++)
            {
                if (propertysOwned[i] != null)
                    propertysOwned[i].ResetProperty();
            }

            for (int i = 0; i < railroadsOwned.Count; i++)
            {
                if (railroadsOwned[i] != null)
                    railroadsOwned[i].ResetRailroad();
            }
        }

        //String representation of the player class
        public override string ToString()
        {
            return "Player " + playerId;
        }
    }
}
