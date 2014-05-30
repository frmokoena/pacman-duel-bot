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
         
            
            var _maze = Maze.Read(args[0]);

            if (PoisonInventory.isTheGameStart(_maze))
                PoisonInventory.FillUpPoisonInventory();
            
            Bot _Bot = new Bot(_maze);

            _maze = _Bot.MakeMove();

           Maze.Write(_maze, _OUTPUT_FILE_NAME);
             
        }
    }
}
