using Logic;
using Data;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Mo�esz zaimplementowa� minimalne FakeDataApi, je�li wymaga Twoja logika
namespace LogicTest
{
    internal class FakeDataApi : DataAbstractAPI
    {
        public int Width => 800;
        public int Height => 600;

        // Nie ma DataBall w API, wi�c nie implementujemy GetAllBalls, CreateBalls, ClearBalls
        // Implementujemy wymagane metody abstrakcyjne

        public override void LogBallState(float x, float y, float vx, float vy, float timestamp) { }
        public override void StopLogger() { }
    }

    [TestClass]
    public class PoolTableTest
    {
        private DataAbstractAPI data;
        private LogicAbstractAPI poolTable;

        [TestInitialize]
        public void Initialize()
        {
            data = FakeDataApi.CreateApi();
            poolTable = LogicAbstractAPI.CreateApi(data);
        }

        [TestMethod]
        public void TestCreateBalls()
        {
            poolTable.CreateBalls(5, 10);
            Assert.AreEqual(5, poolTable.GetAllBalls().Count);
        }

        [TestMethod]
        public void TestWidth()
        {
            Assert.AreEqual(800, poolTable.Width);
        }

        [TestMethod]
        public void TestHeight()
        {
            Assert.AreEqual(600, poolTable.Height);
        }

        [TestMethod]
        public void TestStopGame()
        {
            poolTable.CreateBalls(5, 10);
            poolTable.StartGame();
            poolTable.StopGame();
            // Oczekujemy, �e kule nadal istniej� - tylko si� zatrzyma�y
            Assert.AreEqual(5, poolTable.GetAllBalls().Count, "Balls should still exist after stopping.");
        }

        [TestMethod]
        public void TestBallsStopMoving()
        {
            poolTable.CreateBalls(3, 10);
            poolTable.StartGame();

            // Zapami�taj pozycje na pocz�tek
            var balls = poolTable.GetAllBalls();
            var posBefore = balls.Select(b => (b.X, b.Y)).ToList();

            // Poczekaj, a� si� porusz�
            Thread.Sleep(150);

            poolTable.StopGame();

            // Zapami�taj pozycje kr�tko po zatrzymaniu
            var posAfterStop = balls.Select(b => (b.X, b.Y)).ToList();

            // Poczekaj chwil� i sprawd�, czy ju� si� nie ruszaj�
            Thread.Sleep(200);

            var posAfterWait = balls.Select(b => (b.X, b.Y)).ToList();

            // Po StopGame pozycje powinny ju� si� nie zmienia�
            CollectionAssert.AreEqual(posAfterStop, posAfterWait, "Balls should not move after StopGame.");
        }

        // Je�li chcesz testowa� czyszczenie planszy, musisz zaimplementowa� ClearBalls() w API!
        /*
        [TestMethod]
        public void TestClearBalls()
        {
            poolTable.CreateBalls(5, 10);
            poolTable.ClearBalls();
            Assert.AreEqual(0, poolTable.GetAllBalls().Count);
        }
        */
    }
}
