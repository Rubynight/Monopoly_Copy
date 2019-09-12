using Monopoly.Lib;
using Newtonsoft.Json;
using System.Drawing;

namespace Monopoly.Board
{
    public class RailroadSpace : Space, IClickToDrawText
    {
        //Properties
        public Railroad railroad { get; set; }
        public StringFormat utilityNameFormat { get; set; }
        public StringFormat utilityCostFormat { get; set; }

        //Constructor that sets the text format and rotation orientation for the railroad space.
        //Calls the base constructor (Space) to create the geometry for the railroad space.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public RailroadSpace(int spaceId, int x, int y, Railroad railroad, Size size, RotateEnum rotate)
            : base(spaceId, x, y, size, rotate)
        {
            utilityNameFormat = new StringFormat();
            utilityNameFormat.LineAlignment = StringAlignment.Near;
            utilityNameFormat.Alignment = StringAlignment.Center;


            utilityCostFormat = new StringFormat();
            utilityCostFormat.LineAlignment = StringAlignment.Far;
            utilityCostFormat.Alignment = StringAlignment.Center;

            this.railroad = railroad;

            if (rotate == RotateEnum.Rotate90 || rotate == RotateEnum.Rotate270)
                Rotate();

            GenerateQuadrants();
        }

        //Sets owner of railroad assigned to railroad space
        public void SetOwner(Player player)
        {
            railroad.SetOwner(player);
        }

        //Gets owner of railroad assigned to railroad space
        public Player GetOwner()
        {
            return railroad.GetOwner();
        }

        //Gets railroad assigned to railroad space
        public Railroad GetRailroad()
        {
            return railroad;
        }


        //Renders the railroad space text
        //Calls the parent class Draw to render the actually space.
        //The space is render with respect to the camera's viewport
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);

            if (rotateOrientation == RotateEnum.Rotate90 || rotateOrientation == RotateEnum.Rotate270)
            {
                RotateDrawText(g, railroad.GetName(), rectangle, font, textColor, utilityNameFormat);
                RotateDrawText(g, "Pay " + railroad.GetCost().ToString("C0"), rectangle, font, textColor, utilityCostFormat);
            }
            else
            {
                RotateDrawText(g, railroad.GetName(), rectangle, font, textColor, utilityNameFormat);
                RotateDrawText(g, "Pay " + railroad.GetCost().ToString("C0"), rectangle, font, textColor, utilityCostFormat);
            }
        }

        //Interface method for drawing text in the bottom left corner
        //Displays who is the owner of the railroad.
        public string TextToDraw()
        {
            if (railroad.GetOwner() == null)
                return "Owner - Not Owned";
            else
                return "Owner - " + railroad.GetOwner();
        }
    }
}
