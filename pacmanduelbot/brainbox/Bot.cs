using pacmanduelbot.helpers;
using pacmanduelbot.models;
using pacmanduelbot.shared;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace pacmanduelbot.brainbox
{
    class Bot
    {
        public Maze _maze { get; set; }
        private bool _DROP_POISON_PILL { get; set; }
        private bool _KILL_PLAYER_B { get; set; }

        public Maze MakeMove()
        {
            if (!_PLAYER_A_POSITION.IsEmpty)
            {
                ScoreKeeper.UpdateScore(_maze, false);
                var _nextMove = DetermineNextMove();
                
                if (PoisonBucket.IsSelfRespawnNeeded())
                    return MakeMoveAndRespawn(_nextMove);

                if (_KILL_PLAYER_B)
                    return MakeMoveAndKillPlayerB(_nextMove);

                if (_DROP_POISON_PILL)
                    return MakeMoveAndDropPoisonPill(_nextMove);

                _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
                _maze.SetSymbol(_nextMove, Symbols._PLAYER_A);
                ScoreKeeper.UpdateScore(_maze, true);
            }
            return _maze;
        }

        private Maze MakeMoveAndDropPoisonPill(Point _nextMove)
        {
            PoisonBucket.DropPoisonPill();
            _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._POISON_PILL);
            _maze.SetSymbol(_nextMove, Symbols._PLAYER_A);
            ScoreKeeper.UpdateScore(_maze, true);
            return _maze;
        }

        private Maze MakeMoveAndRespawn(Point _nextMove)
        {
            var _pointlist = MovesGenerator.GenerateMoves(_maze, _PLAYER_A_POSITION);
            foreach (var _point in _pointlist)
            {
                if (_maze.GetSymbol(_point).Equals(Symbols._POISON_PILL))
                {
                    PoisonBucket.EmptyPoisonBucket();
                    _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
                    _maze.SetSymbol(_point, Symbols._PLAYER_A);
                    ScoreKeeper.UpdateScore(_maze, true);
                    return _maze;
                }
            }
            //TODO: maybe op left hers
            var _poisonPill = FindNearbyPoisonPill();
            if (!_poisonPill.IsEmpty)
            {
                var _move = MovesGenerator.FindPathToPill(_maze, _PLAYER_A_POSITION, _poisonPill);
                _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
                _maze.SetSymbol(_move[1], Symbols._PLAYER_A);
                ScoreKeeper.UpdateScore(_maze, true);
                return _maze;
            }

            //tough luck
            PoisonBucket.EmptyPoisonBucket();
            _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
            _maze.SetSymbol(_nextMove, Symbols._PLAYER_A);
            ScoreKeeper.UpdateScore(_maze, true);
            return _maze;
        }

        private Maze MakeMoveAndKillPlayerB(Point _nextMove)
        {
            var _TurnsWithNoPointScored = ScoreKeeper.GetTurnsWithNoPointScored();//adjust for when a pill is far
            var _playerAScore = ScoreKeeper.GetPlayerAScore();
            var _playerBScore = ScoreKeeper.GetPlayerBScore();

            var moveList = MovesGenerator.GenerateMoves(_maze, _PLAYER_A_POSITION);
            if (_playerAScore > _playerBScore
                || _TurnsWithNoPointScored < Properties.Settings.Default._MaxTurnsWithNoPointsScored / 4)
            {
                foreach (var _point in moveList)
                {
                    if (_maze.GetSymbol(_point).Equals(Symbols._PLAYER_B))
                    {
                        _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
                        _maze.SetSymbol(_point, Symbols._PLAYER_A);
                        ScoreKeeper.UpdateScore(_maze, true);
                        return _maze;
                    }
                }
            }

            _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
            _maze.SetSymbol(_nextMove, Symbols._PLAYER_A);
            ScoreKeeper.UpdateScore(_maze, true);
            return _maze;
        }

        private Point DetermineNextMove()
        {
            var _move = new List<Point>();
            var _next = FindNearbyPill();

            if (_maze.GetSymbol(_next).Equals(Symbols._BONUS_PILL))
            {
                _move = MovesGenerator.FindPathToPill(_maze, _PLAYER_A_POSITION, _next);
                return _move[1];
            }

            var possibleMoveList = MovesGenerator.GenerateMoves(_maze, _PLAYER_A_POSITION);
            var list = new List<Point>();
            foreach (var _point in possibleMoveList)
            {
                if (_maze.GetSymbol(_point).Equals(Symbols._PILL))
                {
                    list.Add(_point);
                    if (list.Count > 1) break;
                }
            }

            switch (list.Count)
            {
                case 0:
                    _move = MovesGenerator.FindPathToPill(_maze, _PLAYER_A_POSITION, _next);
                    if (!PoisonBucket.IsPoisonBucketEmpty()
                        && !(_PLAYER_A_POSITION.X == Properties.Settings.Default._MazeRespawnX && _PLAYER_A_POSITION.Y == Properties.Settings.Default._MazeRespawnY)
                        && !(_PLAYER_A_POSITION.X == Properties.Settings.Default._MazeRespawnExitUpX && _PLAYER_A_POSITION.Y == Properties.Settings.Default._MazeRespawnExitUpY)
                        && !(_PLAYER_A_POSITION.X == Properties.Settings.Default._MazeRespawnExitDownX && _PLAYER_A_POSITION.Y == Properties.Settings.Default._MazeRespawnExitDownY))
                    {
                        var _gd = _move[0].X;
                        var _temp = MovesGenerator.FindPathToPill(_maze, new Point { X = Properties.Settings.Default._MazeRespawnX, Y = Properties.Settings.Default._MazeRespawnY }, _next);
                        var _gr = _temp[0].X + 1;

                        if (_gr < _gd)
                            _DROP_POISON_PILL = true;
                    }
                    return _move[1];
                case 1:
                    return list[0];
                default:
                    return MovesGenerator.PathSelect(_maze, _PLAYER_A_POSITION, 1000);
            }
        }

        private Point FindNearbyPill()
        {
            var _next = new Point();
            var _open = new List<Point>
            {
                _PLAYER_A_POSITION
            };
            var _closed = new List<Point>();
            var _explored = new List<Point>();

            if (!AreBonusPillExhausted())
            {
                for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
                {
                    for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                    {
                        var _symbol = _maze.GetSymbol(x, y);
                        if (_symbol.Equals(Symbols._BONUS_PILL))
                        {
                            var _costToBonus = MovesGenerator.FindPathToPill(_maze, _PLAYER_A_POSITION, new Point { X = x, Y = y });
                            if (_costToBonus[0].X < 10)
                                return new Point { X = x, Y = y };
                        }
                    }
                }
            }

            //TODO: Capitalize
            int _playerAcost = 0, _playerBcost = 0;
            Point _playerBtarget;
            if ((_PLAYER_A_POSITION.X > Properties.Settings.Default._MazeTunnel
                && ScoreKeeper.GetPlayerAScore() + _UPPER_PILL_COUNT_TO_SCORE > Properties.Settings.Default._MazeDrawScore)
                || (_PLAYER_B_POSITION.X > Properties.Settings.Default._MazeTunnel
                && ScoreKeeper.GetPlayerBScore() + _UPPER_PILL_COUNT_TO_SCORE > Properties.Settings.Default._MazeDrawScore))
            {
                //TODO: Stay up there
                _KILL_PLAYER_B = true;
                _playerBtarget = new Point();
                while (_open.Count != 0)
                {
                    _closed.Add(_open[0]);
                    var _templist = MovesGenerator.GenerateMoves(_maze, _open[0]);
                    foreach (var _point in _templist)
                    {
                        if (_point.X > Properties.Settings.Default._MazeTunnel)
                        {
                            if (!_explored.Contains(_point))
                            {
                                if (_maze.GetSymbol(_point).Equals(Symbols._PILL) || _maze.GetSymbol(_point).Equals(Symbols._BONUS_PILL))
                                {
                                    //TODO:                        
                                    if (!_PLAYER_B_POSITION.IsEmpty)
                                    {
                                        _playerAcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_A_POSITION, _point)[0].X;
                                        if (_playerBtarget.IsEmpty)
                                        {
                                            _playerBtarget = _point;
                                            _playerBcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_B_POSITION, _point)[0].X;
                                        }
                                        else
                                        {
                                            _playerBcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_B_POSITION, _playerBtarget)[0].X
                                            + MovesGenerator.FindPathToPill(_maze, _playerBtarget, _point)[0].X;
                                        }
                                        if (_playerAcost <= _playerBcost)
                                            return _point;
                                        _playerBtarget = _point;
                                    }
                                    else
                                    {
                                        return _point;
                                    }
                                }
                                _explored.Add(_point);
                            }
                            if (!_closed.Contains(_point))
                                _open.Add(_point);
                        }
                    }
                    _open.Remove(_open[0]);
                }
                _open.Clear(); _open.Add(_PLAYER_A_POSITION);
                _closed.Clear(); _explored.Clear();
            }

            if ((_PLAYER_A_POSITION.X <= Properties.Settings.Default._MazeTunnel
                && ScoreKeeper.GetPlayerAScore() + _LOWER_PILL_COUNT_TO_SCORE > Properties.Settings.Default._MazeDrawScore)
                || (_PLAYER_B_POSITION.X <= Properties.Settings.Default._MazeTunnel
                && ScoreKeeper.GetPlayerBScore() + _LOWER_PILL_COUNT_TO_SCORE > Properties.Settings.Default._MazeDrawScore))
            {
                //TODO: Stay down there
                _KILL_PLAYER_B = true;
                _playerBtarget = new Point();
                while (_open.Count != 0)
                {
                    _closed.Add(_open[0]);
                    var _templist = MovesGenerator.GenerateMoves(_maze, _open[0]);
                    foreach (var _point in _templist)
                    {
                        if (_point.X <= Properties.Settings.Default._MazeTunnel)
                        {
                            if (!_explored.Contains(_point))
                            {
                                if (_maze.GetSymbol(_point).Equals(Symbols._PILL) || _maze.GetSymbol(_point).Equals(Symbols._BONUS_PILL))
                                {
                                    //TODO:                        
                                    if (!_PLAYER_B_POSITION.IsEmpty)
                                    {
                                        _playerAcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_A_POSITION, _point)[0].X;
                                        if (_playerBtarget.IsEmpty)
                                        {
                                            _playerBtarget = _point;
                                            _playerBcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_B_POSITION, _point)[0].X;
                                        }
                                        else
                                        {
                                            _playerBcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_B_POSITION, _playerBtarget)[0].X
                                            + MovesGenerator.FindPathToPill(_maze, _playerBtarget, _point)[0].X;
                                        }
                                        if (_playerAcost <= _playerBcost)
                                            return _point;
                                        _playerBtarget = _point;
                                    }
                                    else
                                    {
                                        return _point;
                                    }
                                }
                                _explored.Add(_point);
                            }
                            if (!_closed.Contains(_point))
                                _open.Add(_point);
                        }
                    }
                    _open.Remove(_open[0]);
                }
                _open.Clear(); _open.Add(_PLAYER_A_POSITION);
                _closed.Clear(); _explored.Clear();
            }

            _playerBtarget = new Point();
            while (_open.Count != 0)
            {
                _closed.Add(_open[0]);
                var _templist = MovesGenerator.GenerateMoves(_maze, _open[0]);
                foreach (var _point in _templist)
                {
                    if (!_explored.Contains(_point))
                    {
                        if (_maze.GetSymbol(_point).Equals(Symbols._PILL) || _maze.GetSymbol(_point).Equals(Symbols._BONUS_PILL))
                        {
                            //TODO:                        
                            if (!_PLAYER_B_POSITION.IsEmpty)
                            {
                                _playerAcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_A_POSITION, _point)[0].X;
                                if (_playerBtarget.IsEmpty)
                                {
                                    _playerBtarget = _point;
                                    _playerBcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_B_POSITION, _point)[0].X;
                                }
                                else
                                {
                                    _playerBcost = MovesGenerator.FindPathToPill(_maze, _PLAYER_B_POSITION, _playerBtarget)[0].X
                                    + MovesGenerator.FindPathToPill(_maze, _playerBtarget, _point)[0].X;
                                }
                                if (_playerAcost <= _playerBcost)
                                    return _point;
                                _playerBtarget = _point;
                            }
                            else
                            {
                                return _point;
                            }
                        }
                        _explored.Add(_point);
                    }
                    if (!_closed.Contains(_point))
                        _open.Add(_point);
                }
                _open.Remove(_open[0]);
            }

            //I give up
            _open.Clear(); _open.Add(_PLAYER_A_POSITION);
            _closed.Clear(); _explored.Clear();
            while (_open.Count != 0)
            {
                _closed.Add(_open[0]);
                var _templist = MovesGenerator.GenerateMoves(_maze, _open[0]);
                foreach (var _point in _templist)
                {
                    if (!_explored.Contains(_point))
                    {
                        if (_maze.GetSymbol(_point).Equals(Symbols._PILL) || _maze.GetSymbol(_point).Equals(Symbols._BONUS_PILL))
                            return _point;
                        _explored.Add(_point);
                    }
                    if (!_closed.Contains(_point))
                        _open.Add(_point);
                }
                _open.Remove(_open[0]);
            }
            return _next;
        }

        private bool AreBonusPillExhausted()
        {
            return !_maze.GetSymbol(Properties.Settings.Default._MazeBonusPillA).Equals(Symbols._BONUS_PILL)
                && !_maze.GetSymbol(Properties.Settings.Default._MazeBonusPillB).Equals(Symbols._BONUS_PILL)
                && !_maze.GetSymbol(Properties.Settings.Default._MazeBonusPillC).Equals(Symbols._BONUS_PILL)
                && !_maze.GetSymbol(Properties.Settings.Default._MazeBonusPillD).Equals(Symbols._BONUS_PILL);
        }

        private Point FindNearbyPoisonPill()
        {
            var _open = new List<Point>
            {
                _PLAYER_A_POSITION
            };
            var _closed = new List<Point>();
            var _explored = new List<Point>();

            while (_open.Count != 0)
            {
                _closed.Add(_open[0]);
                var _templist = MovesGenerator.GenerateMoves(_maze, _open[0]);
                foreach (var _point in _templist)
                {
                    if (!_explored.Contains(_point))
                    {
                        if (_maze.GetSymbol(_point).Equals(Symbols._POISON_PILL))
                        {
                            var _cost = MovesGenerator.FindPathToPill(_maze, _PLAYER_A_POSITION, _point)[0].X;
                            if (_cost < 4)
                                return _point;
                            else
                                return new Point();
                        }
                        _explored.Add(_point);
                    }
                    if (!_closed.Contains(_point))
                        _open.Add(_point);
                }
                _open.Remove(_open[0]);
            }
            return new Point();
        }

        private Point _PLAYER_A_POSITION
        {
            get
            {

                return _maze.FindCoordinateOf(Symbols._PLAYER_A);
            }
        }

        private Point _PLAYER_B_POSITION
        {
            get
            {
                return _maze.FindCoordinateOf(Symbols._PLAYER_B);
            }
        }

        private int _LOWER_PILL_COUNT_TO_SCORE
        {
            get
            {
                var _PILL_COUNT = 0;
                for (var x = 0; x < Properties.Settings.Default._MazeTunnel; x++)
                {
                    for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                    {
                        var _symbol = _maze.GetSymbol(x, y);
                        if (_symbol.Equals(Symbols._PILL))
                            _PILL_COUNT++;
                        if (_symbol.Equals(Symbols._BONUS_PILL))
                            _PILL_COUNT += 10;
                    }
                }
                return _PILL_COUNT;
            }
        }

        private int _UPPER_PILL_COUNT_TO_SCORE
        {
            get
            {
                var _PILL_COUNT = 0;
                for (var x = Properties.Settings.Default._MazeTunnel + 1; x < Properties.Settings.Default._MazeHeight; x++)
                {
                    for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
                    {
                        var _symbol = _maze.GetSymbol(x, y);
                        if (_symbol.Equals(Symbols._PILL))
                            _PILL_COUNT++;
                        if (_symbol.Equals(Symbols._BONUS_PILL))
                            _PILL_COUNT += 10;
                    }
                }
                return _PILL_COUNT;
            }
        }     
    }
}