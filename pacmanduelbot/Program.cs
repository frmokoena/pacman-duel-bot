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

            if (IsTheGameStart(maze))
            { 
                PoisonBucket.FillUpPoisonBucket(); 
                ScoreKeeper.CleanScoreCard(maze); 
            }

            Bot _Bot = new Bot { _maze = maze };

            maze = _Bot.MakeMove();

            maze.WriteMaze(Properties.Settings.Default._OUTPUT_FILE_NAME);
        }

        public static bool IsTheGameStart(Maze _maze)
        {
            var _PILL_COUNT = 0;
            for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                {
                    var _symbol = _maze.GetSymbol(x, y);
                    if (_symbol.Equals(Symbols._PILL))
                        _PILL_COUNT++;
                    if (_symbol.Equals(Symbols._BONUS_PILL))
                        _PILL_COUNT += 10;
                }
            }
            if(_PILL_COUNT == Properties.Settings.Default._MazeTotalPillCount
                || _PILL_COUNT == Properties.Settings.Default._MazeTotalPillCount - 1)
                return true;
            return false;            
        }
    }
}