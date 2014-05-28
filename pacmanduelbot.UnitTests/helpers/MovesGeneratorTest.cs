using NUnit.Framework;
using pacmanduelbot.brainbox;
using pacmanduelbot.helpers;
using pacmanduelbot.models;
using System.Drawing;

namespace pacmanduelbot.UnitTests.helpers
{
    [TestFixture]
    public class MovesGeneratorTest
    {
        [Test]
        public void TestGenerateNextPossiblePositions()
        {
            string _filepath1 = @"..\..\..\game.state";
            var _maze = Maze.Read(_filepath1);
            Bot _Bot = new Bot(_maze);

            var _currentP = _Bot._CURRENT_POSITION;

            var result = MovesGenerator.GenerateNextPossiblePositions(_maze, _currentP);

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

            var result = MovesGenerator.ChoosePath(_maze, new Point { X = 1, Y = 8 },4);

            var _test = false;
            if (result.X == 19 && result.Y == 10)
                _test = true;

            Assert.True(_test);
        }

    }
}
