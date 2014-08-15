using pacmanduelbot.brainbox;
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
        public static List<Point> GenerateMoves(Maze maze, Point currentPoint)
        {
            var nextMoves = new List<Point>();

            if (IsValidPoint(new Point { X = currentPoint.X, Y = currentPoint.Y + 1 }))
            {
                var point = new Point { X = currentPoint.X, Y = currentPoint.Y + 1 };
                if (!IsWall(maze, point) && !IsRespawnZoneRight(point))
                    nextMoves.Add(point);
            }

            if (IsValidPoint(new Point { X = currentPoint.X, Y = currentPoint.Y - 1 }))
            {
                var point = new Point { X = currentPoint.X, Y = currentPoint.Y - 1 };
                if (!IsWall(maze, point) && !IsRespawnZoneLeft(point))
                    nextMoves.Add(point);
            }

            if (IsValidPoint(new Point { X = currentPoint.X + 1, Y = currentPoint.Y }))
            {
                var point = new Point { X = currentPoint.X + 1, Y = currentPoint.Y };
                if (!IsWall(maze, point) && !IsRespawnZoneExitUP(point) && !IsRespawnZone(point))
                {
                    if (!(IsRespawnZone(currentPoint) && maze.GetSymbol(currentPoint.X + 1, currentPoint.Y).Equals(Symbols._PLAYER_B)))
                        nextMoves.Add(new Point { X = currentPoint.X + 1, Y = currentPoint.Y });
                }
            }

            if (IsValidPoint(new Point { X = currentPoint.X - 1, Y = currentPoint.Y }))
            {
                var point = new Point { X = currentPoint.X - 1, Y = currentPoint.Y };
                if (!IsWall(maze, point) && !IsRespawnZoneExitDown(point) && !IsRespawnZone(point))
                {
                    if (!(IsRespawnZone(currentPoint) && maze.GetSymbol(currentPoint.X - 1, currentPoint.Y).Equals(Symbols._PLAYER_B)))
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

        public static List<Point> PathToPill(Maze maze, Point start, Point destination)
        {
            var list = new List<Point>();
            var closedSet = new List<PathNode>();
            var hScore = Math.Abs(start.X - destination.X) + Math.Abs(start.Y - destination.Y);

            var openSet = new List<PathNode>
            {
                new PathNode
                {
                    Position = start,
                    GScore = 0,
                    HScore = hScore,
                    FScore = hScore
                }

            };

            while (openSet.Count != 0)
            {
                var current = BestNode(openSet);
                if (current.Position.Equals(destination))
                {
                    list.Add(new Point { X = current.GScore });
                    list.Add(ReconstructPath(start, current).Position);
                    return list;
                }
                openSet.Remove(current);
                closedSet.Add(current);

                var childpoints = GenerateMoves(maze, current.Position);

                foreach (var childpoint in childpoints)
                {
                    var gScore = current.GScore + 1;
                    hScore = Math.Abs(childpoint.X - destination.X) + Math.Abs(childpoint.Y - destination.Y);
                    var newChild = new PathNode
                    {
                        Position = childpoint,
                        GScore = gScore,
                        HScore = hScore,
                        FScore = gScore + hScore
                    };


                    if (closedSet.Contains(newChild))
                        continue;
                    var _newChild = FindChild(newChild, openSet);
                    if (_newChild != null
                        && gScore < _newChild.GScore)
                    {
                        openSet.Remove(_newChild);
                        current.InsertChild(newChild);
                        openSet.Add(newChild);
                    }
                    else if (_newChild == null)
                    {
                        current.InsertChild(newChild);
                        openSet.Add(newChild);
                    }
                }
            }
            return list;
        }

        public static Point SelectPath(Maze maze, Point start, int depth)
        {
            var openSet = new List<PathNode>
            {
                new PathNode{Position = start}
            };
            var closedSet = new List<PathNode>();
            var _depth = 0;

            while (openSet.Count != 0 && _depth < depth)
            {
                var root = openSet.First();
                openSet.Remove(root);

                var childPoints = GenerateMoves(maze, root.Position);

                foreach (var childPoint in childPoints)
                {
                    if (!isChildExplored(root, childPoint))
                    {
                        var _case = maze.GetSymbol(childPoint);
                        switch (_case)
                        {
                            case '.':
                                var newChild = new PathNode
                                {
                                    Position = childPoint,
                                    Score = root.Score + 1
                                };
                                root.InsertChild(newChild);
                                openSet.Add(newChild);
                                break;
                            case '*':
                                newChild = new PathNode
                                {
                                    Position = childPoint,
                                    Score = root.Score + 10
                                };
                                root.InsertChild(newChild);
                                openSet.Add(newChild);
                                break;
                            default:
                                break;
                        }
                    }
                }
                closedSet.Add(root);
                _depth++;
            }

            var current = new PathNode();
            if (openSet.Count != 0)
            {
                current = closedSet.OrderByDescending(pathNode => pathNode.Score).First();
                return ReconstructPath(start, current).Position;
            }

            current = closedSet.OrderByDescending(pathNode => pathNode.Score).First();
            var longestStreaks = closedSet.Where(pathNode => pathNode.Score == current.Score).ToList();
            if (longestStreaks.Count == 1)
                return ReconstructPath(start, current).Position;
            var random = new Random();
            var longestStreakIndex = random.Next(0, longestStreaks.Count);
            return ReconstructPath(start, longestStreaks[longestStreakIndex]).Position;
        }

        private static PathNode FindChild(PathNode newChild, List<PathNode> openSet)
        {
            foreach (var node in openSet)
                if (node.Position == newChild.Position)
                    return node;
            return null;
        }

        private static PathNode ReconstructPath(Point start, PathNode current)
        {
            while (current.Parent.Position != start)
                current = current.Parent;
            return current;
        }

        private static PathNode BestNode(List<PathNode> openSet)
        {
            return openSet.OrderBy(x => x.FScore).First();
        }

        private static bool isChildExplored(PathNode root, Point childPoint)
        {
            while (root != null)
            {
                if (root.Position == childPoint)
                    return true;
                root = root.Parent;
            }
            return false;
        }
    }
}