using pacmanduelbot.brainbox;
using pacmanduelbot.models;
using pacmanduelbot.shared;
using System;

namespace pacmanduelbot
{
    class Program
    {
        static void Main(string[] args)
        {
            //string _filepath = @"..\..\..\game.state";
            //var maze = new Maze(_filepath);
            
        }

        public static bool IsTheGameStart(Maze _maze)
        {
            var _PILL_COUNT = 0;
            var _result = false;
            for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                {
                    var _symbol = _maze.GetSymbol(x, y);
                    if (_symbol.Equals(Symbols._PILL)
                        || _symbol.Equals(Symbols._BONUS_PILL))
                        _PILL_COUNT++;

                }
            }
            if (_PILL_COUNT > Properties.Settings.Default._MazeNumberOfPills - 2)
                _result = true;
            return _result;
        }
    }
}