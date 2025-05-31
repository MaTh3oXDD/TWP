using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Logic
{
    internal class PoolTable : LogicAbstractAPI
    {
        private int _width;
        private int _height;
        private readonly object _lock = new object();
        private List<Ball> _balls = new List<Ball>();

        public PoolTable(int width, int height, DataAbstractAPI data)
        {
            _width = width;
            _height = height;
        }

        public override void CreateBalls(int ballsQuantity, int radius)
        {
            lock (_lock)
            {
                foreach (var b in _balls) b.Stop();
                _balls.Clear();

                Random random = new Random();

                for (int i = 0; i < ballsQuantity; i++)
                {
                    float x, y;
                    bool positionFound = false;
                    int attempts = 0, maxAttempts = 100;
                    while (!positionFound && attempts < maxAttempts)
                    {
                        x = random.Next(radius, _width - 2 * radius);
                        y = random.Next(radius, _height - 2 * radius);

                        bool colliding = false;
                        foreach (Ball existingBall in _balls)
                        {
                            var dx = x - existingBall.X;
                            var dy = y - existingBall.Y;
                            if (dx * dx + dy * dy < (2f * radius) * (2f * radius) * 1.1f)
                            {
                                colliding = true;
                                break;
                            }
                        }
                        if (!colliding)
                        {
                            float vx = 0, vy = 0;
                            while (Math.Abs(vx) < 0.2f) vx = (float)(random.NextDouble() * 6 - 3);
                            while (Math.Abs(vy) < 0.2f) vy = (float)(random.NextDouble() * 6 - 3);

                            float mass = random.Next(1, 10);
                            _balls.Add(new Ball(x, y, radius, vx, vy, mass));
                            positionFound = true;
                        }
                        attempts++;
                    }
                }

                // Dopiero po utworzeniu całej listy ruszamy wątki!
                foreach (var ball in _balls)
                {
                    Task.Run(() => ball.Run(_width, _height, _balls));
                }
            }
        }

        public override void StartGame() { /* Nieużywane: wątki startują po CreateBalls */ }

        public override void StopGame()
        {
            lock (_lock)
            {
                foreach (var ball in _balls)
                    ball.Stop();
                // NIE czyść _balls! Kulki muszą skończyć pętlę, a widok (ItemsControl) pobiera ich pozycje!
                // _balls.Clear(); // ← usuń ten wiersz, jak wcześniej opisałem
            }
        }

        public override int Width => _width;
        public override int Height => _height;

        public override List<Ball> GetAllBalls()
        {
            lock (_lock)
            {
                return new List<Ball>(_balls); // kopia dla bezpieczeństwa
            }
        }
    }
}
