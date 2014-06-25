using pacmanduelbot.models;
using pacmanduelbot.shared;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace pacmanduelbot.helpers
{
    class Moves
    {
        public static List<Point> FindPathToPill(Maze _maze, Point _start, Point _destination)
        {
            var _next_list = new List<Point>();
            var _closedset = new List<PathFinderNode>();
            var h_score = Math.Abs(_start.X - _destination.X) + Math.Abs(_start.Y - _destination.Y);
            var _openset = new List<PathFinderNode>
            {
                new PathFinderNode
                {
                    _position = _start,
                    _g_score = 0,
                    _h_score = h_score,
                    _f_score = h_score
                }
            };

            while (!(_openset.Count == 0))
            {
                var _current = FindLowFScore(_openset);
                if (_current._position.Equals(_destination))
                {
                    //TODO:
                    _next_list.Add(new Point { X = _current._g_score });
                    _next_list.Add(ReconstructPath(_current, _closedset)._position);
                    return _next_list;
                }
                _openset.Remove(_current);
                _closedset.Add(_current);

                var _neighbor_nodes = GenerateMoves(_maze, _current._position);

                foreach (var neighbor in _neighbor_nodes)
                {
                    var _tentative_g_score = _current._g_score + 1;
                    h_score = Math.Abs(neighbor.X - _destination.X) + Math.Abs(neighbor.Y - _destination.Y);
                    var _neighbor_node = new PathFinderNode
                    {
                        _position = neighbor,
                        _g_score = _tentative_g_score,
                        _h_score = h_score,
                        _f_score = _tentative_g_score + h_score,
                        _parent = _current
                    };
                    if (_closedset.Contains(_neighbor_node))
                        continue;
                    var _neighbor = FindNeighborInOpenSet(_openset, neighbor);
                    if (!(_neighbor == null)
                        && _tentative_g_score < _neighbor._g_score)
                    {
                        _openset.Remove(_neighbor);
                        _openset.Add(_neighbor_node);

                    }
                    else if (_neighbor == null)
                    {
                        _openset.Add(_neighbor_node);
                    }
                }

            }

            return _next_list;
        }

        private static PathFinderNode ReconstructPath(PathFinderNode _current_node, List<PathFinderNode> _closedset)
        {
            if (_current_node._parent == _closedset[0])
                return _current_node;
            else
                return ReconstructPath(_current_node._parent, _closedset);
        }

        private static PathFinderNode FindNeighborInOpenSet(List<PathFinderNode> _openset, Point neighbor)
        {
            foreach (var node in _openset)
            {
                if (node._position == neighbor)
                    return node;
            }
            return null;
        }

        private static PathFinderNode FindLowFScore(List<PathFinderNode> _openset)
        {
            var _result = new PathFinderNode();
            foreach (var _node in _openset)
            {
                if (_result._position.IsEmpty)
                    _result = _node;
                else if (_node._f_score < _result._f_score)
                    _result = _node;
            }
            return _result;
        }

        public static Point PathSelect(Maze _maze, Point _current_position, int depth)
        {
            var _open = new List<PathFinderNode>
            {
                new PathFinderNode{_position = _current_position}
            };
            var _closed = new List<PathFinderNode>();
            var _depth = 0;
            var startTime = DateTime.Now;
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
            var finishTime = DateTime.Now;
            var curr = new PathFinderNode();

            if(_open.Count != 0)
            {
                foreach(var _item in _closed)
                {
                    if (_item._score > curr._score)
                        curr = _item;
                }

                var exit1Time = DateTime.Now;
                return ReconstructPath(curr, _closed)._position;
            }

            
            foreach (var _item in _closed)
            {
                if (_item._isLeaf)
                    if (_item._score > curr._score)
                        curr = _item;
            }
            var exit2Time = DateTime.Now;
            return ReconstructPath(curr, _closed)._position;
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

        /*
        public static Point MinMaxDecision(Maze _maze, Point MaxPosition, Point MinPosition)
        {
            var _nextMove = new Point();


            var gameTree = new List<GameBoard>();

            gameTree.Add(new GameBoard { maze = new Maze(_maze), MaxPlayer = MaxPosition, MinPlayer = MinPosition });
            var depth = 0;
            while (!isTerminalState(gameTree) && depth < 4)
            {
                var MaxCount = gameTree.Count;
                for (var i = 0; i < MaxCount; i++)
                {
                    //MAX
                    if (gameTree[i].isLastLevel)
                    {
                        gameTree[i].isLastLevel = false;
                        var moveList = GenerateMoves(gameTree[i].maze, gameTree[i].MaxPlayer);
                        foreach (var move in moveList)
                        {
                            var _MaxPoints = gameTree[i].MaxPoints;
                            var _symbol = gameTree[i].maze.GetSymbol(move);
                            if (_symbol.Equals(Symbols._PILL))
                                _MaxPoints++;
                            else if (_symbol.Equals(Symbols._BONUS_PILL))
                                _MaxPoints += 10;
                            var gameBoard = new GameBoard
                            {
                                maze = new Maze(gameTree[i].maze),
                                MaxPlayer = move,
                                MinPlayer = gameTree[i].MinPlayer,
                                MaxPoints = _MaxPoints,
                                MinPoints = gameTree[i].MinPoints,
                                PrecedingBoard = gameTree[i]
                            };
                            gameBoard.MakeMove(move, gameTree[i].MaxPlayer, Symbols._PLAYER_A);
                            gameTree.Add(gameBoard);
                            gameTree[i].Childs.Add(gameBoard);
                        }
                    }
                }

                //MIN
                if (!isTerminalState(gameTree))
                {
                    var MinCount = gameTree.Count;
                    for (var i = 0; i < MinCount; i++)
                    {
                        if (gameTree[i].isLastLevel)
                        {
                            gameTree[i].isLastLevel = false;
                            var moveList = GenerateMoves(gameTree[i].maze, gameTree[i].MinPlayer);
                            foreach (var move in moveList)
                            {
                                var _MinPoints = gameTree[i].MinPoints;
                                var _symbol = gameTree[i].maze.GetSymbol(move);
                                if (_symbol.Equals(Symbols._PILL))
                                    _MinPoints++;
                                else if (_symbol.Equals(Symbols._BONUS_PILL))
                                    _MinPoints += 10;
                                var gameBoard = new GameBoard
                                {
                                    maze = new Maze(gameTree[i].maze),
                                    MaxPlayer = gameTree[i].MaxPlayer,
                                    MinPlayer = move,
                                    MaxPoints = gameTree[i].MaxPoints,
                                    MinPoints = _MinPoints,
                                    PrecedingBoard = gameTree[i]
                                };
                                gameBoard.MakeMove(move, gameTree[i].MinPlayer, Symbols._PLAYER_B);
                                gameTree.Add(gameBoard);
                                gameTree[i].Childs.Add(gameBoard);
                            }
                        }
                    }
                }
                depth++;
            }

            EvalGameSTate(gameTree);

            //var _bestMoveUtility = AlphaBeta(gameTree[0], 100,double.NegativeInfinity,double.PositiveInfinity, true);
            var _bestMoveUtility = MinMax(gameTree[0], 100, true);
            foreach (var child in gameTree[0].Childs)
            {
                if (child.Utility.Equals(_bestMoveUtility))
                    _nextMove = child.MaxPlayer;
            }
            return _nextMove;
        }


        private static double MinMax(GameBoard gameBoard, int depth, bool MaxPlayer)
        {
            if (depth == 0 || gameBoard.isLastLevel)
                return gameBoard.Utility;

            if (MaxPlayer)
            {
                var v = double.NegativeInfinity;
                foreach (var child in gameBoard.Childs)
                {
                    var val = MinMax(child, depth - 1, false);
                    v = Max(v, val);
                    child.PrecedingBoard.Utility = v;
                }

                return v;
            }
            else
            {
                var v = double.PositiveInfinity;
                foreach (var child in gameBoard.Childs)
                {
                    var val = MinMax(child, depth - 1, true);
                    v = Min(v, val);
                    child.PrecedingBoard.Utility = v;
                }
                return v;
            }
        }

        private static double AlphaBeta(GameBoard gameBoard, int depth, double a, double b, bool MaxPlayer)
        {
            var bestChild = new GameBoard();
            if (depth == 0 || gameBoard.isLastLevel)
                return gameBoard.Utility;

            if (MaxPlayer)
            {
                foreach (var child in gameBoard.Childs)
                {
                    a = Max(a, AlphaBeta(child, depth - 1, a, b, false));
                    bestChild = child;
                    if (b <= a)
                        break;

                }
                bestChild.isBestMove = true;
                return a;
            }
            else
            {
                foreach (var child in gameBoard.Childs)
                {
                    b = Min(b, AlphaBeta(child, depth - 1, a, b, true));
                    child.Utility = b;
                    if (b <= a)
                        break;
                }
                return b;
            }
        }

        private static double Max(double x, double y)
        {
            return x > y ? x : y;
        }
        private static double Min(double x, double y)
        {
            return x < y ? x : y;
        }

        private static bool isTerminalState(List<GameBoard> gameTree)
        {
            foreach (var board in gameTree)
            {
                if (board.isLastLevel)
                {
                    if (!board.isTerminal())
                        return false;
                }

            }
            return true;
        }

        private static void EvalGameSTate(List<GameBoard> gameTree)
        {
            //var _gameTree = new List<GameBoard>();
            for (var i = 0; i < gameTree.Count; i++)
            {
                if (gameTree[i].isLastLevel)
                {
                    var _utility = gameTree[i].MaxPoints - gameTree[i].MinPoints;
                    gameTree[i].Utility = _utility;
                    //_gameTree.Add(board);
                }
            }
            //return _gameTree;
        }*/

    }
}