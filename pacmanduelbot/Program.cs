using pacmanduelbot.brainbox;
using pacmanduelbot.models;
using System;

namespace pacmanduelbot
{
    class Program
    {
        public const String _OUTPUT_FILE_NAME = "game.state";
        static void Main(string[] args)
        {
            var _maze = Maze.Read(args[0]);

            if (PoisonInventory.isTheGameStart(_maze))
                PoisonInventory.FillUpPoisonInventory();

            Bot _Bot = new Bot(_maze);

            _maze = _Bot.MakeMove();

            Maze.Write(_maze, _OUTPUT_FILE_NAME);
        }
    }
}
