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
                _map = new char[Guide._HEIGHT][];
                var fileLines = System.IO.File.ReadAllLines(filePath);
                if (fileLines.Length != Guide._HEIGHT)
                {
                    throw new UnreadableMazeException("File should be " + Guide._HEIGHT
                        + " lines, but is " + fileLines.Length + " lines");
                }
                var rowCount = 0;
                foreach (var row in fileLines)
                {
                    _map[rowCount] = row.ToCharArray();
                    if (_map[rowCount].Length != Guide._WIDTH)
                    {
                        throw new UnreadableMazeException("Line " + (rowCount + 1) + " is " + _map[rowCount].Length
                            + " characters wide, but should be " + Guide._WIDTH);
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
            _map = new char[Guide._HEIGHT][];
            for (var x = 0; x < Guide._HEIGHT; x++)
            {
                var row = new char[Guide._WIDTH];
                for (var y = 0; y < Guide._WIDTH; y++)
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
            /*if (p.X < 0 || p.Y < 0 || p.X >= Properties.Settings.Default.MazeHeight || p.Y >= Properties.Settings.Default.MazeWidth)
                return 'B'; // Border*/
            return _map[p.X][p.Y];
        }

        public void SetSymbol(int x, int y, char symbol)
        {
            _map[x][y] = symbol;
        }

        public String ToFlatFormatString()
        {
            var result = "";
            for (var x = 0; x < Guide._HEIGHT; x++)
            {
                for (var y = 0; y < Guide._WIDTH; y++)
                {
                    result += _map[x][y];
                }
                if (x != Guide._HEIGHT - 1)
                    result += '\n';
            }
            return result;
        }


        public void Print()
        {
            Console.Out.WriteLine(ToFlatFormatString());
        }

        public Point FindCoordinateOf(char symbol)
        {
            for (var x = 0; x < Guide._HEIGHT; x++)
            {
                for (var y = 0; y < Guide._WIDTH; y++)
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
                for (var x = 0; x < Guide._HEIGHT; x++)
                {
                    for (var y = 0; y < Guide._WIDTH; y++)
                    {
                        output += _map[x][y];
                    }
                    if (x != Guide._HEIGHT - 1) output += ('\n');
                }
                file.Write(output);
                file.Close();
            }
        }

    }
}
