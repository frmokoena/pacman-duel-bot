using System;
using System.Drawing;

namespace pacmanduelbot.shared
{
    class Mappings
    {
        public static int ManhattanDistance(Point _start, Point _finish)
        {
            return Math.Abs(_start.X - _finish.X) + Math.Abs(_start.Y - _finish.Y);
        }

        public static int CalculateWeight(int _g, int _h)
        {
            return _g + _h;
        }
    }
}