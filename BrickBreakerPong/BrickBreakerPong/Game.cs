using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Core;
using System.Runtime.InteropServices;
using Windows.System;

namespace BrickBreakerPong
{
    public class Game
    {

        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] keystate);

        public Ball ball;
        private List<Point> bricks;
        //private List<Point> topWall;
        //private List<Point> bottomWall;
        public HumanPaddle leftPaddle;
        public HumanPaddle rightPaddle;
        public  IPaddle currentPlayer;
        public bool gameIsInPlay;
        public bool gameOver;


        public double boardHeight;
        public double boardWidth;
        private const double LOSE_ZONE = -5.0;
        public Game(double boardWidth = 0.0,
                    double boardHeight = 0.0)
        {
            
            if (boardWidth <= 0.0)
                this.boardWidth = Window.Current.Bounds.Width;
            else
                this.boardWidth = boardWidth;

            if (boardHeight <= 0.0)
                this.boardHeight = Window.Current.Bounds.Height;
            else
                this.boardHeight = boardHeight;


            Reset();

        }

        private void Reset()
        {
            // Reset Paddles
            Point leftPaddlePosition = new Point(LOSE_ZONE, (boardHeight / 2.0) - (200.0 / 2.0));
            Point rightPaddlePosition = new Point (boardWidth - (LOSE_ZONE * -1.0) - 50.0, (boardHeight / 2.0) - (200.0 / 2.0));
            leftPaddle = new HumanPaddle(leftPaddlePosition, 50.0, 200.0);
            rightPaddle = new HumanPaddle(rightPaddlePosition, 50.0, 200.0);

            // Reset ball
            Point ballPosition = new Point(boardWidth / 2.0, boardHeight / 2.0);
            ball = new Ball(ballPosition, 50.0, 50.0, HumanPaddle.Speed / 3.0);

            // Create walls
            //topWall.Add(new Point(0, 0));
            //topWall.Add(new Point(boardWidth, 0));
            //bottomWall.Add(new Point(0, boardHeight));
            //bottomWall.Add(new Point(boardWidth, boardHeight));

            gameIsInPlay = false;
            gameOver = false;
        }

        public void Run()
        {
            CheckKeyboardPress();
            //if (ball.Collides(topWall) || ball.Collides(bottomWall))
               // ball.SwitchDirection();
            ball.Move();
        }
        private void CheckKeyboardPress()
        {
            byte[] keys = new byte[256];
            GetKeyboardState(keys);

            if (keys[(int)VirtualKey.Up] == 128 || keys[(int)VirtualKey.Up] == 129)
            {
                if (rightPaddle.Position.Y - HumanPaddle.Speed > 0.0)
                    rightPaddle.MovePaddleUp();
                else
                    rightPaddle.Position.Y = 0.0;
            }
            if (keys[(int)VirtualKey.Down] == 128 || keys[(int)VirtualKey.Down] == 129)
            {

                if (rightPaddle.Position.Y + rightPaddle.Height + HumanPaddle.Speed < boardHeight)
                    rightPaddle.MovePaddleDown();
                else
                    rightPaddle.Position.Y = boardHeight - rightPaddle.Height;
            }

            if (keys[(int)VirtualKey.W] == 128 || keys[(int)VirtualKey.W] == 129)
            //if (leftPaddle.Margin.Top + leftPaddle.Height / 2.0  > ball.Margin.Top + ball.Height / 2.0)
            {
                if (leftPaddle.Position.Y - HumanPaddle.Speed > 0.0)
                    leftPaddle.MovePaddleUp();
                else
                    leftPaddle.Position.Y = 0.0;
            }
            if (keys[(int)VirtualKey.S] == 128 || keys[(int)VirtualKey.S] == 129)
            //else if(leftPaddle.Margin.Top + leftPaddle.Height / 2.0  < ball.Margin.Top + ball.Height / 2.0)
            {
                if (leftPaddle.Position.Y + rightPaddle.Height + HumanPaddle.Speed < boardHeight)
                    leftPaddle.MovePaddleDown();
                else
                    leftPaddle.Position.Y = boardHeight - leftPaddle.Height;
            }
            //if (keys[(int)VirtualKey.Escape] == 128 || keys[(int)VirtualKey.Escape] == 129)
            //{
            //    game.gameOver = true;
            //}
        }


        private bool BallIsOutOfBounds()
        {
            return (ball.Position.X + ball.Width < 0.0 ||
                    ball.Position.X > boardWidth);
        }
    }
}
