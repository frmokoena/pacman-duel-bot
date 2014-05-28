using System;
using System.Text.RegularExpressions;

namespace pacmanduelbot.models
{
    public class PoisonInventory
    {
        public static readonly string _poisonfilepath = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar
          + "pacmanduelbot" + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar
          + "poison_pill.csv";
        //public static readonly string _poisonfilepath = ".." + System.IO.Path.DirectorySeparatorChar + ".." + System.IO.Path.DirectorySeparatorChar
          //+ ".." + System.IO.Path.DirectorySeparatorChar + "pacmanduelbot" + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar
          //+ "POISON_PILL.csv";
        public static bool arePoisonPillsExhausted()
        {
            int _NUMBER_OF_POISON_PILLS;
            var result = false;
            var _CONTENTS = new string[2];

            try
            {
                var _input = System.IO.File.ReadAllText(_poisonfilepath);
                var columnCount = 0;
                foreach (var column in Regex.Split(_input, ","))
                {
                    _CONTENTS[columnCount] = column;
                    columnCount++;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }


            bool parsed = Int32.TryParse(_CONTENTS[0], out _NUMBER_OF_POISON_PILLS);

            if (!parsed)
                return result;
            if (_NUMBER_OF_POISON_PILLS < 1)
                result = true;
            return result;
        }



        public static bool isSelfRespawn()
        {
            int _RESPAWN_NEEDED;
            var result = false;
            var _CONTENTS = new string[2];

            try
            {
                var _input = System.IO.File.ReadAllText(_poisonfilepath);
                var columnCount = 0;
                foreach (var column in Regex.Split(_input, ","))
                {
                    _CONTENTS[columnCount] = column;
                    columnCount++;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }


            bool parsed = Int32.TryParse(_CONTENTS[1], out _RESPAWN_NEEDED);

            if (!parsed)
                return result;
            if (_RESPAWN_NEEDED > 0)
                result = true;
            return result;
        }

        public static bool isTheGameStart(char[][] _maze)
        {
            var _PILL_COUNT = 0;
            var result = false;
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
                result = true;
            return result;
        }

        public static void FillUpPoisonInventory()
        {
            string _input = "1,0";
            using (var file = new System.IO.StreamWriter(_poisonfilepath, false))
            {
                file.Write(_input);
                file.Close();
            }
        }


        public static void DropPoisonPill()
        {
            string _input = "0,1";
            using (var file = new System.IO.StreamWriter(_poisonfilepath, false))
            {
                file.Write(_input);
                file.Close();
            }
        }

        public static void EmptyPoisonInventory()
        {
            string _input = "0,0";
            using (var file = new System.IO.StreamWriter(_poisonfilepath, false))
            {
                file.Write(_input);
                file.Close();
            }
        }

    }
}
