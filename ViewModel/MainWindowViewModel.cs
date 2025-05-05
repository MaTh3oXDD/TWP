using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model;

namespace ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Model.ModelAbstractAPI _model;
        private String _BallsAmmount = "";
        public RelayCommand StartButton { get; }
        public RelayCommand StopButton { get; }
        public ObservableCollection<ModelBall> Balls => _model.GetBalls();

        public MainWindowViewModel()
        {
            _model = Model.ModelAbstractAPI.CreateAPI();
            StartButton = new RelayCommand(StartGame, CanStartGame);
            StopButton = new RelayCommand(StopGame, CanStopGame);
        }

        public String BallsAmmount 
        { 
            get => _BallsAmmount; 
            set { 
                _BallsAmmount = value;
                OnPropertyChanged();
                StartButton.RaiseCanExecuteChanged();
            } 
        }

        private bool CanStartGame()
        {
            return Balls.Count == 0 && BallsAmmount != "";
        }

        public void StartGame()
        {
            int ballsAmount = int.Parse(BallsAmmount);
            _model.CreateBalls(ballsAmount, 15);
            _model.StartGame();
            OnPropertyChanged("Balls");
            StopButton.RaiseCanExecuteChanged();
            StartButton.RaiseCanExecuteChanged();
        }

        private bool CanStopGame()
        {
            return Balls.Count > 0;
        }

        public void StopGame()
        {
            _model.StopGame();
            OnPropertyChanged("Balls");
            StopButton.RaiseCanExecuteChanged();
            StartButton.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
