using Monopoly.Lib;
using Newtonsoft.Json;
using System.Drawing;

namespace Monopoly.Board
{
    public class FreeParkingSpace : Space, IClickToDrawText
    {
        //Properties
        public const string spaceText = "Free Parking";
        public static int accumulatedMoney { get; set; }

        //Constructor that sets the text format for free parking space and set the total money to zero.
        //Calls the base constructor (Space) to create the geometry for the free parking space.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public FreeParkingSpace(int spaceId, int x, int y, Color color, Size size, RotateEnum rotate)
            : base(spaceId, x, y, size, rotate)
        {
            accumulatedMoney = 0;
            textFormat = new StringFormat();
            textFormat.LineAlignment = StringAlignment.Near;
            textFormat.Alignment = StringAlignment.Center;
            GenerateQuadrants();
            SetIsClickable(true);
        }

        //Gives player the total money store on free parking. 
        //Resets to zero afterwards.
        public void CollectMoney(Player player)
        {
            player.AddMoney(GetAccumulatedMoney());
            accumulatedMoney = 0;
        }


        //Returns the total moeny store on free parking.
        public int GetAccumulatedMoney()
        {
            return accumulatedMoney;
        }

        //Adds money to free parking
        public static void AddMoney(int amount)
        {
            accumulatedMoney += amount;
        }


        //Renders the free parking space text
        //Calls the parent class Draw to render the actually space.
        //The space is render with respect to the camera's viewport
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);
            RotateDrawText(g, spaceText, GetRectangle(), font, textColor, textFormat);
        }

        public string TextToDraw()
        {
            return "Total Money - " + accumulatedMoney.ToString("C2");
        }
    }
}
