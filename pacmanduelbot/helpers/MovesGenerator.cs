using pacmanduelbot.models;
using pacmanduelbot.shared;
using System.Collections.Generic;
using System.Drawing;

namespace pacmanduelbot.helpers
{
    public static class MovesGenerator
    {
        public static List<Point> GenerateNextPossiblePositions(char[][] maze, Point currentPoint)
        {
            var moveList = new List<Point>();
            if (currentPoint.Y + 1 < Guide._WIDTH
                && !maze[currentPoint.X][currentPoint.Y + 1].Equals(Guide._WALL)
                && !(currentPoint.X.Equals(Guide._FORBIDDEN_R_X) && currentPoint.Y.Equals(Guide._FORBIDDEN_R_Y - 1)))
                moveList.Add(new Point { X = currentPoint.X, Y = currentPoint.Y + 1 });

            if (currentPoint.Y - 1 >= 0
                && !maze[currentPoint.X][currentPoint.Y - 1].Equals(Guide._WALL)
                && !(currentPoint.X.Equals(Guide._FORBIDDEN_L_X) && currentPoint.Y.Equals(Guide._FORBIDDEN_L_Y + 1)))
                moveList.Add(new Point { X = currentPoint.X, Y = currentPoint.Y - 1 });

            if (currentPoint.X + 1 < Guide._HEIGHT
                && !maze[currentPoint.X + 1][currentPoint.Y].Equals(Guide._WALL)
                && !(currentPoint.X.Equals(Guide._EXIT_UP_X - 1) && currentPoint.Y.Equals(Guide._EXIT_UP_Y))
                && !(currentPoint.X.Equals(Guide._RESPAWN_X - 1) && currentPoint.Y.Equals(Guide._RESPAWN_Y)))
            {
                if ((currentPoint.X.Equals(Guide._RESPAWN_X) && currentPoint.Y.Equals(Guide._RESPAWN_Y))
                    && maze[currentPoint.X + 1][currentPoint.Y].Equals(Guide._OPONENT_SYMBOL))
                {
                    //DO NOTHING
                    //moveList.Add(new Point { X = Guide._EXIT_UP_X, Y = Guide._EXIT_UP_Y });
                }
                else
                {
                    moveList.Add(new Point { X = currentPoint.X + 1, Y = currentPoint.Y });
                }
            }

            if (currentPoint.X - 1 >= 0
                && !maze[currentPoint.X - 1][currentPoint.Y].Equals(Guide._WALL)
                && !(currentPoint.X.Equals(Guide._EXIT_DOWN_X + 1) && currentPoint.Y.Equals(Guide._EXIT_DOWN_Y))
                && !(currentPoint.X.Equals(Guide._RESPAWN_X + 1) && currentPoint.Y.Equals(Guide._RESPAWN_Y)))
            {
                if ((currentPoint.X.Equals(Guide._RESPAWN_X) && currentPoint.Y.Equals(Guide._RESPAWN_Y))
                    && maze[currentPoint.X - 1][currentPoint.Y].Equals(Guide._OPONENT_SYMBOL))
                {
                    //DO NOTHING
                    //moveList.Add(new Point { X = Guide._EXIT_DOWN_X, Y = Guide._EXIT_DOWN_Y });
                }
                else
                {
                    moveList.Add(new Point { X = currentPoint.X - 1, Y = currentPoint.Y });
                }
            }

            if (currentPoint.X.Equals(Guide._PORTAL1_X) && currentPoint.Y.Equals(Guide._PORTAL1_Y))
                moveList.Add(new Point { X = Guide._PORTAL2_X, Y = Guide._PORTAL2_Y });

            if (currentPoint.X.Equals(Guide._PORTAL2_X) && currentPoint.Y.Equals(Guide._PORTAL2_Y))
                moveList.Add(new Point { X = Guide._PORTAL1_X, Y = Guide._PORTAL1_Y });

            return moveList;
        }

        public static Point BuildPath(char[][] _maze, Point _start, Point _goal)
        {
            var _next = new Point();

            var _open = new LinkedList();

            var _gG = 0;
            var _hH = Mappings.ManhattanDistance(_start, _goal);
            var _fF = _gG + _hH;
            _open.Append(_start, _gG, _hH, _fF, null);

            var _closed = new LinkedList();

            var _found = false;


            while (!_found)
            {
                var _current = LowRank(_open);
                _closed.Append(_current._position, _current._g, _current._h, _current._f, _current._parent);
                _open.Delete(_current);
                if ((_current._position.X == _goal.X)
                    && (_current._position.Y == _goal.Y))
                {
                    _found = true;
                    var temp = new LinkedList();
                    while (_current._parent != null)
                    {
                        temp.Insert(_current);
                        _current = _current._parent;
                    }
                    _next = temp.Last._position;
                }

                var _neighbors = GenerateNextPossiblePositions(_maze, _current._position);

                for (var i = 0; i < _neighbors.Count; i++)
                {
                    _gG = _current._g + 1;
                    _hH = Mappings.ManhattanDistance(_neighbors[i], _goal);
                    _fF = _gG + _hH;
                    var _curr = new LinkedList.Node { _position = _neighbors[i], _g = _gG, _h = _hH, _f = _fF };
                    if (!_closed.contains(_curr))
                    {
                        _open.Append(_neighbors[i], _gG, _hH, _fF, _current);
                    }

                }

            }

            return _next;
        }

        private static LinkedList.Node LowRank(LinkedList _nodes)
        {
            var _result = new LinkedList.Node();
            var _curr = new LinkedList.Node();

            if (_nodes.isEmpty())
            {
                _result = null;
            }
            else
            {
                _result = _nodes.First;
                _curr = _result._next;

                while (_curr != null)
                {
                    if (_curr._f < _result._f)
                    {
                        _result = _curr;
                    }
                    _curr = _curr._next;
                }

            }
            return _result;
        }

        public static Point ChoosePath(char[][] _maze, Point _current_position)
        {
            var _next = new Point();
            var _open = new List<Point>();
            var _closed = new List<Point>();
            _open.Add(_current_position);

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

    }
}
