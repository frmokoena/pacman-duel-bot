using System;
using System.Text.RegularExpressions;

namespace pacmanduelbot.models
{
    public static class Maze
    {
        public static char[][] Read(String filePath)
        {
            var map = new char[Guide._HEIGHT][];
            try
            {
                var fileContents = System.IO.File.ReadAllText(filePath);
                var rowCount = 0;
                foreach (var row in Regex.Split(fileContents, "\n"))
                {
                    map[rowCount] = row.ToCharArray();
                    rowCount++;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            return map;
        }

        public static void Write(char[][] maze, String filePath)
        {
            using (var file = new System.IO.StreamWriter(filePath))
            {
                var output = "";
                for (var x = 0; x < Guide._HEIGHT; x++)
                {
                    for (var y = 0; y < Guide._WIDTH; y++)
                    {
                        output += maze[x][y];
                    }
                    if (x != Guide._HEIGHT - 1) output += ('\n');
                }
                file.Write(output);
                file.Close();
            }
        }
    }
}