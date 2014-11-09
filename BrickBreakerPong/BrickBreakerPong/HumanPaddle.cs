using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
namespace BrickBreakerPong
{
    public class HumanPaddle
    {
        public HumanPaddle(Point initialPaddlePosition, double paddleWidth, double paddleHeight, double paddleSpeed = 20.0)
        {
            this.Position = initialPaddlePosition;
            this.Width = paddleWidth;
            this.Height = paddleHeight;
            HumanPaddle.Speed = paddleSpeed;
        }
        public void MovePaddleUp()
        {
            Position.Y -= Speed;
        }
        public void MovePaddleDown()
        {
            Position.Y += Speed;
        }
        List<Point> RectangularCoordinates
        {
            get
            {
                List<Point> pointList = new List<Point>();

                pointList.Add(new Point(Position.X, Position.Y));
                pointList.Add(new Point(Position.X + Width, Position.Y));
                pointList.Add(new Point(Position.X, Position.Y + Height));
                pointList.Add(new Point(Position.X + Width, Position.Y + Height));

                return pointList;
            }
        }
        public Point Position;
        public double Height;
        public double Width;

        public static double Speed;

    }
}
