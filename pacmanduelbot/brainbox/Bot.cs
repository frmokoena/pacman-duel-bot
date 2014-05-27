using pacmanduelbot.helpers;
using pacmanduelbot.models;
using pacmanduelbot.shared;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace pacmanduelbot.brainbox
{
    public class Bot
    {
        private char[][] _maze { get; set; }
        public bool _DROP_PILL = false;

        public Bot() { }

        public Bot(char[][] _maze)
        {
            // TODO: Complete member initialization
            this._maze = _maze;
        }

        public Point _CURRENT_POSITION
        {
            get
            {
                var coordinate = new Point();
                for (var x = 0; x < Guide._HEIGHT; x++)
                {
                    for (var y = 0; y < Guide._WIDTH; y++)
                    {
                        if (_maze[x][y].Equals(Guide._PLAYER_SYMBOL))
                        {
                            coordinate.X = x;
                            coordinate.Y = y;
                        }
                    }
                }
                return coordinate;
            }
        }

        public char[][] MakeMove()
        {
            if (!_CURRENT_POSITION.IsEmpty)
            {
                var _move = NextMove();

                var random = new Random();
                var randomMoveIndex = random.Next(0, _move.Count);
                var movePoint = _move[randomMoveIndex];

                if (needSelfRespawn())
                    return SelfRespawn(movePoint);

                if (_DROP_PILL)
                    return MakeMoveAndDropPill(movePoint);

                _maze[_CURRENT_POSITION.X][_CURRENT_POSITION.Y] = ' ';
                _maze[movePoint.X][movePoint.Y] = Guide._PLAYER_SYMBOL;
                //_maze[_next.X][_next.Y] = Guide._PLAYER_SYMBOL;
            }

            return _maze;
        }

        public char[][] MakeMoveAndDropPill(Point _move)
        {
            _maze[_CURRENT_POSITION.X][_CURRENT_POSITION.Y] = Guide._POISON_PILL;
            _maze[_move.X][_move.Y] = Guide._PLAYER_SYMBOL;
            PoisonInventory.DropPoisonPill();

            return _maze;
        }

        public char[][] SelfRespawn(Point _move)
        {
            var _next = new Point();
            var list = MovesGenerator.GenerateNextPossiblePositions(_maze, _CURRENT_POSITION);
            for (var i = 0; i < list.Count; i++)
            {
                var _Maze_symbol = _maze[list[i].X][list[i].Y];
                if (_Maze_symbol.Equals(Guide._POISON_PILL))
                {
                    _next.X = list[i].X;
                    _next.Y = list[i].Y;
                }
            }

            if (!_next.IsEmpty)
            {
                _maze[_CURRENT_POSITION.X][_CURRENT_POSITION.Y] = ' ';
                _maze[_next.X][_next.Y] = Guide._PLAYER_SYMBOL;
                PoisonInventory.EmptyPoisonInventory();
                return _maze;
            }
            else
            {
                _maze[_CURRENT_POSITION.X][_CURRENT_POSITION.Y] = ' ';
                _maze[_move.X][_move.Y] = Guide._PLAYER_SYMBOL;
                PoisonInventory.EmptyPoisonInventory();
                return _maze;
            }
        }





        public bool needSelfRespawn()
        {
            var result = PoisonInventory.isSelfRespawn();
            return result;
        }

        public List<Point> NextMove()
        {
            var list = new List<Point>();
            var possibleMoveList = MovesGenerator.GenerateNextPossiblePositions(_maze, _CURRENT_POSITION);

            for (var i = 0; i < possibleMoveList.Count; i++)
            {
                var _Maze_symbol = _maze[possibleMoveList[i].X][possibleMoveList[i].Y];
                if (_Maze_symbol.Equals(Guide._PILL)
                    || _Maze_symbol.Equals(Guide._BONUS_PILL))
                    list.Add(new Point { X = possibleMoveList[i].X, Y = possibleMoveList[i].Y });

            }
            if (list.Count == 0)
            {
                var _goal = FindNearbyPill();

                if (!PoisonInventory.arePoisonPillsExhausted())
                {
                    var _mapping = Mappings.ManhattanDistance(_CURRENT_POSITION, _goal);
                    if (_mapping > 10)
                        _DROP_PILL = true;
                }

                var _next = MovesGenerator.BuildPath(_maze, _CURRENT_POSITION, _goal);
                list.Add(new Point { X = _next.X, Y = _next.Y });
            }

            return list;
        }

        public Point FindNearbyPill()
        {
            var _next = new Point();
            var _open = new List<Point>();
            var _closed = new List<Point>();
            _open.Add(_CURRENT_POSITION);


            for (var x = 0; x < Guide._HEIGHT; x++)
            {
                for (var y = 0; y < Guide._WIDTH; y++)
                {
                    if (_maze[x][y].Equals(Guide._BONUS_PILL))
                    {
                        _next = new Point { X = x, Y = y };
                        var tempH = Mappings.ManhattanDistance(_CURRENT_POSITION, _next);
                        if (tempH < 6)
                            return _next;

                    }
                }
            }


            while (_open.Count != 0)
            {
                var _templist = MovesGenerator.GenerateNextPossiblePositions(_maze, _open[0]);
                _closed.Add(_open[0]);
                for (var j = 0; j < _templist.Count; j++)
                {
                    if (_maze[_templist[j].X][_templist[j].Y] == Guide._BONUS_PILL
                        || _maze[_templist[j].X][_templist[j].Y] == Guide._PILL)
                    {
                        _next = _templist[j];
                        return _next;
                    }
                    if (!_closed.Contains(_templist[j]))
                        _open.Add(_templist[j]);

                }
                _open.Remove(_open[0]);

            }
            return _next;
        }

        private bool isRespawn()
        {
            var result = false;
            return result;
        }
    }
}
