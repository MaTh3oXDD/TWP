using Logic; // Wymaga referencji do projektu Logic
using Data; // Wymaga referencji do projektu Data
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Numerics; // Dodano using dla Vector2 w testach
using System.Threading;
using System.Threading.Tasks;

namespace LogicTest
{
    // Ta klasa dziedziczy z Data.DataAbstractAPI (z projektu Data)
    internal class FakeDataApi : DataAbstractAPI
    {
        // Ta klasa jest tylko w projekcie LogicTest i implementuje DataAbstractAPI
        // do celów testowych.
        public FakeDataApi()
        {
            // Konstruktor
        }
        // Jeœli DataAbstractAPI mia³oby abstrakcyjne metody, musia³byœ je tutaj zaimplementowaæ.
    }

    [TestClass]
    public class PoolTableTest
    {
        private DataAbstractAPI? data; // Typ to Data.DataAbstractAPI
        private LogicAbstractAPI? poolTable;
        private int testWidth = 800;
        private int testHeight = 600;
        private int testBallRadius = 10;
        private int testBallsQuantity = 5;

        [TestInitialize]
        public void Initialize()
        {
            // Tworzymy instancjê FakeDataApi, która JEST typem Data.DataAbstractAPI (przez dziedziczenie)
            data = new FakeDataApi();
            // Przekazujemy instancjê FakeDataApi do metody CreateApi, która oczekuje Data.DataAbstractAPI
            // B³¹d CS1503 powinien znikn¹æ, jeœli FakeDataApi poprawnie dziedziczy
            // z Data.DataAbstractAPI i nie ma duplikatów definicji.
            poolTable = LogicAbstractAPI.CreateApi();
        }

        // ... (reszta metod testowych jak CreateBallsTest, StartStopGameTest)

        [TestMethod]
        public void CreateBallsTest()
        {
            Assert.IsNotNull(poolTable, "PoolTable should be initialized.");

            poolTable.CreateBalls(testBallsQuantity, testBallRadius);

            Assert.AreEqual(testBallsQuantity, poolTable.GetAllBalls().Count, "Should create the specified number of balls.");

            foreach (var ball in poolTable.GetAllBalls())
            {
                Assert.AreEqual(testBallRadius, ball.Radius, "Ball should have the specified radius.");
                Assert.IsTrue(ball.Mass > 0, "Ball mass should be positive.");
                Assert.IsTrue(ball.X >= ball.Radius && ball.X <= testWidth - ball.Radius, "Ball X position is out of bounds.");
                Assert.IsTrue(ball.Y >= ball.Radius && ball.Y <= testHeight - ball.Radius, "Ball Y position is out of bounds.");
            }
        }

        [TestMethod]
        public async Task StartStopGameTest()
        {
            Assert.IsNotNull(poolTable, "PoolTable should be initialized.");

            poolTable.CreateBalls(2, testBallRadius);
            Assert.AreEqual(2, poolTable.GetAllBalls().Count, "Should have 2 balls before starting game.");

            poolTable.StartGame();

            await Task.Delay(50);

            var initialPositions = poolTable.GetAllBalls().Select(b => new Vector2(b.X, b.Y)).ToList();

            await Task.Delay(100);

            bool moved = false;
            var currentPositions = poolTable.GetAllBalls().Select(b => new Vector2(b.X, b.Y)).ToList();
            for (int i = 0; i < initialPositions.Count; i++)
            {
                if (Vector2.Distance(initialPositions[i], currentPositions[i]) > 0.1f)
                {
                    moved = true;
                    break;
                }
            }
            Assert.IsTrue(moved, "Balls should move after starting the game.");

            poolTable.StopGame();

            await Task.Delay(50);

            Assert.AreEqual(2, poolTable.GetAllBalls().Count, "Balls should not be removed after stopping the game.");
        }
    }
}
