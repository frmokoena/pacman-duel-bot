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
            var list = Moves.NextPossiblePositions(_maze, _CURRENT_POSITION);
            foreach (var _point in list)
            {
                var _Maze_symbol = _maze[_point.X][_point.Y];
                if (_Maze_symbol.Equals(Guide._POISON_PILL))
                {
                    _next = _point;
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
                return Moves.BuildPath(_maze, _CURRENT_POSITION, _next);

            var possibleMoveList = Moves.NextPossiblePositions(_maze, _CURRENT_POSITION);

            foreach (var _point in possibleMoveList)
            {
                var _Maze_symbol = _maze[_point.X][_point.Y];
                if (_Maze_symbol.Equals(Guide._PILL)
                    || _Maze_symbol.Equals(Guide._BONUS_PILL))
                    list.Add(_point);

            }

            switch(list.Count)
            {
                case 0:
                    //var _goal = FindNearbyPill();
                    if (!PoisonInventory.arePoisonPillsExhausted()
                        && !(_CURRENT_POSITION.X == Guide._RESPAWN_X && _CURRENT_POSITION.Y==Guide._RESPAWN_Y))
                    {
                        var _mapping = Mappings.ManhattanDistance(_CURRENT_POSITION, _next);
                        if (_mapping > 10)
                            _DROP_PILL = true;
                    }

                    _move = Moves.BuildPath(_maze, _CURRENT_POSITION, _next);
                    break;
                case 1:
                    _move = list[0];
                    break;
                default:
                    _move = Moves.ChoosePath(_maze, _CURRENT_POSITION,30);
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
                var _templist = Moves.NextPossiblePositions(_maze, _open[0]);
                _closed.Add(_open[0]);
                foreach (var _point in _templist)
                {
                    if (_maze[_point.X][_point.Y] == Guide._BONUS_PILL
                        || _maze[_point.X][_point.Y] == Guide._PILL)
                    {
                        _next = _point;
                        return _next;
                    }
                    if (!_closed.Contains(_point))
                        _open.Add(_point);

                }
                _open.Remove(_open[0]);

            }
            return _next;
        }

        public int PillCount()
        {
            var _PILL_COUNT = 0;
            for (var x = 0; x < Guide._HEIGHT; x++)
            {
                for (var y = 0; y < Guide._WIDTH; y++)
                {
                    if (_maze[x][y].Equals(Guide._PILL)
                        || _maze[x][y].Equals(Guide._BONUS_PILL))
                        _PILL_COUNT++;
                }
            }
            return _PILL_COUNT;
        }
    }
}
