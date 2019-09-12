using Newtonsoft.Json;
using System;
using System.Drawing;

namespace Monopoly.Lib
{
    /*
     * RectangleObject inheirts GameObject. This class implements the Draw, ClickAction, and Rotate
     * functions from GameObject. It has one new variable for drawing the rectangle object a certain color.
     */
    public class RectangleObject : GameObject
    {
        public Color color { get; set; }

        public RectangleObject(int x, int y, Color color, Size size) : base(x, y, size)
        {
            this.color = color;
        }

        [JsonConstructor]
        public RectangleObject(int x, int y, Size size) : base(x, y, size)
        {
            color = Color.White;
        }

        public Color GetColor()
        {
            return color;
        }

        public void SetColor(Color color)
        {
            this.color = color;
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(color), rectangle);
        }

        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            SetRectangle( new Rectangle(xOffset, yOffset, GetWidth(), GetHeight()));
            g.FillRectangle(new SolidBrush(color), GetRectangle());
        }


        public override void Rotate()
        {
            SetRectangle(new Rectangle(new Point(GetRectangle().X, GetRectangle().Y), new Size(GetRectangle().Height, GetRectangle().Width)));
        }

        public void DrawWireFrame(Graphics g)
        {
            g.DrawRectangle(new Pen(color), rectangle);
            g.SetClip(rectangle);
        }

        public override void ClickAction()
        {
            Console.WriteLine(this);
        }
    }
}
