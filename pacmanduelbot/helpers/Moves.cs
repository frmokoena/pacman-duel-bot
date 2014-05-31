using pacmanduelbot.models;
using pacmanduelbot.shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacmanduelbot.helpers
{
    public static class Moves
    {
        public static List<Point> NextPossiblePositions(char[][] maze, Point currentPoint)
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
                    //do nothing          
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
                    //do nothing
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

        public static Point ChoosePath(char[][] _maze, Point _current_position, int _depth)
        {
            var _next = new Point();
            var _open = new List<ChoosePathNode>();
            var _closed = new List<ChoosePathNode>();
            var _node = new ChoosePathNode { _position = _current_position };
            _open.Add(_node);
            var _count = 0;

            while (_open.Count != 0 && _count < _depth)
            {
                _closed.Add(_open[0]);

                var _tempI = NextPossiblePositions(_maze, _open[0]._position);
                
                foreach (var _point in  _tempI)
                {
                    if (_open[0]._parent != null)
                    {
                        if (!(_point.X == _open[0]._parent._position.X && _point.Y == _open[0]._parent._position.Y))
                        {
                            var _case = _maze[_point.X][_point.Y];

                            switch(_case)
                            {
                                case Guide._PILL:
                                    var _path_node = new ChoosePathNode
                                    { 
                                        _position = _point,
                                        _score=_open[0]._score + 1,
                                        _isLeaf=isLeaf(_maze,_point,_open[0]._position),
                                        _parent = _open[0]
                                    };
                                    _open.Add(_path_node);
                                    break;
                                case Guide._BONUS_PILL:
                                    _path_node = new ChoosePathNode
                                    {
                                        _position = _point,
                                        _score=_open[0]._score + 10,
                                        _isLeaf = isLeaf(_maze, _point, _open[0]._position),
                                        _parent = _open[0]
                                    };
                                    _open.Add(_path_node);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        var _case = _maze[_point.X][_point.Y];
                        switch (_case)
                        {
                            case Guide._PILL:
                                var _path_node = new ChoosePathNode
                                {
                                    _position = _point,
                                    _score = _open[0]._score + 1,
                                    _isLeaf = isLeaf(_maze,_point,_open[0]._position),
                                    _parent = _open[0]
                                };
                                _open.Add(_path_node);
                                break;
                            case Guide._BONUS_PILL:
                                _path_node = new ChoosePathNode
                                {
                                    _position = _point,
                                    _score = _open[0]._score + 10,
                                    _isLeaf = isLeaf(_maze,_point,_open[0]._position),
                                    _parent = _open[0]
                                };
                                _open.Add(_path_node);
                                break;
                            default:
                                break;
                        }
                    }
                }
                _open.Remove(_open[0]);
                _count++;
            }

            var curr = new ChoosePathNode();
                       
            foreach(var _item in _closed)
            {

                if(_item._isLeaf)
                {
                    if(curr._position.IsEmpty)
                    {
                        curr = _item;
                    }
                    else
                    {
                        if(_item._score > curr._score)
                        {
                            curr = _item;
                        }
                    }
                }
            }

            
            //traverse back
            if (curr._position.IsEmpty)
            {
                curr = _closed[_closed.Count - 1];
                while (!(curr._parent._position.X==_closed[0]._position.X&&curr._parent._position.Y==_closed[0]._position.Y))
                    curr = curr._parent;
                _next = curr._position;

            }
            else
            {
                while (!(curr._parent._position.X == _closed[0]._position.X && curr._parent._position.Y == _closed[0]._position.Y))
                    curr = curr._parent;
                _next = curr._position;
            }


            return _next;
        }

        private static bool isLeaf(char[][] _maze,Point _point, Point _parent)
        {
            var isLeaf = true;
            var _list = NextPossiblePositions(_maze, _point);

            foreach (var _item in  _list)
            {
                if (!(_item.X == _parent.X && _item.Y == _parent.Y)
                    && (_maze[_item.X][_item.Y].Equals(Guide._BONUS_PILL)
                    || _maze[_item.X][_item.Y].Equals(Guide._PILL)))
                {
                    isLeaf = false;
                    break;
                }
            }
            return isLeaf;
        }
        
        public static Point BuildPath(char[][] _maze, Point _start, Point _goal)
        {
            var _next = new Point();
            var _open = new List<BuildPathNode>();
            var _closed = new List<BuildPathNode>();

            var _gG = 0;
            var _hH = Mappings.ManhattanDistance(_start, _goal);
            var _fF = _gG + _hH;
            var _node = new BuildPathNode { _position = _start, _g = _gG, _h = _hH, _f = _fF };

            _open.Add(_node);
            var _found = false;
            
            while (!_found)
            {
                var _current = LowestRank(_open);                
                if ((_current._position.X == _goal.X)
                    && (_current._position.Y == _goal.Y))
                {
                    _found = true;//goal found
                    //traverse back
                    while (!(_current.parent._position.X==_start.X
                           &&_current.parent._position.Y==_start.Y))
                    {
                        _current = _current.parent;
                    }
                    _next = _current._position;
                }

                _closed.Add(_current);
                _open.Remove(_current);//remove it from open list

                var _neighbors = NextPossiblePositions(_maze, _current._position);


                foreach(var _neighbor in _neighbors)
                {
                    _gG = _current._g + 1;
                    _hH = Mappings.ManhattanDistance(_neighbor, _goal);
                    _fF = _gG + _hH;
                    var _curr = new BuildPathNode { _position = _neighbor, _g = _gG, _h = _hH, _f = _fF, parent = _current };
                    if (!_closed.Contains(_curr))
                    {
                        //add it to open list
                        _open.Add(_curr);
                    }
                }
            }
            return _next;
        }

        /*
        private static bool Contains(List<PathNode> _nodes, PathNode _node)
        {
            bool _found = false;
            for (var i = 0; i < _nodes.Count;i++)
            {
                if (_nodes[i]._position.X == _node._position.X && _nodes[i]._position.Y == _node._position.Y)
                {
                    _found = true;
                    break;
                }
            }
                return _found;
        }*/

        private static BuildPathNode LowestRank(List<BuildPathNode> _nodes)
        {
            var _result = new BuildPathNode();
            if (_nodes.Count==0)
            {
                _result = null;
            }
            else
            {
                foreach(var _node in _nodes)
                {
                    if (_result._position.IsEmpty)
                        _result = _node;
                    else
                    {
                        if (_node._f < _result._f)
                            _result = _node;
                    }
                }
            }
            return _result;
        }
    }
}
