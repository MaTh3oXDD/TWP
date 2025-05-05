using System.ComponentModel;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        // Zmieniamy 'private' na 'public'
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Ball(float x, float y, int radius, float vx, float vy)
        {
            _x = x;
            _y = y;
            _radius = radius;
            _vx = vx;
            _vy = vy;
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

        public int Radius
        {
            get => _radius;
        }

        // Metoda Move aktualizuje pozycję i sprawdza kolizje ze ścianami
        public void Move(int width, int height)
        {
            _x += _vx;
            _y += _vy;

            // Sprawdzamy kolizje ze ścianami
            if (_x - _radius < -15)
            {
                _x = _radius - 15;
                _vx = -_vx;
            }
            else if (_x + _radius > width)
            {
                _x = width - _radius;
                _vx = -_vx;
            }

            if (_y - _radius < -15)
            {
                _y = _radius - 15;
                _vy = -_vy;
            }
            else if (_y + _radius > height)
            {
                _y = height - _radius;
                _vy = -_vy;
            }

            // Powiadamiamy o zmianach w pozycji
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }
    }
}
