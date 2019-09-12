using Monopoly.Lib;
using Newtonsoft.Json;
using System.Drawing;

namespace Monopoly.Board
{
    public class GoToJailSpace : Space
    {
        public string spaceText { get; set; }

        //Constructor that sets the text format for go to jail space.
        //Calls the base constructor (Space) to create the geometry for the go space.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public GoToJailSpace(int spaceId, int x, int y, Color color, Size size, RotateEnum rotate)
            : base(spaceId, x, y, size, rotate)
        {
            textFormat = new StringFormat();
            textFormat.LineAlignment = StringAlignment.Near;
            textFormat.Alignment = StringAlignment.Center;
            spaceText = "Go To Jail";
            GenerateQuadrants();
            SetIsClickable(true);
        }


        //Draws the text on the Go to Jail Space.
        //Calls parent class to draw the actually space.
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);
            RotateDrawText(g, spaceText, GetRectangle(), font, textColor, textFormat);

        }
    }
}
