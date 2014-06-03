using NUnit.Framework;
using pacmanduelbot.brainbox;
using pacmanduelbot.helpers;
using pacmanduelbot.models;
using System.Collections.Generic;
using System.Drawing;

namespace pacmanduelbot.UnitTests.helpers
{
    [TestFixture]
    public class MovesTest
    {
        [Test]
        public void TestGenerateNextPossiblePositions()
        {
            string _filepath1 = @"..\..\..\game.state";
            var _maze = Maze.Read(_filepath1);
            Bot _Bot = new Bot(_maze);

            var _currentP = _Bot._CURRENT_POSITION;

            var result = Moves.NextPossiblePositions(_maze, _currentP);

            var _test = false;
            if (result.Count == 2
                && (result[0].X == 1 && result[0].Y == 2)
                && (result[1].X == 2 && result[1].Y == 1))
                _test = true;

            Assert.True(_test);
        }

        [Test]
        public void TestChoosePath()
        {
            string _filepath1 = @"..\..\..\game.state";
            var _maze = Maze.Read(_filepath1);
            var list = new List<Point>
            {
                new Point{X = 20,Y = 11},
                new Point{X = 19,Y = 10}
            };

            var result = Moves.ChoosePath(_maze, list,10);

            var _test = false;
            if (result.X == 19 && result.Y == 10)
                _test = true;

            Assert.True(_test);
        }

    }
}
