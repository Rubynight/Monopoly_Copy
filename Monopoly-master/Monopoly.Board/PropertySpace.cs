using System;
using Monopoly.Lib;
using System.Drawing;
using System.Drawing.Drawing2D;
using Newtonsoft.Json;

namespace Monopoly.Board
{
    [JsonObject(IsReference = true)]
    public class PropertySpace : Space, IClickToDrawText
    {
        //Fill multipler determines how much space upperRectangle
        //takes in NormalSpace percentage wise. lowerRectangle will fill
        //the remaining space.
        public const double fillMultipler = 0.20;
        public RectangleObject upperRectangle { get; set; }
        public RectangleObject lowerRectangle { get; set; }
        public Color upperRectangleColor { get; set; }
        public Color lowerRectangleColor = Color.White;
        public Property property { get; set; }
        public int upperRectangleHeigt { get; set; }
        public int lowerRectangleHeight { get; set; }
        public StringFormat propertyNameFormat { get; set; }
        public StringFormat propertyCostFormat { get; set; }

        //Constructor for property space, assigns the property and generates the two inner rectangles. The upper is a rectangle that stores the houses and 
        //displays the color of the property. The lower rectangle is for holding the players on the space and displaying the name and cost of the space.
        [JsonConstructor]
        public PropertySpace(int spaceId, int x, int y, Property property, Size size, RotateEnum rotate) : base(spaceId, x, y, size, rotate)
        {
            upperRectangleHeigt = (int)(rectangle.Height * fillMultipler);
            lowerRectangleHeight = (int)(rectangle.Height * (1 - fillMultipler));
            upperRectangleColor = property.GetColor();
            InitialSetupInnerRectangles(rotate);
            this.property = property;

            propertyNameFormat = new StringFormat();
            propertyNameFormat.LineAlignment = StringAlignment.Near;
            propertyNameFormat.Alignment = StringAlignment.Center;

            propertyCostFormat = new StringFormat();
            propertyCostFormat.LineAlignment = StringAlignment.Far;
            propertyCostFormat.Alignment = StringAlignment.Center;
            GenerateQuadrants();
            SetIsClickable(true);
        }

        //Generates for squares inside the lower rectangle used for displaying the players in a uniform fashion.
        //Players get drawn in the center of each quadrant, resulting in a grid.
        public new void GenerateQuadrants()
        {
            quadrants = new RectangleObject[4];
            RectangleObject rectangle = GetLowerRectangle();

            int spaceWidth = rectangle.GetWidth();
            int spaceHeight = rectangle.GetHeight();
            int quadrantWidth = spaceWidth / 2;
            int quadrantHeight = spaceHeight / 2;
            int spaceBeginX = rectangle.GetX();
            int spaceBeginY = rectangle.GetY();

            quadrants[0] = new RectangleObject(spaceBeginX, spaceBeginY, new Size(quadrantWidth, quadrantHeight));
            quadrants[1] = new RectangleObject(spaceBeginX + quadrantWidth, spaceBeginY, new Size(quadrantWidth, quadrantHeight));
            quadrants[2] = new RectangleObject(spaceBeginX, spaceBeginY + quadrantHeight, new Size(quadrantWidth, quadrantHeight));
            quadrants[3] = new RectangleObject(spaceBeginX + quadrantWidth, spaceBeginY + quadrantHeight, new Size(quadrantWidth, quadrantHeight));
        }

        //Set the owner of the property 
        public void SetOwner(Player player)
        {
            property.SetOwner(player);
        }

        //Get the owner of the property
        public GameObject GetOwner()
        {
            return property.GetOwner();
        }

        //Set the property on the on the property space
        public void SetProperty(Property property)
        {
            this.property = property;
        }

        //Get the property assigned to the property space
        public Property GetProperty()
        {
            return property;
        }

        //Rotates the space by 90 degrees
        public new void Rotate()
        {
            base.Rotate();
            InitialSetupInnerRectangles(RotateEnum.Rotate90);
        }

        //Sets up the inner rectangles based on their rotation orientation
        public void InitialSetupInnerRectangles(RotateEnum rotate)
        {

            switch (rotate)
            {
                case RotateEnum.DoNotRotate:
                    upperRectangle = new RectangleObject(rectangle.X, rectangle.Y, upperRectangleColor, new Size(rectangle.Width, upperRectangleHeigt));
                    lowerRectangle = new RectangleObject(rectangle.X, rectangle.Y + upperRectangleHeigt,
                        lowerRectangleColor, new Size(rectangle.Width, lowerRectangleHeight));
                    break;
                case RotateEnum.Rotate90:
                    upperRectangle = new RectangleObject(rectangle.X, rectangle.Y, upperRectangleColor, new Size(upperRectangleHeigt, rectangle.Width));
                    lowerRectangle = new RectangleObject(rectangle.X + upperRectangleHeigt, rectangle.Y,
                        lowerRectangleColor, new Size(lowerRectangleHeight, rectangle.Width));
                    //base.Rotate();
                    break;
                case RotateEnum.Rotate180:
                    upperRectangle = new RectangleObject(rectangle.X, rectangle.Y + lowerRectangleHeight, upperRectangleColor, new Size(rectangle.Width, upperRectangleHeigt));
                    lowerRectangle = new RectangleObject(rectangle.X, rectangle.Y, lowerRectangleColor,
                        new Size(rectangle.Width, lowerRectangleHeight));
                    break;
                case RotateEnum.Rotate270:
                    upperRectangle = new RectangleObject(rectangle.X + lowerRectangleHeight, rectangle.Y, upperRectangleColor, new Size(upperRectangleHeigt, rectangle.Width));
                    lowerRectangle = new RectangleObject(rectangle.X, rectangle.Y, lowerRectangleColor,
                        new Size(lowerRectangleHeight, rectangle.Width));
                    //base.Rotate();
                    break;
            }
        }

