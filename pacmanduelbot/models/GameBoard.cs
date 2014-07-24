using pacmanduelbot.shared;
using System.Collections.Generic;
using System.Drawing;

namespace pacmanduelbot.models
{
    class GameBoard
    {
        public Maze maze { get; set; }
        public Point MaxPlayer { get; set; }
        public Point MinPlayer { get; set; }
        public bool isLastLevel = true;
        public double MaxPoints { get; set; }
        public double MinPoints { get; set; }

        public GameBoard PrecedingBoard { get; set; }
        public List<GameBoard> Childs = new List<GameBoard>();
        public double Utility { get; set; }
        public bool isBestMove { get; set; }
        public bool isTerminal()
        {
            for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                {
                    var _symbol = maze.GetSymbol(new Point { X = x, Y = y });
                    if (_symbol.Equals(Symbols._PILL) || _symbol.Equals(Symbols._BONUS_PILL))
                        return false;
                }
            }
            return true;
        }

        public void MakeMove(Point move, Point currentPoint, char PlayerSymbol)
        {
            maze.SetSymbol(currentPoint, Symbols._EMPTY);
            maze.SetSymbol(move, PlayerSymbol);
        }
    }
}