using Monopoly.Lib;
using Newtonsoft.Json;
using System.Drawing;

namespace Monopoly.Board
{
    public class UtilitySpace : Space
    {
        //Properties
        public Utility utility { get; set; }
        public StringFormat utilityNameFormat { get; set; }
        public StringFormat utilityCostFormat { get; set; }

        //Constructor that sets the text format and rotation orientation for the utility space.
        //Calls the base constructor (Space) to create the geometry for the utility space.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public UtilitySpace(int spaceId, int x, int y, Utility utility, Size size, RotateEnum rotate) 
            : base(spaceId, x, y, size, rotate)
        {
            utilityNameFormat = new StringFormat();
            utilityNameFormat.LineAlignment = StringAlignment.Near;
            utilityNameFormat.Alignment = StringAlignment.Center;

            utilityCostFormat = new StringFormat();
            utilityCostFormat.LineAlignment = StringAlignment.Far;
            utilityCostFormat.Alignment = StringAlignment.Center;

            this.utility = utility;

            if (rotate == RotateEnum.Rotate90 || rotate == RotateEnum.Rotate270)
                Rotate();

            GenerateQuadrants();
        }

        //Gets the utility assigned to the utility space
        public Utility GetUtility()
        {
            return utility;
        }

        //Removes money from player who landed on the utility space and adds it to free parking
        public void RemoveMoneyFromPlayer(Player player)
        {
            FreeParkingSpace.AddMoney(player.RemoveMoneyWithReturn(utility.GetCost()));
        }


        //Renders the utility space text
        //Calls the parent class Draw to render the actually space.
        //The space is render with respect to the camera's viewport
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);

            RotateDrawText(g, utility.GetName(), GetRectangle(), font, textColor, utilityNameFormat);
            RotateDrawText(g, "Pay " + utility.GetCost().ToString("C0"), GetRectangle(), font, textColor, utilityCostFormat);     
        }
    }
}