        //Draws property space with respect to the camera.
        //Calls parent class for drawing the space geometry.
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);

            InitialSetupInnerRectangles(rotateOrientation);

            g.FillRectangle(new SolidBrush(upperRectangle.GetColor()), upperRectangle.GetRectangle());
            g.FillRectangle(new SolidBrush(lowerRectangle.GetColor()), lowerRectangle.GetRectangle());

            RotateDrawText(g, property.GetPropertyName(), lowerRectangle.GetRectangle(), font, textColor, propertyNameFormat);
            RotateDrawText(g, property.GetPropertyCost().ToString("C2"), lowerRectangle.GetRectangle(), font, textColor, propertyCostFormat);

            g.DrawRectangle(new Pen(Color.Black, 2), lowerRectangle.GetRectangle());
            g.DrawRectangle(new Pen(Color.Black, 2), upperRectangle.GetRectangle());

            int houseWidth;
            int houseHeight;
            int houses = property.GetNumOfHouses();


            //Draws houses depending on object orientation
            if (rotateOrientation == RotateEnum.Rotate90 || rotateOrientation == RotateEnum.Rotate270)
            {
                houseWidth = upperRectangle.GetWidth() / 2;
                houseHeight = upperRectangle.GetHeight() / 5;

                for (int i = 0; i < houses; i++)
                {
                    Color houseColor = property.GetOwner().GetColor();

                    Rectangle tempRect = new Rectangle(upperRectangle.GetX() + (houseWidth / 2), upperRectangle.GetY() + (i * houseHeight), houseWidth, houseHeight);
                    g.FillRectangle(new SolidBrush(houseColor), tempRect);
                    g.DrawRectangle(new Pen(Color.Black, 2), tempRect);
                }
            }
            else
            {
                houseWidth = upperRectangle.GetWidth() / 5;
                houseHeight = upperRectangle.GetHeight() / 2;

                for (int i = 0; i < houses; i++)
                {
                    Color houseColor = property.GetOwner().GetColor();

                    Rectangle tempRect = new Rectangle(upperRectangle.GetX() + (i * houseWidth), upperRectangle.GetY() + (houseHeight / 2), houseWidth, houseHeight);
                    g.FillRectangle(new SolidBrush(houseColor), tempRect);
                    g.DrawRectangle(new Pen(Color.Black, 2), tempRect);
                }
            }
        }

        //Overides parent's class method since text is render in the lower rectangle.
        //Draws text based on on the space's rotate orientation.
        public new void RotateDrawText(Graphics g, string str, Rectangle rectangle, Font font, SolidBrush brush, StringFormat format)
        {
            Matrix m = new Matrix();

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.SmoothingMode = SmoothingMode.HighQuality;

            m.RotateAt((int)rotateOrientation, new PointF(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2));
            g.Transform = m;

            if (font == null)
                font = FormatManager.GetGeneralFont();

            if (rotateOrientation == RotateEnum.Rotate270 || rotateOrientation == RotateEnum.Rotate90)
            {
                Rectangle temp = new Rectangle(rectangle.X, rectangle.Y + 25, rectangle.Width, (int)(rectangle.Height * 0.75));
                g.DrawString(str, font, brush, temp, format);
            }
            else
            {
                Rectangle temp = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, (int)(rectangle.Height * 1));
                g.DrawString(str, font, brush, temp, format);
            }

            g.ResetTransform();
            m.Dispose();
        }

        //Returns the lower rectangle 
        public RectangleObject GetLowerRectangle()
        {
            return lowerRectangle;
        }

        //Get rectangle of the space with respect to the camera
        public Rectangle GetOffsetRect(int xOffset, int yOffset, int width, int height)
        {
            return new Rectangle(xOffset, yOffset, width, height);
        }


        //The to string representation of a property space
        public override string ToString()
        {
            return property.GetPropertyName() ;
        }

        //Interface method for drawing text in the bottom left corner
        //Displays who is the owner of the property space and number of houses the property has.
        public string TextToDraw()
        {
            if (property.GetOwner() == null)
                return "Owner - Not Owned" + "      Number of houses - " + property.GetNumOfHouses();
            else
                return "Owner - " + property.GetOwner() + "       Number of houses - " + property.GetNumOfHouses();
        }
    }
}
