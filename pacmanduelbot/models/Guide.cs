
namespace pacmanduelbot.models
{
    class Guide
    {
        //Maze borders
        public const int _WIDTH = 19;
        public const int _HEIGHT = 22;
        public const int _PORTAL1_X = 10;
        public const int _PORTAL1_Y = 0;
        public const int _PORTAL2_X = 10;
        public const int _PORTAL2_Y = 18;

        //Re-spawn zone
        public const int _RESPAWN_X = 10;
        public const int _RESPAWN_Y = 9;

        //Re-spawn forbidden zone
        public const int _FORBIDDEN_L_X = 10;
        public const int _FORBIDDEN_L_Y = 8;
        public const int _FORBIDDEN_R_X = 10;
        public const int _FORBIDDEN_R_Y = 10;

        //Exit points
        public const int _EXIT_UP_X = 9;
        public const int _EXIT_UP_Y = 9;
        public const int _EXIT_DOWN_X = 11;
        public const int _EXIT_DOWN_Y = 9;

        //Maze symbols
        public const char _PLAYER_SYMBOL = 'A';
        public const char _OPPONENT_SYMBOL = 'B';
        public const char _PILL = '.';
        public const char _BONUS_PILL = '*';
        public const char _POISON_PILL = '!';
        public const char _WALL = '#';
        public const int _PILLS = 183;

        public const int _UPPER = _HEIGHT / 2;
        public const int _LOWER = _UPPER - 1;
    }
}
