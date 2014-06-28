using pacmanduelbot.brainbox;
using pacmanduelbot.helpers;
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
            var maze = new Maze(args[0]);

            if (IsTheGameStart(maze)) { PoisonBucket.FillUpPoisonBucket(); ScoreCard.CleanScoreCard(maze); }


            Bot _Bot = new Bot { _maze = maze };

            maze = _Bot.MakeMove();

            maze.WriteMaze(Properties.Settings.Default._OUTPUT_FILE_NAME);
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