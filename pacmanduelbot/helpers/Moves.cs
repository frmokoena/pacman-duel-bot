using pacmanduelbot.models;
using pacmanduelbot.shared;
using System.Collections.Generic;
using System.Drawing;

namespace pacmanduelbot.helpers
{
    class Moves
    {
        public static Point MinMaxDecision(Maze _maze, Point MaxPosition, Point MinPosition)
        {
            var _nextMove = new Point();


            var gameTree = new List<GameBoard>();

            gameTree.Add(new GameBoard { maze = new Maze(_maze), MaxPlayer = MaxPosition, MinPlayer = MinPosition });
            var depth = 0;
            while (!isTerminalState(gameTree) && depth < 6)
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
                            if (_symbol.Equals(Guide._PILL))
                                _MaxPoints++;
                            else if (_symbol.Equals(Guide._BONUS_PILL))
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
                            gameBoard.MakeMove(move, gameTree[i].MaxPlayer, Guide._PLAYER_A);
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
                                if (_symbol.Equals(Guide._PILL))
                                    _MinPoints++;
                                else if (_symbol.Equals(Guide._BONUS_PILL))
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
                                gameBoard.MakeMove(move, gameTree[i].MinPlayer, Guide._PLAYER_B);
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
                    if(b <= a)
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
        }

        public static List<Point> GenerateMoves(Maze maze, Point currentPoint)
        {
            var nextMoves = new List<Point>();

            if (currentPoint.Y + 1 < Guide._WIDTH)
            {
                var _symbol = maze.GetSymbol(new Point { X = currentPoint.X, Y = currentPoint.Y + 1 });
                if (!_symbol.Equals(Guide._WALL)
                    && !(currentPoint.X.Equals(Guide._FORBIDDEN_R_X) && currentPoint.Y.Equals(Guide._FORBIDDEN_R_Y - 1)))
                    nextMoves.Add(new Point { X = currentPoint.X, Y = currentPoint.Y + 1 });
            }

            if (currentPoint.Y - 1 >= 0)
            {
                var _symbol = maze.GetSymbol(new Point { X = currentPoint.X, Y = currentPoint.Y - 1 });
                if (!_symbol.Equals(Guide._WALL)
                    && !(currentPoint.X.Equals(Guide._FORBIDDEN_L_X) && currentPoint.Y.Equals(Guide._FORBIDDEN_L_Y + 1)))
                    nextMoves.Add(new Point { X = currentPoint.X, Y = currentPoint.Y - 1 });
            }

            if (currentPoint.X + 1 < Guide._HEIGHT)
            {
                var _symbol = maze.GetSymbol(new Point { X = currentPoint.X + 1, Y = currentPoint.Y });
                if (!_symbol.Equals(Guide._WALL)
                    && !(currentPoint.X.Equals(Guide._EXIT_UP_X - 1) && currentPoint.Y.Equals(Guide._EXIT_UP_Y))
                    && !(currentPoint.X.Equals(Guide._RESPAWN_X - 1) && currentPoint.Y.Equals(Guide._RESPAWN_Y)))
                {
                    if ((currentPoint.X.Equals(Guide._RESPAWN_X) && currentPoint.Y.Equals(Guide._RESPAWN_Y))
                        && _symbol.Equals(Guide._PLAYER_B))
                    {
                        //do nothing   
                    }
                    else
                    {
                        nextMoves.Add(new Point { X = currentPoint.X + 1, Y = currentPoint.Y });
                    }
                }
            }


            if (currentPoint.X - 1 >= 0)
            {
                var _symbol = maze.GetSymbol(new Point { X = currentPoint.X - 1, Y = currentPoint.Y });
                if (!_symbol.Equals(Guide._WALL)
                    && !(currentPoint.X.Equals(Guide._EXIT_DOWN_X + 1) && currentPoint.Y.Equals(Guide._EXIT_DOWN_Y))
                    && !(currentPoint.X.Equals(Guide._RESPAWN_X + 1) && currentPoint.Y.Equals(Guide._RESPAWN_Y)))
                {
                    if ((currentPoint.X.Equals(Guide._RESPAWN_X) && currentPoint.Y.Equals(Guide._RESPAWN_Y))
                    && _symbol.Equals(Guide._PLAYER_B))
                    {
                        //do nothing
                    }
                    else
                    {
                        nextMoves.Add(new Point { X = currentPoint.X - 1, Y = currentPoint.Y });
                    }
                }
            }

            if (currentPoint.X.Equals(Guide._PORTAL1_X) && currentPoint.Y.Equals(Guide._PORTAL1_Y))
                nextMoves.Add(new Point { X = Guide._PORTAL2_X, Y = Guide._PORTAL2_Y });

            if (currentPoint.X.Equals(Guide._PORTAL2_X) && currentPoint.Y.Equals(Guide._PORTAL2_Y))
                nextMoves.Add(new Point { X = Guide._PORTAL1_X, Y = Guide._PORTAL1_Y });

            return nextMoves;
        }

