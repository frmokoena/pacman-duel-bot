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
            + "scorecard.csv";
        public static readonly string _pathToGameState = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar
            + "pacmanduelbot" + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar
            + "game.state";

        //public static readonly string _pathToScoreCard = ".." + System.IO.Path.DirectorySeparatorChar + ".."
        //    + System.IO.Path.DirectorySeparatorChar + ".." + System.IO.Path.DirectorySeparatorChar + "pacmanduelbot"
        //    + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar + "scorecard.csv";

        //public static readonly string _pathToGameState = ".." + System.IO.Path.DirectorySeparatorChar + ".."
        //    + System.IO.Path.DirectorySeparatorChar + ".." + System.IO.Path.DirectorySeparatorChar + "pacmanduelbot"
        //    + System.IO.Path.DirectorySeparatorChar + "store" + System.IO.Path.DirectorySeparatorChar + "game.state";


        public static void CleanScoreCard(Maze _maze)
        {
            string _input;
            if (PillCountToScore(_maze) == Properties.Settings.Default._MazeTotalPillCount - 1)
                _input = "0,1,0";
            else
                _input = "0,0,0";
            using (var file = new System.IO.StreamWriter(_pathToScoreCard, false))
            {
                file.Write(_input);
                file.Close();
            }
            _maze.WriteMaze(_pathToGameState);
        }

        public static int GetPlayerAScore()
        {
            int _PlayerAScore;
            var _fileContests = new string[3];
            try
            {
                var _index = 0;
                var _input = System.IO.File.ReadAllText(_pathToScoreCard);
                foreach (var column in Regex.Split(_input, ","))
                {
                    _fileContests[_index] = column;
                    _index++;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            bool parsed = Int32.TryParse(_fileContests[0], out _PlayerAScore);
            if (!parsed)
                return -1;
            return _PlayerAScore;
        }

        public static int GetPlayerBScore()
        {
            int _PlayerBScore;
            var _fileContents = new string[3];
            try
            {
                var _input = System.IO.File.ReadAllText(_pathToScoreCard);
                var _index = 0;
                foreach (var column in Regex.Split(_input, ","))
                {
                    _fileContents[_index] = column;
                    _index++;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            bool parsed = Int32.TryParse(_fileContents[1], out _PlayerBScore);
            if (!parsed)
                return -1;
            return _PlayerBScore;
        }

        public static int GetTurnsWithNoPointScored()
        {
            int _TurnsWithNoPointScored;
            var _fileContents = new string[3];
            try
            {
                var _input = System.IO.File.ReadAllText(_pathToScoreCard);
                var _index = 0;
                foreach (var column in Regex.Split(_input, ","))
                {
                    _fileContents[_index] = column;
                    _index++;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            bool parsed = Int32.TryParse(_fileContents[2], out _TurnsWithNoPointScored);
            if (!parsed)
                return -1;
            return _TurnsWithNoPointScored;
        }

        public static void UpdateScore(Maze _maze, bool PlayerA)
        {
            var _previousMaze = new Maze(_pathToGameState);
            var _previousPillCountToScore = PillCountToScore(_previousMaze);

            var _currentMaze = new Maze(_maze);
            var _currentPillCountToScore = PillCountToScore(_currentMaze);

            var _playerAScore = GetPlayerAScore();
            var _playerBScore = GetPlayerBScore();
            var _TurnsWithNoPointScored = GetTurnsWithNoPointScored();

            if (_currentPillCountToScore == _previousPillCountToScore)
            {
                if (_currentPillCountToScore != Properties.Settings.Default._MazeTotalPillCount
                && _currentPillCountToScore != Properties.Settings.Default._MazeTotalPillCount - 1)
                {
                    _TurnsWithNoPointScored++;
                    string _input = _playerAScore.ToString() + "," + _playerBScore.ToString() + "," + _TurnsWithNoPointScored.ToString();
                    using (var file = new System.IO.StreamWriter(_pathToScoreCard, false))
                    {
                        file.Write(_input);
                        file.Close();
                    }
                }
            }
            else
            {
                _TurnsWithNoPointScored = 0;
                if (PlayerA)
                {
                    _playerAScore = (_currentPillCountToScore == _previousPillCountToScore - 1) ? _playerAScore += 1 : _playerAScore += 10;
                }
                else
                {
                    _playerBScore = (_currentPillCountToScore == _previousPillCountToScore - 1) ? _playerBScore+=1 : _playerBScore+=10;
                }

                string _input = _playerAScore.ToString() + "," + _playerBScore.ToString() + "," + _TurnsWithNoPointScored.ToString();
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
                    if (_symbol.Equals(Symbols._BONUS_PILL))
                        _PILL_COUNT = _PILL_COUNT + 10;
                }
            }
            return _PILL_COUNT;
        }
    }
}