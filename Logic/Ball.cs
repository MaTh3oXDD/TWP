using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Logic
{
    public class Ball : INotifyPropertyChanged
    {
        private float _x;
        private float _y;
        private float _vx;
        private float _vy;
        private readonly int _radius;
        public float Mass { get; }
        private volatile bool _running = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Stop() => _running = false;

        public Ball(float x, float y, int radius, float vx, float vy, float mass)
        {
            _x = x; _y = y; _radius = radius;
            _vx = vx; _vy = vy; Mass = mass;
        }

        public float X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }

        public float Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }

        public float Vx
        {
            get => _vx;
            set => _vx = value;
        }

        public float Vy
        {
            get => _vy;
            set => _vy = value;
        }

        public int Radius => _radius;

        // Możesz poprawić delay wedle preferencji, 16ms ~ 60 FPS
        public void Run(int width, int height, List<Ball> balls, object data = null!)
        {
            var lastTime = DateTime.Now;
            while (_running)
            {
                var now = DateTime.Now;
                var delta = (float)(now - lastTime).TotalSeconds;
                if (delta < 0.016f) // czekaj na pełny tick ~60Hz
                {
                    Thread.Sleep(1);
                    continue;
                }
                lastTime = now;

                Move(width, height, balls, delta);
            }
        }

        private void Move(int width, int height, List<Ball> balls, float deltaTime)
        {
            // Prosta fizyka
            _x += _vx * deltaTime * 60;
            _y += _vy * deltaTime * 60;

            // Odbicia od ścian
            if (_x < 0) { _x = 0; _vx = -_vx; }
            if (_x + _radius * 2 > width) { _x = width - _radius * 2; _vx = -_vx; }
            if (_y < 0) { _y = 0; _vy = -_vy; }
            if (_y + _radius * 2 > height) { _y = height - _radius * 2; _vy = -_vy; }

            CheckCollisions(balls);

            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        private void CheckCollisions(List<Ball> balls)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                var other = balls[i];
                if (other == this) continue;

                float dx = other.X - this.X;
                float dy = other.Y - this.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                float minDist = this.Radius + other.Radius;
                if (distance < minDist && distance > 0)
                {
                    // Krok 1: rozdziel, by nie zachodziły

                    float overlap = 0.5f * (minDist - distance + 0.01f); // +0.01f zapobiega dzieleniu przez zero
                    float ox = overlap * dx / distance;
                    float oy = overlap * dy / distance;
                    this._x -= ox;
                    this._y -= oy;
                    other._x += ox;
                    other._y += oy;

                    // Krok 2: prawidłowe odbicie:
                    // Wyznacz normalną i styczną zderzenia
                    float nx = dx / distance;
                    float ny = dy / distance;
                    float tx = -ny;
                    float ty = nx;

                    // Składowe wzdłuż osi
                    float dpTan1 = this._vx * tx + this._vy * ty;
                    float dpTan2 = other._vx * tx + other._vy * ty;

                    float dpNorm1 = this._vx * nx + this._vy * ny;
                    float dpNorm2 = other._vx * nx + other._vy * ny;

                    // Zasada zachowania pędu i energii (równe masy dla uproszczenia — jeśli nie, użyj mas...)
                    float m1 = this.Mass;
                    float m2 = other.Mass;
                    float momentum1 = (dpNorm1 * (m1 - m2) + 2 * m2 * dpNorm2) / (m1 + m2);
                    float momentum2 = (dpNorm2 * (m2 - m1) + 2 * m1 * dpNorm1) / (m1 + m2);

                    this._vx = tx * dpTan1 + nx * momentum1;
                    this._vy = ty * dpTan1 + ny * momentum1;
                    other._vx = tx * dpTan2 + nx * momentum2;
                    other._vy = ty * dpTan2 + ny * momentum2;
                }
            }
        }


        private void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
