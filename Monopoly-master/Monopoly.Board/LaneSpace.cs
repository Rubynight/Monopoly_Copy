using Monopoly.Lib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly.Board
{
    public class LaneSpace : Space
    {


        public LaneSpace(int spaceId, int x, int y, Color color, RotateEnum rotate) : base(spaceId, x, y, color, rotate)
        {
            double sizeMultiplier = FormatManager.GetSizeMultiplier();
            int CornerWidth = (int)(150 * sizeMultiplier);
            int CornerHeight = (int)(150 * sizeMultiplier);
            int LaneWidth = (int)(100 * sizeMultiplier);
            int LaneHeight = (int)(150 * sizeMultiplier);

            size = new Size(LaneWidth, LaneHeight);
        }
    }
}
