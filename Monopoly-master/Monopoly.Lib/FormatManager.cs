using System.Drawing;

namespace Monopoly.Lib
{
    /*
     * Static class for getting information related to the font used in the game.
     */
    public static class FormatManager
    {
        /*
         *  Font size variable for storing the standard font size used
         *  Font family variable for storing the standard font family used
         *  A font object that is based off the fontSize and fontFamily variables.
         *  Size multiplier for scale objects in the game in uniform.
         */ 
        private static int generalFontSize;
        private static string generalFontFamily;
        private static Font generalFont;
        private static double sizeMultiplier;

        private static int playerFontSize;
        private static string playerFontFamily;
        private static Font playerFont;

        public static int GetGeneralFontSize()
        {
            return generalFontSize;
        }

        public static string GetGeneralFontFamily()
        {
            return generalFontFamily;
        }

        public static Font GetGeneralFont()
        {
            return generalFont;
        }

        public static Font GetPlayerFont()
        {
            return playerFont;
        }

        public static void SetupFormatManager(SizeEnum size)
        {
            SetSizeMultiplier(size);
            SetupGameFont("Arial", size);
        }

        public static void SetupGeneralGameFont(string xFontFamily, int xFontSize)
        {
            generalFontFamily = xFontFamily;
            generalFontSize = xFontSize;
            generalFont = new Font(generalFontFamily, generalFontSize);
        }

        public static void SetupPlayerGameFont(string xFontFamily, int xFontSize)
        {
            playerFontFamily = xFontFamily;
            playerFontSize = xFontSize;
            playerFont = new Font(playerFontFamily, playerFontSize);
        }

        public static void SetupGameFont(string xFontFamily, SizeEnum size)
        {
            generalFontFamily = xFontFamily;

            switch (size)
            {
                case SizeEnum.Small:
                    generalFontSize = 6;
                    break;
                case SizeEnum.Medium:
                    generalFontSize = 8;
                    break;
                case SizeEnum.Large:
                    generalFontSize = 10;
                    break;
                default:
                    generalFontSize = 8;
                    break;
            }

            SetupPlayerGameFont("Arial", 24);
            generalFont = new Font(generalFontFamily, generalFontSize);
        }

        public static Font FindFont(Graphics g, string longString, Size Room, Font PreferedFont)
        {
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;
            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }

        public static double GetSizeMultiplier()
        {
            return sizeMultiplier;
        }

        public static void SetSizeMultiplier(SizeEnum size)
        {
            if (size == SizeEnum.Large)
                sizeMultiplier = 1;
            else if (size == SizeEnum.Medium)
                sizeMultiplier = 0.75;
            else
                sizeMultiplier = 0.50;
        }
    }
}
