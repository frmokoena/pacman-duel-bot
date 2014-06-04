using NUnit.Framework;
using pacmanduelbot.brainbox;
using pacmanduelbot.models;

namespace pacmanduelbot.UnitTests.brainbox
{
    [TestFixture]
    public class BotTest
    {
        [Test]
        public void TestMakeMove()
        {
            //string _filepath1 = @"..\..\..\game.state";
            //string _filepath2 = @"..\..\..\TempFile\test.txt";
            //var maze = Maze.Read(_filepath1);
            //Bot _Bot = new Bot { _maze = maze };

            //var result = _Bot._CURRENT_POSITION;
            //var curtxt1 = result.ToString();

            //using (var file = new System.IO.StreamWriter(_filepath2, false))
            //{
            //    file.WriteLine(curtxt1);
            //    file.Close();
            //}

            //maze = _Bot.MakeMove();
            //Maze.Write(maze, _filepath1);

            //var _test = false;
            //if (result.X == 7
            //    && result.Y == 4)
            //    _test = true;

            //Assert.True(_test);

        }

        [Test]
        public void TestFindNearbyPill()
        {
            //string _filepath1 = @"..\..\..\game.state";
            //string _filepath2 = @"..\..\..\TempFile\test.txt";
            //var maze = Maze.Read(_filepath1);
            //Bot _Bot = new Bot { _maze = maze };

            //var _curr = _Bot._CURRENT_POSITION;

            //var result = _Bot.FindNearbyPill();

            //var _test = false;
            //if (result.X == 20 && result.Y == 16)
            //    _test = true;

            //Assert.True(_test);
        }

        [Test]
        public void TestDropPill()
        {
            //string _filepath1 = @"..\..\..\game.state";
            //string _filepath2 = @"..\..\..\TempFile\test.txt";
            //var maze = Maze.Read(_filepath1);
            //Bot _Bot = new Bot { _maze = maze };

            //maze = _Bot.MakeMove();

            //var result = _Bot._DROP_PILL;

            //Assert.True(result);

        }

        [Test]
        public void TestIfNeedRespawn()
        {
            //string _filepath1 = @"..\..\..\game.state";
            ////string _filepath2 = @"..\..\..\TempFile\test.txt";
            //var maze = Maze.Read(_filepath1);
            //Bot _Bot = new Bot { _maze = maze };

            //var result = _Bot.needSelfRespawn();


            //Assert.True(result);
        }

        [Test]
        public void TestRespawnMove()
        {
            //string _filepath1 = @"..\..\..\game.state";
            ////string _filepath2 = @"..\..\..\TempFile\test.txt";
            //var maze = Maze.Read(_filepath1);
            //Bot _Bot = new Bot { _maze = maze };

            //maze = _Bot.MakeMove();

            //Maze.Write(maze, _filepath1);

            //var result = _Bot.needSelfRespawn();


            //Assert.True(result);

        }
    }
}
