using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Numerics; // Dodano using dla Vector2
using System; // Dodano using dla ArgumentOutOfRangeException

namespace Logic
{
    public class Ball : INotifyPropertyChanged
    {
        private float _x;
        private float _y;
        private float _vx;
        private float _vy;
        private readonly int _radius;
        private float _mass;

        public float Mass
        {
            get => _mass;
            set
            {
                if (value > 0)
                {
                    _mass = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(Mass), "Mass must be positive.");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? name = null) // Zmieniono na string?
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Ball(float x, float y, int radius, float vx, float vy, float mass)
        {
            _x = x;
            _y = y;
            if (radius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be positive.");
            }
            _radius = radius;

            _vx = vx;
            _vy = vy;
            Mass = mass;
        }

        public float X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(nameof(X)); }
        }

        public float Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(nameof(Y)); }
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

        public int Radius
        {
            get => _radius;
        }

        public double Diameter
        {
            get => _radius * 2.0;
        }

        public void Move(int width, int height)
        {
            _x += _vx;
            _y += _vy;

            bool collidedX = false;
            bool collidedY = false;

            if (_x - _radius < 0)
            {
                _x = _radius;
                _vx = -_vx;
                collidedX = true;
            }
            else if (_x + _radius > width)
            {
                _x = width - _radius;
                _vx = -_vx;
                collidedX = true;
            }

            if (_y - _radius < 0)
            {
                _y = _radius;
                _vy = -_vy;
                collidedY = true;
            }
            else if (_y + _radius > height)
            {
                _y = height - _radius;
                _vy = -_vy;
                collidedY = true;
            }

            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }
    }
}
