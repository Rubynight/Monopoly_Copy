using Newtonsoft.Json;
using System.Drawing;

namespace Monopoly.Board
{
    [JsonObject(IsReference = true)]
    public class Property
    {
        //Properties 
        public string propertyName { get; set; }
        public int propteryCost { get; set; }
        public int numOfHouses { get; set; }
        public Color color { get; set; }
        public Player owner { get; set; }

        //Constructor that sets the name, cost, and color of the property
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public Property(string propertyName, int propertyCost, int numOfHouses, Color color)
        {
            this.propertyName = propertyName;
            this.propteryCost = propertyCost;
            this.numOfHouses = numOfHouses;
            this.color = color;
        }

        //Method that determines how much a player has to pay in rent when landing on a 
        //owned property space.
        public int CalculatePayment()
        {
            int Payment = 0;

            if (color == Color.Brown)
            {
                int rent = 2;
                Payment = (rent * (numOfHouses * 20));

                if (numOfHouses == 0)
                {
                    Payment = rent;
                }

            }
            if (color == Color.AliceBlue)
            {
                int rent = 6;
                Payment = (rent - 1) * (numOfHouses * 20);
                if (numOfHouses == 0)
                {
                    Payment = rent;
                }
            }

            if (color == Color.MediumVioletRed)
            {
                int rent = 10;
                Payment = (rent - 2) * (numOfHouses * 20);
                if (numOfHouses == 0)
                {
                    Payment = rent;
                }
            }
            if (color == Color.Orange)
            {
                int rent = 14;
                Payment = (rent - 3) * (numOfHouses * 20);
                if (numOfHouses == 0)
                {
                    Payment = rent;
                }
            }
            if (color == Color.Red)
            {
                int rent = 18;
                Payment = (rent - 4) * (numOfHouses * 20);
                if (numOfHouses == 0)
                {
                    Payment = rent;
                }
            }
            if (color == Color.Yellow)
            {
                int rent = 22;
                Payment = (rent - 5) * (numOfHouses * 20);
                if (numOfHouses == 0)
                {
                    Payment = rent;
                }
            }
            if (color == Color.Green)
            {
                int rent = 26;
                Payment = (rent - 6) * (numOfHouses * 20);
                if (numOfHouses == 0)
                {
                    Payment = rent;
                }
            }
            if (color == Color.Blue)
            {
                int rent = 35;
                Payment = (rent - 18) * (numOfHouses * 20);
                if (numOfHouses == 0)
                {
                    Payment = rent;
                }
            }

            return Payment;
        }

        //Cost for upgrading a property
        public int GetUpgradeCost()
        {
            return 100;
        }

        //Gets the property's name
        public string GetPropertyName()
        {
            return propertyName;
        }

        //Sets the property's name
        public void SetPropertyName(string propertyName)
        {
            this.propertyName = propertyName;
        }

        //Gets the cost of the property
        public int GetPropertyCost()
        {
            return propteryCost;
        }

        //Sets the cost of the property
        public void SetPropertyCost(int propteryCost)
        {
            this.propteryCost = propteryCost;
        }

        //Get the number of houses on the property
        public int GetNumOfHouses()
        {
            return numOfHouses;
        }

        //Increases the number of houses on the property by one, to a max of five.
        public void IncreaseNumOfHouses()
        {
            if(numOfHouses < 5)
                numOfHouses++;
        }

        //Gets the color of the property
        public Color GetColor()
        {
            return color;
        }

        //Sets the owner of the property
        public void SetOwner(Player player)
        {
            owner = player;
        }


        //Gets the owner of the property
        public Player GetOwner()
        {
            return owner;
        }

        //Resets the property to have no owner and zero houses
        public void ResetProperty()
        {
            owner = null;
            
            numOfHouses = 0;
        }
    }
}
