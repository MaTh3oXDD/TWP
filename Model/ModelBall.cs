using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
            set {
                _x = value;
                OnPropertyChanged();
            } 
        }

        public float Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        }

        public int Radius
        {
            get => _radius;
        }

        public void UpdateBall(Object s, PropertyChangedEventArgs e)
        {
           Ball ball = (Ball)s;
            if(e.PropertyName == "X")
            {
                X = ball.X;
            }
            else if (e.PropertyName == "Y")
            {
                Y = ball.Y;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
