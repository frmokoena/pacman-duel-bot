using System;
using System.Drawing;

namespace pacmanduelbot.shared
{
    class Mappings
    {
        public static int ManhattanDistance(Point _start, Point _finish)
        {
            var result = Math.Abs(_start.X - _finish.X) + Math.Abs(_start.Y - _finish.Y);
            return result;
        }

        public static int CalculateWeight(int _g, int _h)
        {
            var result = _g + _h;
            return result;
        }
    }
}