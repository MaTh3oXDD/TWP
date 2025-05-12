using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic; // Zakładam, że Ball z Logic jest potrzebne do UpdateBall

namespace Model
{
    public class ModelBall : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _radius;

        public ModelBall(double x, double y, double radius)
        {
            _x = x;
            _y = y;
            _radius = radius;
        }

        public double X
        {
            get => _x;
            // Setter zmieniony na private, ponieważ aktualizacja powinna iść przez UpdateBall
            private set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                    OnPropertyChanged(nameof(DisplayX)); // Zgłoś zmianę dla DisplayX
                }
            }
        }

        public double Y
        {
            get => _y;
            // Setter zmieniony na private
            private set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                    OnPropertyChanged(nameof(DisplayY)); // Zgłoś zmianę dla DisplayY
                }
            }
        }

        public double Radius
        {
            get => _radius;
            // Zakładam, że promień się nie zmienia po utworzeniu.
            // Jeśli się zmienia, dodaj setter i zgłoś zmiany dla Radius, Diameter, DisplayX, DisplayY.
        }

        // Nowe właściwości do powiązania w XAML
        public double DisplayX => X - Radius; // Pozycja X dla lewego górnego rogu elipsy
        public double DisplayY => Y - Radius; // Pozycja Y dla lewego górnego rogu elipsy
        public double Diameter => 2 * Radius; // Średnica dla szerokości i wysokości elipsy

        // Metoda do aktualizacji pozycji kulki z warstwy logiki
        // Upewnij się, że obiekt Ball z warstwy Logic poprawnie implementuje INotifyPropertyChanged
        // i że te zdarzenia są obsługiwane (np. w PoolTable.cs).
        public void UpdateBall(object? sender, PropertyChangedEventArgs e)
        {
            // Upewnij się, że sender jest oczekiwanym typem Ball z warstwy Logic
            if (sender is Logic.Ball logicBall)
            {
                // Zaktualizuj właściwości ModelBall na podstawie danych z Logic.Ball
                // Settery X i Y w ModelBall zajmą się zgłoszeniem PropertyChanged dla siebie i DisplayX/DisplayY
                this.X = logicBall.X;
                this.Y = logicBall.Y;
                // Jeśli Radius w Logic.Ball może się zmieniać, zaktualizuj i zgłoś zmianę dla Radius, Diameter, DisplayX, DisplayY
                // this.Radius = logicBall.Radius;
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
