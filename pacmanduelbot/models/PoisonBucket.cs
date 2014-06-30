using System;
using System.Text.RegularExpressions;

namespace pacmanduelbot.models
{
    class PoisonBucket
    {
        public static readonly string _poisonfilepath = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar
            + "pacmanduelbot" + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar
            + "poisonpillbucket.csv";
        //public static readonly string _poisonfilepath = ".." + System.IO.Path.DirectorySeparatorChar + ".."
        //   + System.IO.Path.DirectorySeparatorChar + ".." + System.IO.Path.DirectorySeparatorChar + "pacmanduelbot"
        //   + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar + "poisonpillbucket.csv";
        public static bool IsPoisonBucketEmpty()
        {
            int _NUMBER_OF_POISON_PILLS;
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
            return !parsed || _NUMBER_OF_POISON_PILLS < 1;
        }

        public static bool IsSelfRespawnNeeded()
        {
            int _RESPAWN_NEEDED;
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
            return !parsed || _RESPAWN_NEEDED > 0;
        }

        public static void FillUpPoisonBucket()
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

        public static void EmptyPoisonBucket()
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