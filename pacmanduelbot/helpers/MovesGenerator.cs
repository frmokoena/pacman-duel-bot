using pacmanduelbot.models;
using pacmanduelbot.shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace pacmanduelbot.helpers
{
    class MovesGenerator
    {
        public static List<Point> FindPathToPill(Maze _maze, Point _start, Point _destination)
        {
            var _nextList = new List<Point>();
            var _closedSet = new List<PathFinderNode>();
            var _hScore = Math.Abs(_start.X - _destination.X) + Math.Abs(_start.Y - _destination.Y);
            var _openSet = new List<PathFinderNode>
            {
                new PathFinderNode
                {
                    _position = _start,
                    _g_score = 0,
                    _h_score = _hScore,
                    _f_score = _hScore
                }
            };

            while (!(_openSet.Count == 0))
            {
                var _current = FindLowFScore(_openSet);
                if (_current._position.Equals(_destination))
                {
                    //TODO:
                    _nextList.Add(new Point { X = _current._g_score });
                    _nextList.Add(ReconstructPath(_current, _closedSet)._position);
                    return _nextList;
                }
                _openSet.Remove(_current);
                _closedSet.Add(_current);

                var _neighbor_nodes = GenerateMoves(_maze, _current._position);

                foreach (var neighbor in _neighbor_nodes)
                {
                    var _tentative_gScore = _current._g_score + 1;
                    _hScore = Math.Abs(neighbor.X - _destination.X) + Math.Abs(neighbor.Y - _destination.Y);
                    var _neighbor_node = new PathFinderNode
                    {
                        _position = neighbor,
                        _g_score = _tentative_gScore,
                        _h_score = _hScore,
                        _f_score = _tentative_gScore + _hScore,
                        _parent = _current
                    };
                    if (_closedSet.Contains(_neighbor_node))
                        continue;
                    var _neighbor = FindNeighborInOpenSet(_openSet, neighbor);
                    if (!(_neighbor == null)
                        && _tentative_gScore < _neighbor._g_score)
                    {
                        _openSet.Remove(_neighbor);
                        _openSet.Add(_neighbor_node);

                    }
                    else if (_neighbor == null)
                    {
                        _openSet.Add(_neighbor_node);
                    }
                }
            }
            return _nextList;
        }

        private static PathFinderNode ReconstructPath(PathFinderNode _current_node, List<PathFinderNode> _closedSet)
        {
            if (_current_node._parent == _closedSet[0])
                return _current_node;
            else
                return ReconstructPath(_current_node._parent, _closedSet);
        }

        private static PathFinderNode FindNeighborInOpenSet(List<PathFinderNode> _openSet, Point neighbor)
        {
            foreach (var node in _openSet)
            {
                if (node._position == neighbor)
                    return node;
            }
            return null;
        }

        private static PathFinderNode FindLowFScore(List<PathFinderNode> _openset)
        {
            return _openset.OrderBy(x => x._f_score).First();
        }

        public static Point PathSelect(Maze _maze, Point _current_position, int depth)
        {
            var _open = new List<PathFinderNode>
            {
                new PathFinderNode{_position = _current_position}
            };
            var _closed = new List<PathFinderNode>();
            var _depth = 0;
            while (_open.Count != 0 && _depth < depth)
            {
                var _open_root = _open[0];
                _closed.Add(_open_root);
                _open.Remove(_open_root);

                var _neighbors = GenerateMoves(_maze, _open_root._position);

                foreach (var _point in _neighbors)
                {
                    if (!isExplored(_open_root, _point))
                    {
                        var _case = _maze.GetSymbol(_point);
                        switch (_case)
                        {
                            case '.':
                                var _path_node = new PathFinderNode
                                {
                                    _position = _point,
                                    _score = _open_root._score + 1,
                                    _isLeaf = isLeaf(_maze,_open_root, _point),
                                    _parent = _open_root
                                };
                                _open.Add(_path_node);
                                break;
                            case '*':
                                _path_node = new PathFinderNode
                                {
                                    _position = _point,
                                    _score = _open_root._score + 10,
                                    _isLeaf = isLeaf(_maze, _open_root, _point),
                                    _parent = _open_root
                                };
                                _open.Add(_path_node);
                                break;
                            default:
                                break;
                        }
                    }
                }
                _depth++;
            }
            
            var curr = new PathFinderNode();
            if(_open.Count != 0)
            {
                curr = _closed.OrderByDescending(x => x._score).First();
                return ReconstructPath(curr, _closed)._position;
            }

            curr = _closed.OrderByDescending(x => x._score).First();

            var _longestStreaks = _closed.Where(pathNode => pathNode._score == curr._score).ToList();
            if(_longestStreaks.Count == 1)
                return ReconstructPath(curr, _closed)._position;
            var random = new Random();
            var _longestPathIndex = random.Next(0, _longestStreaks.Count);
            return ReconstructPath(_longestStreaks[_longestPathIndex], _closed)._position;            
        }

        private static bool isLeaf(Maze maze, PathFinderNode parent, Point point)
        {
            var _list = GenerateMoves(maze, point);
            foreach (var _item in _list)
            {
                if(!isExplored(new PathFinderNode { _position = point, _parent = parent }, _item))
                {
                    var _symbol = maze.GetSymbol(_item);
                    if (_symbol.Equals(Symbols._BONUS_PILL) || _symbol.Equals(Symbols._PILL))
                        return false;
                }              
            }
            return true;
        }

        static bool isExplored(PathFinderNode parent, Point point)
        {
            while (!(parent == null))
            {
                if (point == parent._position)
                    return true;
                parent = parent._parent;
            }
            return false;
        }    

        public static List<Point> GenerateMoves(Maze maze, Point currentPoint)
        {
            var nextMoves = new List<Point>();

            if (IsValidPoint(new Point { X = currentPoint.X, Y = currentPoint.Y + 1}))
            {
                var point = new Point{X = currentPoint.X,Y=currentPoint.Y + 1};
                if (!IsWall(maze, point) && !IsRespawnZoneRight(point))
                    nextMoves.Add(point);
            }

            if (IsValidPoint(new Point { X = currentPoint.X, Y = currentPoint.Y - 1}))
            {
                var point = new Point{X = currentPoint.X, Y = currentPoint.Y - 1};
                if (!IsWall(maze, point) && !IsRespawnZoneLeft(point))
                    nextMoves.Add(point);

            }

            if (IsValidPoint(new Point { X = currentPoint.X + 1, Y = currentPoint.Y}))
            {
                var point = new Point{X = currentPoint.X + 1, Y = currentPoint.Y};
                if(!IsWall(maze, point) && !IsRespawnZoneExitUP(point) && !IsRespawnZone(point))
                {
                    if (!(IsRespawnZone(currentPoint) && maze.GetSymbol(currentPoint.X + 1, currentPoint.Y).Equals(Symbols._PLAYER_B)))
                        nextMoves.Add(new Point { X = currentPoint.X + 1, Y = currentPoint.Y });
                }
            }
            
            if (IsValidPoint(new Point { X = currentPoint.X - 1, Y = currentPoint.Y}))
            {
                var point = new Point { X = currentPoint.X - 1, Y = currentPoint.Y };
                if (!IsWall(maze, point) && !IsRespawnZoneExitDown(point) && !IsRespawnZone(point))
                {
                    if(!(IsRespawnZone(currentPoint) && maze.GetSymbol(currentPoint.X - 1, currentPoint.Y).Equals(Symbols._PLAYER_B)))
                        nextMoves.Add(new Point { X = currentPoint.X - 1, Y = currentPoint.Y });
                }
            }
            
            if (IsPortalLeft(currentPoint))
                nextMoves.Add(new Point { X = Properties.Settings.Default._MazePortal2X, Y = Properties.Settings.Default._MazePortal2Y });

            if (IsPortalRight(currentPoint))
                nextMoves.Add(new Point { X = Properties.Settings.Default._MazePortal1X, Y = Properties.Settings.Default._MazePortal1Y });

            return nextMoves;
        }

        static bool IsRespawnZone(Point p)
        {
            return (p.X == Properties.Settings.Default._MazeRespawnX) 
                && (p.Y == Properties.Settings.Default._MazeRespawnY); 
        }

        static bool IsRespawnZoneExitUP(Point p)
        {
            return (p.X == Properties.Settings.Default._MazeRespawnExitUpX) 
                && (p.Y == Properties.Settings.Default._MazeRespawnExitUpY);
        }

        static bool IsRespawnZoneExitDown(Point p)
        {
            return (p.X == Properties.Settings.Default._MazeRespawnExitDownX)
                && (p.Y == Properties.Settings.Default._MazeRespawnExitDownY);
        }

        static bool IsRespawnZoneLeft(Point p)
        {
            return (p.X == Properties.Settings.Default._MazeForbiddenLX)
                && (p.Y == Properties.Settings.Default._MazeForbiddenLY);
        }

        static bool IsRespawnZoneRight(Point p)
        {
            return (p.X == Properties.Settings.Default._MazeForbiddenRX)
                && (p.Y == Properties.Settings.Default._MazeForbiddenRY);
        }

        static bool IsWall(Maze _maze, Point p)
        {
            return (_maze.GetSymbol(p) == Symbols._WALL);
        }

        static bool IsValidPoint(Point p)
        {
            return (p.X >= 0 && p.X < Properties.Settings.Default._MazeHeight)
                && (p.Y >= 0 && p.Y < Properties.Settings.Default._MazeWidth);
        }

        static bool IsPortalLeft(Point point)
        {
            return (point.X == Properties.Settings.Default._MazePortal1X)
                && (point.Y == Properties.Settings.Default._MazePortal1Y);
        }

        static bool IsPortalRight(Point point)
        {
            return (point.X == Properties.Settings.Default._MazePortal2X)
                && (point.Y == Properties.Settings.Default._MazePortal2Y);
        }
    }
}