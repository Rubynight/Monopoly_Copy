using Newtonsoft.Json;

namespace Monopoly.Board
{
    public class Utility
    {
        //Properties
        public string name { get; set; }
        public int cost { get; set; }

        //Constructor that sets the name and cost of the utility.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public Utility(string name, int cost)
        {
            this.name = name;
            this.cost = cost;
        }

        //Gets name of the utility
        public string GetName()
        {
            return name;
        }

        //Gets cost ofthe utility
        public int GetCost()
        {
            return cost;
        }
    }
}
