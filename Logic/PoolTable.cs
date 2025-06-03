using System;
using System.Collections.Generic;
using System.Timers;
using Data;
using Logic.Utils;

namespace Logic
{
    internal class PoolTable : LogicAbstractAPI
    {
        private int _width;
        private int _height;
        private readonly object _lock = new object();
        private List<Ball> _balls = new List<Ball>();
        private System.Timers.Timer? _timer;
        private DateTime _lastTick = DateTime.Now;

        public PoolTable(int width, int height, DataAbstractAPI data)
        {
            _width = width;
            _height = height;
        }

        public override void CreateBalls(int ballsQuantity, int radius)
        {
            lock (_lock)
            {
                StopGame();
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
                            IDiagnosticsLogger logger = new DiagnosticsLogger("log.txt");
                            _balls.Add(new Ball(x, y, radius, vx, vy, mass, logger));
                            positionFound = true;
                        }
                        attempts++;
                    }
                }
                StartGame();
            }
        }

        public override void StartGame()
        {
            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }

                _lastTick = DateTime.Now;

                _timer = new System.Timers.Timer(16.0);
                _timer.Elapsed += OnTimerElapsed;
                _timer.AutoReset = true;
                _timer.Start();
            }
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                var now = DateTime.Now;
                float delta = (float)(now - _lastTick).TotalSeconds;
                _lastTick = now;

                foreach (var ball in _balls)
                {
                    ball.Move(_width, _height, _balls, delta);
                }
            }
        }

        public override void StopGame()
        {
            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }
                foreach (var ball in _balls)
                    ball.Stop();
            }
        }

        public override int Width => _width;
        public override int Height => _height;

        public override List<Ball> GetAllBalls()
        {
            lock (_lock)
            {
                return new List<Ball>(_balls);
            }
        }
    }
}
