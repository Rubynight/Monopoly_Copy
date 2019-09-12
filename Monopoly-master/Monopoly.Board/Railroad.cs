using Newtonsoft.Json;

namespace Monopoly.Board
{
    [JsonObject(IsReference = true)]
    public class Railroad
    {
        //Properties
        public string name { get; set; }
        public int cost { get; set; }
        public Player owner { get; set; }


        //Constructor that sets the name and cost of the railroad.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public Railroad(string name, int cost)
        {
            this.name = name;
            this.cost = cost;
        }

        //Gets the name of the railroad
        public string GetName()
        {
            return name;
        }

        //Gets the cost of the railroad
        public int GetCost()
        {
            return cost;
        }

        //Gets the owner of the railroad
        public Player GetOwner()
        {
            return owner;
        }

        //Set the owner of the railroad
        public void SetOwner(Player owner)
        {
            this.owner = owner;
        }

        //Reset the railroad to have no owner
        public void ResetRailroad()
        {
            owner = null;
        }
    }
}
