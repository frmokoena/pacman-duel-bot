using pacmanduelbot.helpers;
using pacmanduelbot.models;
using pacmanduelbot.shared;
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
                var _next_position = NextMove();
                
                if (needSelfRespawn())
                    return SelfRespawn(_next_position);

                if (_DROP_PILL)
                    return MakeMoveAndDropPill(_next_position);

                _maze[_CURRENT_POSITION.X][_CURRENT_POSITION.Y] = ' ';
                _maze[_next_position.X][_next_position.Y] = Guide._PLAYER_SYMBOL;
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
                    break;
                }
            }

            if (!_next.IsEmpty)
            {
                _maze[_CURRENT_POSITION.X][_CURRENT_POSITION.Y] = ' ';
                _maze[_next.X][_next.Y] = Guide._PLAYER_SYMBOL;
                PoisonInventory.EmptyPoisonInventory();
                return _maze;
            }

            _maze[_CURRENT_POSITION.X][_CURRENT_POSITION.Y] = ' ';
            _maze[_move.X][_move.Y] = Guide._PLAYER_SYMBOL;
            PoisonInventory.EmptyPoisonInventory();
            return _maze;
        }

        public bool needSelfRespawn()
        {
            var result = PoisonInventory.isSelfRespawn();
            return result;
        }

        public Point NextMove()
        {
            var _move = new Point();
            var list = new List<Point>();

            var _next = FindNearbyPill();
            if (_maze[_next.X][_next.Y].Equals(Guide._BONUS_PILL))
                return MovesGenerator.BuildPath(_maze, _CURRENT_POSITION, _next);

            var possibleMoveList = MovesGenerator.GenerateNextPossiblePositions(_maze, _CURRENT_POSITION);

            for (var i = 0; i < possibleMoveList.Count; i++)
            {
                var _Maze_symbol = _maze[possibleMoveList[i].X][possibleMoveList[i].Y];
                if (_Maze_symbol.Equals(Guide._PILL)
                    || _Maze_symbol.Equals(Guide._BONUS_PILL))
                    list.Add(new Point { X = possibleMoveList[i].X, Y = possibleMoveList[i].Y });

            }

            switch(list.Count)
            {
                case 0:
                    var _goal = FindNearbyPill();

                    if (!PoisonInventory.arePoisonPillsExhausted()
                        && !(_CURRENT_POSITION.X == Guide._RESPAWN_X && _CURRENT_POSITION.Y==Guide._RESPAWN_Y))
                    {
                        var _mapping = Mappings.ManhattanDistance(_CURRENT_POSITION, _goal);
                        if (_mapping > 10)
                            _DROP_PILL = true;
                    }

                    _move = MovesGenerator.BuildPath(_maze, _CURRENT_POSITION, _goal);
                    break;
                case 1:
                    _move = list[0];
                    break;
                default:
                    _move = MovesGenerator.ChoosePath(_maze, _CURRENT_POSITION,10);
                    break;
            }
            return _move;
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
                        if (tempH < 8)
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

        public int PillCount()
        {
            var _pill_count = 0;
            for (var x = 0; x < Guide._HEIGHT; x++)
            {
                for (var y = 0; y < Guide._WIDTH; y++)
                {
                    if (_maze[x][y].Equals(Guide._PILL)
                        || _maze[x][y].Equals(Guide._BONUS_PILL))
                        _pill_count++;
                }
            }
            return _pill_count;
        }
    }
}
