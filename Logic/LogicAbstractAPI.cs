using System.Collections.Generic;
using Data;

namespace Logic
{
    public abstract class LogicAbstractAPI
    {
        public static LogicAbstractAPI CreateApi(DataAbstractAPI data = null)
        {
            return new PoolTable(550, 300, data ?? DataAbstractAPI.CreateApi());
        }

        public abstract void CreateBalls(int ballsQuantity, int radius);
        public abstract void StartGame();
        public abstract void StopGame();
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract IReadOnlyList<Ball> GetAllBalls();
    }
}
