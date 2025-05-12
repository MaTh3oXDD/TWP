using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Numerics; // Dodano using dla Vector2
using System; // Dodano using dla ArgumentOutOfRangeException

namespace Logic
{
    public class Ball : INotifyPropertyChanged
    {
        private readonly object _lock = new object();

        private float _x;
        private float _y;
        private float _vx;
        private float _vy;
        private readonly int _radius;
        private float _mass;

        public float Mass
        {
            get
            {
                lock (_lock)
                {
                    return _mass;
                }
            }
            set
            {
                if (value > 0)
                {
                    lock (_lock)
                    {
                        _mass = value;
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(Mass), "Mass must be positive.");
                }

            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? name = null)
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
            get
            {
                lock (_lock)
                {
                    return _x;
                }
            }
            set
            {
                lock (_lock)
                {
                    _x = value;
                }
                OnPropertyChanged(nameof(X));
            }
        }

        public float Y
        {
            get
            {
                lock (_lock)
                {
                    return _y;
                }
            }
            set
            {
                lock (_lock)
                {
                    _y = value;
                }
                OnPropertyChanged(nameof(Y));
            }
        }

        public float Vx
        {
            get
            {
                lock (_lock)
                {
                    return _vx;
                }
            }
            set
            {
                lock (_lock)
                {
                    _vx = value;
                }
            }
        }

        public float Vy
        {
            get
            {
                lock (_lock)
                {
                    return _vy;
                }
            }
            set
            {
                lock (_lock)
                {
                    _vy = value;
                }
            }
        }

        public int Radius
        {
            get
            {
                return _radius;
            }
        }

        public double Diameter
        {
            get
            {
                return _radius * 2.0;
            }
        }

        public void Move(int width, int height)
        {
            lock (_lock)
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
            }
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }
    }
}
