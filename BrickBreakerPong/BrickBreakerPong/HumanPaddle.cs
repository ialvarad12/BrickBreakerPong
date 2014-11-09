using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
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
        public Rectangle GetRectangle
        {
            get
            {
                Rectangle paddle = new Rectangle();

                paddle.HorizontalAlignment = HorizontalAlignment.Left;
                paddle.VerticalAlignment = VerticalAlignment.Top;
                paddle.Margin = new Thickness(Position.X, Position.Y, 0, 0);
                paddle.Height = Height;
                paddle.Width = Width;

                return paddle;
            }
        }
        public Point Position;
        public double Height;
        public double Width;

        public static double Speed;

    }
}
