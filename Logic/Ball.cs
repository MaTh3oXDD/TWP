using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Ball(float x, float y, int radius, float vx, float vy, float mass)
        {
            _x = x;
            _y = y;
            _radius = radius;
            _vx = vx;
            _vy = vy;
            Mass = mass;
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

        public void Run(int width, int height, List<Ball> balls)
        {
            while (true)
            {
                Move(width, height, balls);
                System.Threading.Thread.Sleep(16);
            }
        }

        public void Move(int width, int height, List<Ball> balls)
        {
            _x += _vx;
            _y += _vy;

            if (_x < 0) { _x = 0; _vx = -_vx; }
            if (_x + 2*_radius > width) { _x = width - 2*_radius; _vx = -_vx; }
            if (_y  < 0) { _y = 0; _vy = -_vy; }
            if (_y + 2*_radius > height) { _y = height -2* _radius; _vy = -_vy; }
            
            CheckCollisions(balls);
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        private void CheckCollisions(List<Ball> balls)
        {
            foreach (var other in balls)
            {
                if (other != this && IsColliding(other))
                {
                    ResolveCollision(other);
                }
            }
        }

        private bool IsColliding(Ball other)
        {
            float distanceSquared = Vector2.DistanceSquared(new Vector2(X, Y), new Vector2(other.X, other.Y));
            float radiusSum = Radius + other.Radius;
            return distanceSquared < radiusSum * radiusSum;
        }

        private void ResolveCollision(Ball other)
        {
            Vector2 normal = new Vector2(other.X - X, other.Y - Y);
            float distance = normal.Length();
            float overlap = (Radius + other.Radius) - distance;

            if (overlap > 0)
            {
                normal = Vector2.Normalize(normal);
                Vector2 separation = normal * (overlap / 2f);

                X -= separation.X;
                Y -= separation.Y;
                other.X += separation.X;
                other.Y += separation.Y;

                float velocityDiff = Vector2.Dot(new Vector2(Vx, Vy) - new Vector2(other.Vx, other.Vy), normal);
                if (velocityDiff < 0) return;

                float coefficientOfRestitution = 1f; 
                float j = (-(1 + coefficientOfRestitution) * velocityDiff) / (1 / Mass + 1 / other.Mass);

                Vx += j * normal.X / Mass;
                Vy += j * normal.Y / Mass;

                other.Vx -= j * normal.X / other.Mass;
                other.Vy -= j * normal.Y / other.Mass;
            }
        }
    }
}
