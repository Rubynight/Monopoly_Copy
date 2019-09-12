using Monopoly.Lib;
using Newtonsoft.Json;
using System.Drawing;

namespace Monopoly.Board
{
    public class GoSpace : Space
    {
        public string spaceText { get; set; }
        public static int MONEY_TO_COLLECT = 200;

        //Constructor that sets the text format for go space.
        //Calls the base constructor (Space) to create the geometry for the go space.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public GoSpace(int spaceId, int x, int y, Color color, Size size, RotateEnum rotate)
            : base(spaceId, x, y, size, rotate)
        {
            textFormat = new StringFormat();
            textFormat.LineAlignment = StringAlignment.Near;
            textFormat.Alignment = StringAlignment.Center;

            spaceText = $"Pass go, collect {MONEY_TO_COLLECT.ToString("C0")}";
            GenerateQuadrants();
            SetIsClickable(true);
        }

        //Draws the text on the Go Space.
        //Calls parent class to draw the actually space.
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);
            RotateDrawText(g, spaceText, GetRectangle(), font, textColor, textFormat);

        }
    }
}
