using pacmanduelbot.models;
using pacmanduelbot.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pacmanduelbot.helpers
{
    class ScoreCard
    {
        public static readonly string _pathToScoreCard = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar
            + "pacmanduelbot" + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar
            + "scoreboard.csv";
        public static readonly string _pathToGameState = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar
            + "pacmanduelbot" + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar
            + "game.state";

        public static void CleanScoreCard(Maze _maze)
        {
            string _input;
            _maze.WriteMaze(_pathToGameState);
            if (PillCountToScore(_maze) < Properties.Settings.Default._MazeTotalPillCount)
                _input = "0,1";
            else
                _input = "0,0";
            using (var file = new System.IO.StreamWriter(_pathToScoreCard, false))
            {
                file.Write(_input);
                file.Close();
            }
        }


        public static int GetPlayerAScore()
        {
            int _PlayerAScore;
            var _CONTENTS = new string[2];
            try
            {
                var _input = System.IO.File.ReadAllText(_pathToScoreCard);
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
            bool parsed = Int32.TryParse(_CONTENTS[0], out _PlayerAScore);
            if (!parsed)
                return -1;
            return _PlayerAScore;
        }

        public static int GetPlayerBScore()
        {
            int _PlayerBScore;
            var _CONTENTS = new string[2];
            try
            {
                var _input = System.IO.File.ReadAllText(_pathToScoreCard);
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
            bool parsed = Int32.TryParse(_CONTENTS[1], out _PlayerBScore);
            if (!parsed)
                return -1;
            return _PlayerBScore;
        }


        public static void UpdateScore(Maze _maze, bool MaxPlayer)
        {
            var _playerASscore = GetPlayerAScore();
            var _playerBScore = GetPlayerBScore();

            var _previousMaze = new Maze(_pathToGameState);
            var _currentMaze = new Maze(_maze);

            var _previousPillCountToScore = PillCountToScore(_previousMaze);
            var _currentPillCountToScore = PillCountToScore(_currentMaze);

            if(_currentPillCountToScore != _previousPillCountToScore)
            {
                if (_currentPillCountToScore == _previousPillCountToScore - 1)
                    if (MaxPlayer)
                        _playerASscore++;
                    else
                        _playerBScore++;
                if (_currentPillCountToScore == _previousPillCountToScore - 10)
                    if (MaxPlayer)
                        _playerASscore = _playerASscore + 10;
                    else
                        _playerBScore = _playerBScore + 10;

                string _input = _playerASscore.ToString() + "," + _playerBScore.ToString(); ;
                using (var file = new System.IO.StreamWriter(_pathToScoreCard, false))
                {
                    file.Write(_input);
                    file.Close();
                }
                _currentMaze.WriteMaze(_pathToGameState);
            }
        }
        
        public static int PillCountToScore(Maze _maze)
        {
            var _PILL_COUNT = 0;
            for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                {
                    var _symbol = _maze.GetSymbol(x, y);
                    if (_symbol.Equals(Symbols._PILL))
                        _PILL_COUNT++;
                    if(_symbol.Equals(Symbols._BONUS_PILL))
                        _PILL_COUNT = _PILL_COUNT+10;
                }
            }
            return _PILL_COUNT;
        }
    }
}