        public static Point ChoosePath(Maze _maze, Point _current_position, int _depth)
        {
            var _next = new Point();
            var _open = new List<Node>();
            var _closed = new List<Node>();

            var _node = new Node { _position = _current_position };

            _open.Add(_node);

            var _count = 0;

            while (_open.Count != 0 && _count < _depth)
            {
                var _open_root = _open[0];
                _closed.Add(_open_root);

                var _tempI = GenerateMoves(_maze, _open_root._position);

                foreach (var _point in _tempI)
                {
                    var _case = _maze.GetSymbol(_point.X, _point.Y);
                    if (_open_root._parent != null)
                    {
                        if (!(_point.X == _open_root._parent._position.X && _point.Y == _open_root._parent._position.Y))
                        {
                            switch (_case)
                            {
                                case Guide._PILL:
                                    var _path_node = new Node
                                    {
                                        _position = _point,
                                        _score = _open_root._score + 1,
                                        _isLeaf = isLeaf(_maze, _point, _open_root._position),
                                        _parent = _open_root
                                    };
                                    _open.Add(_path_node);
                                    break;
                                case Guide._BONUS_PILL:
                                    _path_node = new Node
                                    {
                                        _position = _point,
                                        _score = _open_root._score + 10,
                                        _isLeaf = isLeaf(_maze, _point, _open_root._position),
                                        _parent = _open_root
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
                        switch (_case)
                        {
                            case Guide._PILL:
                                var _path_node = new Node
                                {
                                    _position = _point,
                                    _score = _open_root._score + 1,
                                    _isLeaf = isLeaf(_maze, _point, _open_root._position),
                                    _parent = _open_root
                                };
                                _open.Add(_path_node);
                                break;
                            case Guide._BONUS_PILL:
                                _path_node = new Node
                                {
                                    _position = _point,
                                    _score = _open_root._score + 10,
                                    _isLeaf = isLeaf(_maze, _point, _open_root._position),
                                    _parent = _open_root
                                };
                                _open.Add(_path_node);
                                break;
                            default:
                                break;
                        }
                    }
                }
                _open.Remove(_open_root);
                _count++;
            }

            var curr = new Node();
            var _closed_root = _closed[0];

            if (!(_open.Count == 0))
            {
                foreach (var _item in _closed)
                {
                    if (_item._score > curr._score)
                    {
                        curr = _item;
                    }
                }
                while (!(curr._parent._position.X == _closed_root._position.X && curr._parent._position.Y == _closed_root._position.Y))
                    curr = curr._parent;
                _next = curr._position;
                return _next;
            }

            foreach (var _item in _closed)
            {
                if (_item._isLeaf)
                {
                    if (_item._score == 1)
                    {
                        _next = _item._position;
                        return _next;
                    }
                    else
                    {
                        if (_item._score > curr._score)
                        {
                            curr = _item;
                        }
                    }
                }
            }

            while (!(curr._parent._position.X == _closed_root._position.X && curr._parent._position.Y == _closed_root._position.Y))
                curr = curr._parent;
            _next = curr._position;
            return _next;
        }

        private static bool isLeaf(Maze _maze, Point _point, Point _parent)
        {
            var _isLeaf = true;
            var _list = GenerateMoves(_maze, _point);

            foreach (var _item in _list)
            {
                if (!(_item.X == _parent.X && _item.Y == _parent.Y))
                {
                    var _symbol = _maze.GetSymbol(_item.X,_item.Y);
                    if(_symbol.Equals(Guide._BONUS_PILL)
                    || _symbol.Equals(Guide._PILL))
                    {
                        _isLeaf = false;
                        break;
                    }
                }
            }
            return _isLeaf;
        }

        public static List<Point> BuildPath(Maze _maze, Point _start, Point _target)
        {
            var _list = new List<Point>();
            var _open = new List<Node>();
            var _closed = new List<Node>();

            var g = 0;
            var h = Mappings.ManhattanDistance(_start, _target);
            var f = Mappings.CalculateWeight(g, h);
            var _curr = new Node
            {
                _position = _start,
                _g = g,
                _h = h,
                _f = f,
            };
            _open.Add(_curr);

            while (_open.Count != 0)
            {
                var _current = LowestRank(_open);
                if ((_current._position.X == _target.X)
                    && (_current._position.Y == _target.Y))
                {
                    _list.Add(new Point { X = _current._g });
                    //traverse back
                    while (!(_current._parent._position.X == _start.X
                           && _current._parent._position.Y == _start.Y))
                    {
                        _current = _current._parent;
                    }
                    _list.Add(_current._position);
                    //_list = _current._position;
                    break;
                }

                _closed.Add(_current);
                _open.Remove(_current);//remove it from open list

                var _neighbors = GenerateMoves(_maze, _current._position);

                foreach (var _neighbor in _neighbors)
                {
                    g = _current._g + 1;
                    h = Mappings.ManhattanDistance(_neighbor, _target);
                    f = Mappings.CalculateWeight(g, h);
                    _curr = new Node
                    {
                        _position = _neighbor,
                        _g = g,
                        _h = h,
                        _f = f,
                        _parent = _current
                    };
                    if (!_closed.Contains(_curr))
                    {
                        //add it to open list
                        _open.Add(_curr);
                    }
                }
            }
            return _list;
        }

        private static Node LowestRank(List<Node> _nodes)
        {
            var _result = new Node();
            if (_nodes.Count == 0)
            {
                _result = null;
            }
            else
            {
                foreach (var _node in _nodes)
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