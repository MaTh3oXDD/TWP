using Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    internal class BallManager : ModelAbstractAPI
    {
        private LogicAbstractAPI poolTable = LogicAbstractAPI.CreateApi();
        private ObservableCollection<ModelBall> balls = new ObservableCollection<ModelBall>();

        public override void CreateBalls(int ballsQuantity, int radius)
        {
            poolTable.CreateBalls(ballsQuantity, radius);
        }

        public override ObservableCollection<ModelBall> GetBalls()
        {
            balls.Clear();
            foreach (Ball ball in poolTable.GetAllBalls())
            {
                ModelBall b = new ModelBall(ball.X, ball.Y, ball.Radius);
                balls.Add(b);
                ball.PropertyChanged += b.UpdateBall!;
            }
            return balls;
        }

        public override ObservableCollection<ModelBall> Balls => balls;
        

        public override void StartGame()
        {
            poolTable.StartGame();
        }

        public override void StopGame()
        {
            poolTable.StopGame();
        }
    }
}
