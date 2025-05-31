using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic;

namespace Model
{
    public class ModelBall : INotifyPropertyChanged
    {
        private float _x;
        private float _y;
        private readonly int _radius;

        public ModelBall(float x, float y, int radius)
        {
            _x = x;
            _y = y;
            _radius = radius;
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

        public int Radius => _radius;

        public void UpdateBall(object s, PropertyChangedEventArgs e)
        {
            Ball ball = (Ball)s;
            if (e.PropertyName == nameof(Ball.X))
                X = ball.X;
            else if (e.PropertyName == nameof(Ball.Y))
                Y = ball.Y;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
