using pacmanduelbot.brainbox;
using pacmanduelbot.helpers;
using pacmanduelbot.models;
using System;
using System.Drawing;

namespace pacmanduelbot
{
    class Program
    {
        public const String _OUTPUT_FILE_NAME = "game.state";
        static void Main(string[] args)
        {
            //string _filepath1 = @"..\..\..\game.state";
            //var _maze = Maze.Read(_filepath1); 


        }

        public static bool isTheGameStart(char[][] _maze)
        {
            var _PILL_COUNT = 0;
            var _result = false;
            for (var x = 0; x < Guide._HEIGHT; x++)
            {
                for (var y = 0; y < Guide._WIDTH; y++)
                {
                    if (_maze[x][y].Equals(Guide._PILL)
                        || _maze[x][y].Equals(Guide._BONUS_PILL))
                        _PILL_COUNT++;

                }
            }
            if (_PILL_COUNT > Guide._PILLS - 2)
                _result = true;
            return _result;
        }
    }
}