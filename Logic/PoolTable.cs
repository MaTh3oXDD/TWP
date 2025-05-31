using Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Logic
{
    internal class PoolTable : LogicAbstractAPI
    {
        private int _width;
        private int _height;
        private List<Ball> _balls = new List<Ball>();
        private DataAbstractAPI _data;
        private bool _isRunning = false;
        private readonly object _lock = new object(); // Obiekt do synchronizacji

        public PoolTable(int width, int height, DataAbstractAPI data)
        {
            _width = width;
            _height = height;
            _data = data;
        }

        public override void CreateBalls(int ballsQuantity, int radius)
        {
            Random random = new Random();
            lock (_lock) // Chronimy dostęp do _balls
            {
                _balls.Clear();
                float minDistanceSquared = (2f * radius) * (2f * radius) * 1.1f;

                for (int i = 0; i < ballsQuantity; i++)
                {
                    float x, y;
                    bool positionFound = false;
                    int attempts = 0;
                    int maxAttempts = 100;

                    while (!positionFound && attempts < maxAttempts)
                    {
                        x = random.Next(radius, _width - radius);
                        y = random.Next(radius, _height - radius);

                        bool colliding = false;
                        foreach (Ball existingBall in _balls)
                        {
                            float distanceSquared = Vector2.DistanceSquared(new Vector2(x, y), new Vector2(existingBall.X, existingBall.Y));
                            if (distanceSquared < minDistanceSquared)
                            {
                                colliding = true;
                                break;
                            }
                        }

                        if (!colliding)
                        {
                            float vx = (float)(random.NextDouble() * 4 - 2);
                            float vy = (float)(random.NextDouble() * 4 - 2);
                            float mass = random.Next(1, 10); // Example mass between 1 and 10
                            Ball ball = new Ball(x, y, radius, vx, vy, mass);
                            _balls.Add(ball);
                            positionFound = true;
                            Task.Run(() => ball.Run(_width, _height, _balls));
                        }
                        attempts++;
                    }
                }
            }
        }

        public override void StartGame()
        {
            lock (_lock) // Chronimy dostęp do _isRunning
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                }
            }
        }

        public override void StopGame()
        {
            lock (_lock) // Chronimy dostęp do _balls i _isRunning
            {
                _isRunning = false;
                _balls.Clear();
            }
        }

        public override int Width => _width;
        public override int Height => _height;

        public override List<Ball> GetAllBalls()
        {
            lock (_lock) // Chronimy dostęp do _balls przy odczycie
            {
                return new List<Ball>(_balls); // Zwracamy kopię listy dla bezpieczeństwa
            }
        }
    }
}