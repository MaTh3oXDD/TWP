using Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Logic
{
    public class PoolTable : LogicAbstractAPI
    {
        private readonly object _lock = new object();

        private int _width;
        private int _height;
        private List<Ball> _balls = new List<Ball>();
        private DataAbstractAPI _data;

        private bool _isRunning = false;
        private Task? _simulationTask;
        private CancellationTokenSource? _cancellationTokenSource;

        public PoolTable(int width, int height, DataAbstractAPI data)
        {
            _width = width;
            _height = height;
            _data = data;
        }

        public override void CreateBalls(int ballsQuantity, int radius)
        {
            Random random = new Random();
            lock (_lock)
            {
                _balls.Clear();
            }

            float minDistanceSquared = (2f * radius) * (2f * radius) * 1.1f;

            for (int i = 0; i < ballsQuantity; i++)
            {
                float x, y;
                bool positionFound = false;
                int attempts = 0;
                int maxAttempts = 1000;

                while (!positionFound && attempts < maxAttempts)
                {
                    x = (float)(random.NextDouble() * (_width - 2 * radius) + radius);
                    y = (float)(random.NextDouble() * (_height - 2 * radius) + radius);

                    bool colliding = false;
                    lock (_lock)
                    {
                        foreach (Ball existingBall in _balls)
                        {
                            float distanceSquared = Vector2.DistanceSquared(new Vector2(x, y), new Vector2(existingBall.X, existingBall.Y));
                            if (distanceSquared < (2f * radius) * (2f * radius))
                            {
                                colliding = true;
                                break;
                            }
                        }
                    }

                    if (!colliding)
                    {
                        float vx = (float)(random.NextDouble() * 4 - 2);
                        float vy = (float)(random.NextDouble() * 4 - 2);
                        float mass = (float)(random.NextDouble() * 1.5 + 0.5);

                        Ball ball = new Ball(x, y, radius, vx, vy, mass);
                        lock (_lock)
                        {
                            _balls.Add(ball);
                        }
                        positionFound = true;
                    }
                    attempts++;
                }
                if (!positionFound)
                {
                    Console.WriteLine($"Warning: Could not find a non-colliding position for ball {i + 1}.");
                }
            }
        }

        public override IReadOnlyList<Ball> GetAllBalls()
        {
            lock (_lock)
            {
                return _balls.AsReadOnly();
            }
        }

        public override void StartGame()
        {
            lock (_lock)
            {
                if (_isRunning) return;

                _isRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = _cancellationTokenSource.Token;

                _simulationTask = Task.Run(() => SimulationLoop(token), token);
            }
        }

        public override void StopGame()
        {
            lock (_lock)
            {
                if (!_isRunning) return;

                _isRunning = false;
                _cancellationTokenSource?.Cancel();
            }
        }

        public override int Width => _width;
        public override int Height => _height;

        private void SimulationLoop(CancellationToken token)
        {
            int simulationDelay = 16;

            while (_isRunning && !token.IsCancellationRequested)
            {
                lock (_lock)
                {
                    foreach (Ball ball in _balls)
                    {
                        ball.Move(_width, _height);
                    }

                    CheckBallCollisions();
                }

                Thread.Sleep(simulationDelay);
            }
        }

        private void CheckBallCollisions()
        {
            float collisionCheckThresholdFactor = 0.9f;

            for (int i = 0; i < _balls.Count; i++)
            {
                for (int j = i + 1; j < _balls.Count; j++)
                {
                    Ball ball1 = _balls[i];
                    Ball ball2 = _balls[j];

                    Vector2 pos1 = new Vector2(ball1.X, ball1.Y);
                    Vector2 pos2 = new Vector2(ball2.X, ball2.Y);

                    float distance = Vector2.Distance(pos1, pos2);

                    float minDistanceForCollision = ball1.Radius + ball2.Radius;

                    if (distance < minDistanceForCollision * collisionCheckThresholdFactor)
                    {
                        if (distance < minDistanceForCollision)
                        {
                            ResolveBallCollision(ball1, ball2);
                            float overlap = minDistanceForCollision - distance;
                            if (overlap > 0)
                            {
                                Vector2 separation = Vector2.Normalize(pos1 - pos2) * (overlap / 2f + 0.5f);
                                ball1.X += separation.X;
                                ball1.Y += separation.Y;
                                ball2.X -= separation.X;
                                ball2.Y -= separation.Y;
                            }
                        }
                    }
                }
            }
        }

        private void ResolveBallCollision(Ball ball1, Ball ball2)
        {
            Vector2 pos1 = new Vector2(ball1.X, ball1.Y);
            Vector2 pos2 = new Vector2(ball2.X, ball2.Y);
            Vector2 vel1 = new Vector2(ball1.Vx, ball1.Vy);
            Vector2 vel2 = new Vector2(ball2.Vx, ball2.Vy);

            Vector2 normal = Vector2.Normalize(pos2 - pos1);
            if (normal.LengthSquared() < 1e-6)
            {
                normal = new Vector2(1, 0);
            }

            Vector2 tangent = new Vector2(-normal.Y, normal.X);

            float vel1Normal = Vector2.Dot(vel1, normal);
            float vel1Tangent = Vector2.Dot(vel1, tangent);
            float vel2Normal = Vector2.Dot(vel2, normal);
            float vel2Tangent = Vector2.Dot(vel2, tangent);

            float m1 = ball1.Mass;
            float m2 = ball2.Mass;

            float newVel1Normal = ((vel1Normal * (m1 - m2)) + (2 * m2 * vel2Normal)) / (m1 + m2);
            float newVel2Normal = ((vel2Normal * (m2 - m1)) + (2 * m1 * vel1Normal)) / (m1 + m2);

            float newVel1Tangent = vel1Tangent;
            float newVel2Tangent = vel2Tangent;

            Vector2 newVel1 = newVel1Normal * normal + newVel1Tangent * tangent;
            Vector2 newVel2 = newVel2Normal * normal + newVel2Tangent * tangent;

            ball1.Vx = newVel1.X;
            ball1.Vy = newVel1.Y;
            ball2.Vx = newVel2.X;
            ball2.Vy = newVel2.Y;
        }

    }
}
