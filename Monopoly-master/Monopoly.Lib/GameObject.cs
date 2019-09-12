using System.Drawing;
using Newtonsoft.Json;

namespace Monopoly.Lib
{
    /*
     *  Class design to generalize the creation of a GameObject, aka a object
     *  that is meant to be render on screen. This class includes the x and y coordinates
     *  of the object, the color of the object, the size (width and height) of the object,
     *  the point (x and y combined into a object) of the object, and the rectangle which is
     *  used for drawing the object.
     */
    public abstract class GameObject
    {
        public bool isClickable { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public Size size { get; set; }
        public Point point { get; set; }
        public Rectangle rectangle { get; set; }

        [JsonConstructor]
        public GameObject(int x, int y, Size size)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.point = new Point(this.x, this.y);
            this.rectangle = new Rectangle(this.point, this.size);
            isClickable = false;
        }

        public void Resize(int width, int height)
        {
            if(width > size.Width && height > size.Height)
                rectangle = new Rectangle(new Point(x + width, y + height), new Size(width, height));
            else if (width < size.Width && height > size.Height)
                rectangle = new Rectangle(new Point(x - width, y + height), new Size(width, height));
            else if (width > size.Width && height < size.Height)
                rectangle = new Rectangle(new Point(x + width, y - height), new Size(width, height));
            else
                rectangle = new Rectangle(new Point(x - width, y - height), new Size(width, height));

        }

        public bool GetIsClickable()
        {
            return isClickable;
        }

        public void SetIsClickable(bool isClickable)
        {
            this.isClickable = isClickable;
        }

        public int GetWidth()
        {
            return size.Width;
        }

        public int GetHeight()
        {
            return size.Height;
        }

        public int GetX()
        {
            return x;
        }

        public void SetX(int x)
        {
            this.x = x;
        }

        public int GetY()
        {
            return y;
        }

        public void SetY(int y)
        {
            this.y = y;
        }

        public Size GetSize()
        {
            return size;
        }

        public void SetSize(Size size)
        {
            this.size = size;
        }

        public Point GetPoint()
        {
            return point;
        }

        public void SetPoint(Point point)
        {
            this.point = point;
        }

        public Rectangle GetRectangle()
        {
            return rectangle;
        }

        public void SetRectangle(Rectangle rectangle)
        {
            this.rectangle = rectangle;
            SetSize(new Size(rectangle.Width, rectangle.Height));
        }

        public int AdjustForXOffset(int xOffset)
        {
            return x - xOffset;
        }

        public int AdjustForYOffset(int yOffset)
        {
            return y - yOffset;
        }

        public abstract void Rotate();
        public abstract void Draw(Graphics g);
        public abstract void Draw(Graphics g, int xOffset, int yOffset);
        public abstract void ClickAction();

        public bool WasClicked(int x, int y, int xOffset, int yOffset)
        {
            if (isClickable == true)
            {
                Rectangle temp = new Rectangle(new Point(x, y), new Size(1, 1));
                if (temp.IntersectsWith(new Rectangle(xOffset, yOffset, GetWidth(), GetHeight())))
                {
                    return true;
                }


            }

            return false;
        }
    }
}
