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
                    //DO NOTHING, OTHERWISE I FORSEE DUPLICATION.                   
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
                    //DO NOTHING, OTHERWISE I FORSEE DUPLICATION.
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
            var _open = new List<PathNode>();
            var _closed = new List<PathNode>();
            var _node = new PathNode { _position = _current_position };
            _open.Add(_node);
            var _count = 0;

            while (_open.Count != 0 && _count < _depth)
            {
                _closed.Add(_open[0]);

                var _tempI = NextPossiblePositions(_maze, _open[0]._position);
                
                for (var i = 0; i < _tempI.Count; i++)
                {
                    if (_open[0]._parent != null)
                    {
                        if (!(_tempI[i].X == _open[0]._parent._position.X && _tempI[i].Y == _open[0]._parent._position.Y))
                        {


                            var _case = _maze[_tempI[i].X][_tempI[i].Y];

                            switch(_case)
                            {
                                case Guide._PILL:
                                    var _path_node = new PathNode
                                    { 
                                        _position = _tempI[i],
                                        _score=_open[0]._score + 1,
                                        isLeaf=_isLeaf(_maze,_tempI[i],_open[0]._position),
                                        _parent = _open[0]
                                    };
                                    _open.Add(_path_node);
                                    break;
                                case Guide._BONUS_PILL:
                                    _path_node = new PathNode
                                    {
                                        _position = _tempI[i],
                                        _score=_open[0]._score + 10,
                                        isLeaf = _isLeaf(_maze, _tempI[i], _open[0]._position),
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
                        var _case = _maze[_tempI[i].X][_tempI[i].Y];
                        switch (_case)
                        {
                            case Guide._PILL:
                                var _path_node = new PathNode
                                {
                                    _position = _tempI[i],
                                    _score = _open[0]._score + 1,
                                    isLeaf = _isLeaf(_maze,_tempI[i],_open[0]._position),
                                    _parent = _open[0]
                                };
                                _open.Add(_path_node);
                                break;
                            case Guide._BONUS_PILL:
                                _path_node = new PathNode
                                {
                                    _position = _tempI[i],
                                    _score = _open[0]._score + 10,
                                    isLeaf = _isLeaf(_maze,_tempI[i],_open[0]._position),
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

            var curr = new PathNode();
                       
            for(var k=0;k<_closed.Count;k++)
            {

                if(_closed[k].isLeaf)
                {
                    if(curr._position.IsEmpty)
                    {
                        curr = _closed[k];
                    }
                    else
                    {
                        if(_closed[k]._score > curr._score)
                        {
                            curr = _closed[k];
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

        private static bool _isLeaf(char[][] _maze,Point _point, Point _parent)
        {
            var isLeaf = true;
            var _list = NextPossiblePositions(_maze, _point);

            for (var j = 0; j < _list.Count; j++)
            {
                if (!(_list[j].X == _parent.X && _list[j].Y == _parent.Y)
                    && (_maze[_list[j].X][_list[j].Y].Equals(Guide._BONUS_PILL)
                    || _maze[_list[j].X][_list[j].Y].Equals(Guide._PILL)))
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

            var _open = new List<PathNode>();

            var _gG = 0;
            var _hH = Mappings.ManhattanDistance(_start, _goal);
            var _fF = _gG + _hH;
            var _node = new PathNode { _position = _start, _g = _gG, _h = _hH, _f = _fF };

            _open.Add(_node);

            var _closed = new List<PathNode>();

            var _found = false;

            
            while (!_found)
            {
                var _current = LowRank(_open);
                _closed.Add(new PathNode{_position=_current._position, _g=_current._g, _h=_current._h, _f=_current._f, _parent=_current._parent});
                _open.Remove(_current);//remove it from open list
                if ((_current._position.X == _goal.X)
                    && (_current._position.Y == _goal.Y))
                {
                    _found = true;
                    while (!(_current._parent._position.X==_start.X
                           &&_current._parent._position.Y==_start.Y))
                    {
                        _current = _current._parent;
                    }
                    _next = _current._position;
                }

                var _neighbors = NextPossiblePositions(_maze, _current._position);

                for (var i = 0; i < _neighbors.Count; i++)
                {
                    _gG = _current._g + 1;
                    _hH = Mappings.ManhattanDistance(_neighbors[i], _goal);
                    _fF = _gG + _hH;
                    var _curr = new PathNode { _position = _neighbors[i], _g = _gG, _h = _hH, _f = _fF,_parent=_current };
                    if (!contains(_closed,_curr))
                    {
                        //add it to open list
                        _open.Add(_curr);
                    }
                }
            }
            return _next;
        }


        private static bool contains(List<PathNode> _nodes, PathNode _node)
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
        }

        private static PathNode LowRank(List<PathNode> _nodes)
        {
            var _result = new PathNode();
            if (_nodes.Count==0)
            {
                _result = null;
            }
            else
            {
                for(var i=0;i<_nodes.Count;i++)
                {
                    if (_result._position.IsEmpty)
                        _result = _nodes[0];
                    else
                    {
                        if (_nodes[i]._f < _result._f)
                            _result = _nodes[i];
                    }
                }

            }
            return _result;
        }



    }
}
