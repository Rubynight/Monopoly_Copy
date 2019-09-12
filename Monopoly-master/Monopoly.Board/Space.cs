using System;
using System.Collections.Generic;
using Monopoly.Lib;
using System.Drawing;
using System.Drawing.Drawing2D;
using Newtonsoft.Json;

namespace Monopoly.Board
{
    [JsonObject(IsReference = true)]
    public class Space : RectangleObject
    {
        //Properties
        public int spaceId { get; set; }
        public RotateEnum rotateOrientation { get; set; }
        public Font font { get; set; }
        public SolidBrush textColor { get; set; }
        public List<Player> playersOnSpace { get; set; }
        public StringFormat textFormat { get; set; }
        public RectangleObject[] quadrants { get; set; }

        //Constructors
        [JsonConstructor]
        public Space(int spaceId, int x, int y, Size size, RotateEnum rotate) : base(x, y, size)
        {
            this.spaceId = spaceId;
            font = FormatManager.GetGeneralFont();
            textColor = new SolidBrush(Color.Black);
            rotateOrientation = rotate;
            SetIsClickable(true);
            playersOnSpace = new List<Player>();

            //Default text alignment
            textFormat = new StringFormat();
            textFormat.LineAlignment = StringAlignment.Far;
            textFormat.Alignment = StringAlignment.Center;
        }

        //Generates four smaller rectanlges based on the space's rectangle.
        public void GenerateQuadrants()
        {
            quadrants = new RectangleObject[4];
        
            int spaceWidth = GetWidth();
            int spaceHeight = GetHeight();
            int quadrantWidth = spaceWidth / 2;
            int quadrantHeight = spaceHeight / 2;
            int spaceBeginX = GetX();
            int spaceBeginY = GetY();

            quadrants[0] = new RectangleObject(spaceBeginX, spaceBeginY, new Size(quadrantWidth, quadrantHeight));
            quadrants[1] = new RectangleObject(spaceBeginX + quadrantWidth, spaceBeginY, new Size(quadrantWidth, quadrantHeight));
            quadrants[2] = new RectangleObject(spaceBeginX, spaceBeginY + quadrantHeight, new Size(quadrantWidth, quadrantHeight));
            quadrants[3] = new RectangleObject(spaceBeginX + quadrantWidth, spaceBeginY + quadrantHeight, new Size(quadrantWidth, quadrantHeight));
            
        }

        //Returns the space's id
        public int GetSpaceId()
        {
            return spaceId;
        }

        //Returns the space's rotate orientation
        public RotateEnum GetRotateOrientation()
        {
            return rotateOrientation;
        }

        //Draws the outline of the space in black
        //Calls the parent class Draw to render the space fill.
        //The space is render with respect to the camera's viewport
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);
            g.DrawRectangle(new Pen(Brushes.Black, 2), GetRectangle());             
        }

        //Returns the space's quadrants, generates them if they are null.
        public RectangleObject[] GetQuadrants()
        {
            while (quadrants == null)
                GenerateQuadrants();

            return quadrants;
        }

        //Draws text based on on the space's rotate orientation.
        public void RotateDrawText(Graphics g, string str, Rectangle rectangle, Font font, SolidBrush brush, StringFormat format)
        {
            Matrix m = new Matrix();

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.SmoothingMode = SmoothingMode.HighQuality;

            m.RotateAt((int)rotateOrientation, new PointF(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2));
            g.Transform = m;

            if (font == null)
                font = FormatManager.GetGeneralFont();

            //Calculates new rectangle with padding for text to be drawn 
            Rectangle temp = new Rectangle(rectangle.X , rectangle.Y + (int)(rectangle.Height * 0.10), rectangle.Width, (int)(rectangle.Height * 0.80));
            g.DrawString(str, font, brush, temp, format);

            g.ResetTransform();
            m.Dispose();
        }

        //Adds player to list of players on space
        //Used when player lands or is moved to space.
        public void AddPlayerToSpace(Player player)
        {
            playersOnSpace.Add(player);

        }

        //Removes player from list of players on space
        //Used when player leaves space.
        public void RemovePlayerToSpace(Player player)
        {
            playersOnSpace.Remove(player);
        }
    }
}
