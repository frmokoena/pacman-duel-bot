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
        private bool _DROP_PILL { get; set; }

        public Maze MakeMove()
        {
            if (!_PLAYER_A_POSITION.IsEmpty)
            {
                ScoreCard.UpdateScore(_maze, false);
                var _next_position = BotMove();

                if (PoisonBucket.IsSelfRespawnNeeded())
                    return SelfRespawn(_next_position);

                if (_DROP_PILL)
                    return MakeMoveAndDropPoisonPill(_next_position);

                _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
                _maze.SetSymbol(_next_position, Symbols._PLAYER_A);
                ScoreCard.UpdateScore(_maze, true);
            }
            return _maze;
        }

        private Maze MakeMoveAndDropPoisonPill(Point _move)
        {
            PoisonBucket.DropPoisonPill();
            _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._POISON_PILL);
            _maze.SetSymbol(_move, Symbols._PLAYER_A);
            return _maze;
        }

        private Maze SelfRespawn(Point _move)
        {
            var _pointlist = Moves.GenerateMoves(_maze, _PLAYER_A_POSITION);
            foreach (var _point in _pointlist)
            {
                if (_maze.GetSymbol(_point).Equals(Symbols._POISON_PILL))
                {
                    PoisonBucket.EmptyPoisonBucket();
                    _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
                    _maze.SetSymbol(_point, Symbols._PLAYER_A);
                    return _maze;
                }
            }
            //TODO: maybe op left hers
            var _poisonPill = FindNearbyPill(true);
            if (!_poisonPill.IsEmpty)
            {
                var _nextMove = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, _poisonPill);
                _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
                _maze.SetSymbol(_nextMove[1], Symbols._PLAYER_A);
                return _maze;
            }

            //tough luck
            PoisonBucket.EmptyPoisonBucket();
            _maze.SetSymbol(_PLAYER_A_POSITION, Symbols._EMPTY);
            _maze.SetSymbol(_move, Symbols._PLAYER_A);
            return _maze;
        }

        private Point BotMove()
        {
            var _move = new List<Point>();
            var _next = FindNearbyPill(false);

            if (_maze.GetSymbol(_next).Equals(Symbols._BONUS_PILL))
            {
                _move = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, _next);
                return _move[1];
            }

            //var _decide = Moves.BuildPath(_maze, _CURRENT_POSITION, _next);
            //if (_decide[0].X < 4)
            //  return Moves.MinMaxDecision(_maze, _CURRENT_POSITION, _OPPONENT_POSITION);

            var possibleMoveList = Moves.GenerateMoves(_maze, _PLAYER_A_POSITION);
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
                    _move = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, _next);
                    if (!PoisonBucket.IsPoisonBucketEmpty()
                        && !(_PLAYER_A_POSITION.X == Properties.Settings.Default._MazeRespawnX && _PLAYER_A_POSITION.Y == Properties.Settings.Default._MazeRespawnY)
                        && !(_PLAYER_A_POSITION.X == Properties.Settings.Default._MazeRespawnExitUpX && _PLAYER_A_POSITION.Y == Properties.Settings.Default._MazeRespawnExitUpY)
                        && !(_PLAYER_A_POSITION.X == Properties.Settings.Default._MazeRespawnExitDownX && _PLAYER_A_POSITION.Y == Properties.Settings.Default._MazeRespawnExitDownY))
                    {
                        var _gd = _move[0].X;
                        var _temp = Moves.FindPathToPill(_maze, new Point { X = Properties.Settings.Default._MazeRespawnX, Y = Properties.Settings.Default._MazeRespawnY }, _next);
                        var _gr = _temp[0].X + 5;

                        if (_gr < _gd)
                            _DROP_PILL = true;
                    }
                    return _move[1];
                case 1:
                    return list[0];
                default:
                    return Moves.PathSelect(_maze, _PLAYER_A_POSITION, 1000);
            }
        }

        private Point FindNearbyPill(bool NeedPoisonPill)
        {
            var _next = new Point();
            var _open = new List<Point>
            {
                _PLAYER_A_POSITION
            };
            var _closed = new List<Point>();
            var _explored = new List<Point>();

            if (NeedPoisonPill)
            {
                while (_open.Count != 0)
                {
                    _closed.Add(_open[0]);
                    var _templist = Moves.GenerateMoves(_maze, _open[0]);
                    foreach (var _point in _templist)
                    {
                        if (!_explored.Contains(_point))
                        {
                            if (_maze.GetSymbol(_point).Equals(Symbols._POISON_PILL))
                            {
                                var _cost = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, _point)[0].X;
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

            //for (var x = 0; x < Properties.Settings.Default._MazeHeight; x++)
            //{
            //    for (var y = 0; y < Properties.Settings.Default._MazeWidth; y++)
            //    {
            //        if (_maze.GetSymbol(x, y).Equals(Symbols._BONUS_PILL))
            //        {
            //            _next = new Point { X = x, Y = y };
            //            var _temp = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, _next);
            //            var _tempg = _temp[0].X;
            //            if (_tempg < 10)
            //                return _next;
            //        }
            //    }
            //}

            if (!AreBonusPillExhausted())
            {
                //TODO:
                var _playerAcostToBonusPill = 0;

                //PILL_A
                if(_BONUS_PILL_A.Equals(Symbols._BONUS_PILL))
                {
                    _playerAcostToBonusPill = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, Properties.Settings.Default._MazeBonusPillA)[0].X;
                    if (_playerAcostToBonusPill < 10)
                        return Properties.Settings.Default._MazeBonusPillA;
                }

                //PILL_B
                if (_BONUS_PILL_B.Equals(Symbols._BONUS_PILL))
                {
                    _playerAcostToBonusPill = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, Properties.Settings.Default._MazeBonusPillB)[0].X;
                    if (_playerAcostToBonusPill < 10)
                        return Properties.Settings.Default._MazeBonusPillB;
                }

                //PILL_C
                if (_BONUS_PILL_C.Equals(Symbols._BONUS_PILL))
                {
                    _playerAcostToBonusPill = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, Properties.Settings.Default._MazeBonusPillC)[0].X;
                    if (_playerAcostToBonusPill < 10)
                        return Properties.Settings.Default._MazeBonusPillC;
                }

                //PILL_D
                if (_BONUS_PILL_D.Equals(Symbols._BONUS_PILL))
                {
                    _playerAcostToBonusPill = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, Properties.Settings.Default._MazeBonusPillD)[0].X;
                    if (_playerAcostToBonusPill < 10)
                        return Properties.Settings.Default._MazeBonusPillD;
                }
            }

            //TODO: Capitalize
            int _playerAcost = 0, _playerBcost = 0;
            Point _playerBtarget;
            if ((_PLAYER_A_POSITION.X > Properties.Settings.Default._MazeTunnel
                && ScoreCard.GetPlayerAScore() + _UPPER_PILL_COUNT_TO_SCORE > Properties.Settings.Default._MazeDrawScore)
                || (_PLAYER_B_POSITION.X > Properties.Settings.Default._MazeTunnel
                && ScoreCard.GetPlayerBScore() + _UPPER_PILL_COUNT_TO_SCORE > Properties.Settings.Default._MazeDrawScore))
            {
                //TODO: Stay up there
                _playerBtarget = new Point();
                while (_open.Count != 0)
                {
                    _closed.Add(_open[0]);
                    var _templist = Moves.GenerateMoves(_maze, _open[0]);
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
                                        _playerAcost = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, _point)[0].X;
                                        if (_playerBtarget.IsEmpty)
                                        {
                                            _playerBtarget = _point;
                                            _playerBcost = Moves.FindPathToPill(_maze, _PLAYER_B_POSITION, _point)[0].X;
                                        }
                                        else
                                        {
                                            _playerBcost = Moves.FindPathToPill(_maze, _PLAYER_B_POSITION, _playerBtarget)[0].X
                                            + Moves.FindPathToPill(_maze, _playerBtarget, _point)[0].X;
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
                && ScoreCard.GetPlayerAScore() + _LOWER_PILL_COUNT_TO_SCORE > Properties.Settings.Default._MazeDrawScore)
                || (_PLAYER_B_POSITION.X <= Properties.Settings.Default._MazeTunnel
                && ScoreCard.GetPlayerBScore() + _LOWER_PILL_COUNT_TO_SCORE > Properties.Settings.Default._MazeDrawScore))
            {
                //TODO: Stay down there
                _playerBtarget = new Point();
                while (_open.Count != 0)
                {
                    _closed.Add(_open[0]);
                    var _templist = Moves.GenerateMoves(_maze, _open[0]);
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
                                        _playerAcost = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, _point)[0].X;
                                        if (_playerBtarget.IsEmpty)
                                        {
                                            _playerBtarget = _point;
                                            _playerBcost = Moves.FindPathToPill(_maze, _PLAYER_B_POSITION, _point)[0].X;
                                        }
                                        else
                                        {
                                            _playerBcost = Moves.FindPathToPill(_maze, _PLAYER_B_POSITION, _playerBtarget)[0].X
                                            + Moves.FindPathToPill(_maze, _playerBtarget, _point)[0].X;
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
                var _templist = Moves.GenerateMoves(_maze, _open[0]);
                foreach (var _point in _templist)
                {
                    if (!_explored.Contains(_point))
                    {
                        if (_maze.GetSymbol(_point).Equals(Symbols._PILL) || _maze.GetSymbol(_point).Equals(Symbols._BONUS_PILL))
                        {
                            //TODO:                        
                            if (!_PLAYER_B_POSITION.IsEmpty)
                            {
                                _playerAcost = Moves.FindPathToPill(_maze, _PLAYER_A_POSITION, _point)[0].X;
                                if (_playerBtarget.IsEmpty)
                                {
                                    _playerBtarget = _point;
                                    _playerBcost = Moves.FindPathToPill(_maze, _PLAYER_B_POSITION, _point)[0].X;
                                }
                                else
                                {
                                    _playerBcost = Moves.FindPathToPill(_maze, _PLAYER_B_POSITION, _playerBtarget)[0].X
                                    + Moves.FindPathToPill(_maze, _playerBtarget, _point)[0].X;
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
                var _templist = Moves.GenerateMoves(_maze, _open[0]);
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

        private bool AreBonusPillExhausted()
        {
            return !_BONUS_PILL_A.Equals(Symbols._BONUS_PILL)
                && !_BONUS_PILL_B.Equals(Symbols._BONUS_PILL)
                && !_BONUS_PILL_C.Equals(Symbols._BONUS_PILL)
                && !_BONUS_PILL_D.Equals(Symbols._BONUS_PILL);
        }

        private char _BONUS_PILL_A
        {
            get
            {
                return _maze.GetSymbol(Properties.Settings.Default._MazeBonusPillA);
            }
        }

        private char _BONUS_PILL_B
        {
            get
            {
                return _maze.GetSymbol(Properties.Settings.Default._MazeBonusPillB);
            }
        }

        private char _BONUS_PILL_C
        {
            get
            {
                return _maze.GetSymbol(Properties.Settings.Default._MazeBonusPillC);
            }
        }

        private char _BONUS_PILL_D
        {
            get
            {
                return _maze.GetSymbol(Properties.Settings.Default._MazeBonusPillD);
            }
        }
    }
}