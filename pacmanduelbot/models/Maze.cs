using System;
using System.Drawing;
using pacmanduelbot.Exceptions;

namespace pacmanduelbot.models
{
    class Maze
    {
        private readonly char[][] _map;

        public Maze(String filePath)
        {
            try
            {
                _map = new char[Properties.Settings.Default._MazeHeight][];
                var fileLines = System.IO.File.ReadAllLines(filePath);
                if (fileLines.Length != Properties.Settings.Default._MazeHeight)
                {
                    throw new UnreadableMazeException("File should be " + Properties.Settings.Default._MazeHeight
                        + " lines, but is " + fileLines.Length + " lines");
                }
                var rowCount = 0;
                foreach (var row in fileLines)
                {
                    _map[rowCount] = row.ToCharArray();
                    if (_map[rowCount].Length != Properties.Settings.Default._MazeWidth)
                    {
                        throw new UnreadableMazeException("Line " + (rowCount + 1) + " is " + _map[rowCount].Length
                            + " characters wide, but should be " + Properties.Settings.Default._MazeWidth);
                    }
                    rowCount++;
                }
            }
            catch (Exception e)
            {
                throw new UnreadableMazeException("Maze could not be read", e);
            }
        }

        public Maze(Maze maze)
        {
            _map = new char[Properties.Settings.Default._MazeHeight][];
            for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
            {
                var row = new char[Properties.Settings.Default._MazeWidth];
                for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                {
                    row[y] = maze.GetSymbol(x, y);
                }
                _map[x] = row;
            }
        }

        public char GetSymbol(int x, int y)
        {
            return _map[x][y];
        }

        public char GetSymbol(Point p)
        {
            return _map[p.X][p.Y];
        }

        public void SetSymbol(int x, int y, char symbol)
        {
            _map[x][y] = symbol;
        }

        public void SetSymbol(Point p, char symbol)
        {
            _map[p.X][p.Y] = symbol;
        }

        public Point FindCoordinateOf(char symbol)
        {
            for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                {
                    if (_map[x][y] == symbol)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point();
        }

        public void WriteMaze(String filePath)
        {
            using (var file = new System.IO.StreamWriter(filePath))
            {
                var output = "";
                for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
                {
                    for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                    {
                        output += _map[x][y];
                    }
                    if (x != Properties.Settings.Default._MazeHeight - 1) output += ('\n');
                }
                file.Write(output);
                file.Close();
            }
        }
    }
}
