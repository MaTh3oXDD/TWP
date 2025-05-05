using System.Collections.ObjectModel;

namespace Model
{
    public abstract class ModelAbstractAPI
    {
        public static ModelAbstractAPI CreateAPI()
        {
            return new BallManager();
        }

        public abstract void StartGame();
        public abstract void StopGame();
        public abstract void CreateBalls(int ballsQuantity, int radius);
        public abstract ObservableCollection<ModelBall> Balls { get; }
        public abstract ObservableCollection<ModelBall> GetBalls();

    }
}
