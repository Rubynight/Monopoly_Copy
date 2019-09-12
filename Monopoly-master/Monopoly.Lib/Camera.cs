using System;
using System.Drawing;

namespace Monopoly.Lib
{
    public class Camera
    {
        private int xOffset, yOffset;
        private int windowWidth, windowHeight;
        private GameObject gameObject;
        public static int CAMERA_SPEED = 40;
        private int xStart, xEnd, yStart, yEnd;

        //Creates a camera with a specific width and height (viewport)
        public Camera(int width, int height, int xOffset, int yOffset)
        {
            this.windowWidth = width;
            this.windowHeight = height;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }

        //Centers camera on gameobject
        public void centerOnGameObject(GameObject gameObject)
        {
            setxOffset((gameObject.GetX() - windowWidth / 2) + (gameObject.GetWidth() / 2));
            setyOffset((gameObject.GetY() - windowHeight / 2) + (gameObject.GetHeight() / 2));
            this.gameObject = gameObject;
        }

        //Centers camera on x and y location
        public void centerOnPoint(int x, int y)
        {
            setxOffset((x - windowWidth / 2) + 1 / 2);
            setyOffset((y - windowHeight / 2) + 1 / 2);
        }

        //Checks if the gameobject is in the camera's viewport, return true if it is, else false.
        public bool IsInViewPort(GameObject gameObject)
        {
            xStart = Math.Min(0, GetXOffset() / gameObject.GetWidth());
            xEnd = Math.Max(windowWidth, (GetXOffset() + windowWidth) / gameObject.GetWidth() + 1);

            // Gets starting and ending y positions of viewport
            yStart = Math.Min(0, GetYOffset() / gameObject.GetHeight());
            yEnd = Math.Max(windowHeight, (GetYOffset() + windowHeight) / gameObject.GetHeight() + 1);

            Rectangle windowViewport = new Rectangle(new Point(xStart, yStart), new Size(xEnd - xStart, yEnd - yStart));
            Rectangle tempGameObject = new Rectangle(gameObject.AdjustForXOffset(GetXOffset()), gameObject.AdjustForYOffset(GetYOffset()), gameObject.GetWidth(), gameObject.GetHeight());

            if (windowViewport.IntersectsWith(tempGameObject))
            {
                return true;
            }

            return false;
        }

        //Moves camera by increasing and decreasing the x and y coordinates
        public void move(int x, int y)
        {
            xOffset += x;
            yOffset += y;
        }

        //Get the xOffset for drawing gameobject with respect to the camera
        public int GetXOffset()
        {
            return xOffset;
        }

        //Sets the camera's xOffset
        public void setxOffset(int xOffset)
        {
            this.xOffset = xOffset;
        }

        //Get the yOffset for drawing gameobject with respect to the camera
        public int GetYOffset()
        {
            return yOffset;
        }

        //Sets the camera's yOffset
        public void setyOffset(int yOffset)
        {
            this.yOffset = yOffset;
        }
    }
}
