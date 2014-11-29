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
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;

namespace BrickBreakerPong
{
    public class Game
    {
        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] keystate);

        public Ball ball;
        public List<Rectangle> bricks;
        public List<Rectangle> bricksCache;
        public Rectangle topWall;
        public Rectangle bottomWall;
        private List<Rectangle> walls;
        private List<Rectangle> paddles;
        public HumanPaddle leftPaddle;
        public HumanPaddle rightPaddle;
        private bool gameIsInPlay;
        public bool gameOver;
        private bool isLeftPlayersTurn = true;
        public int scoreLeft;
        public int scoreRight;
        private int gamePoint;

        private double LOSE_ZONE = -5.0;

        public double boardHeight;
        public double boardWidth;

        public Game(double boardWidth = 0.0, double boardHeight = 0.0)
        {
            if (boardWidth <= 0.0)
                this.boardWidth = Window.Current.Bounds.Width;
            else
                this.boardWidth = boardWidth;

            if (boardHeight <= 0.0)
                this.boardHeight = Window.Current.Bounds.Height;
            else
                this.boardHeight = boardHeight;


            ball = new Ball();

            leftPaddle = new HumanPaddle("left");
            rightPaddle = new HumanPaddle("right");

            paddles = new List<Rectangle>();

            bricks = new List<Rectangle>();
            bricksCache = new List<Rectangle>();
            
            walls = new List<Rectangle>();

            scoreLeft = 0;
            scoreRight = 0;
            gamePoint = 10000;

            CreateWalls();
            Reset();
        }

        private void CreateWalls()
        {
            topWall = new Rectangle();
            topWall.HorizontalAlignment = HorizontalAlignment.Left;
            topWall.VerticalAlignment = VerticalAlignment.Top;
            topWall.Margin = new Thickness(0, 0, 0, 0);
            topWall.Width = boardWidth;
            topWall.Height = 0.0;

            bottomWall = new Rectangle();
            bottomWall.HorizontalAlignment = HorizontalAlignment.Left;
            bottomWall.VerticalAlignment = VerticalAlignment.Top;
            bottomWall.Width = boardWidth;
            bottomWall.Height = 0.0;
            bottomWall.Margin = new Thickness(0, boardHeight - bottomWall.Height, 0, 0);

            walls.Add(topWall);
            walls.Add(bottomWall);
        }
        public void Update()
        {
            leftPaddle.Position = new Point(LOSE_ZONE, boardHeight / 2.0 - leftPaddle.Height / 2.0);
            rightPaddle.Position = new Point(boardWidth + (LOSE_ZONE * -1.0) - rightPaddle.Width, boardHeight / 2.0 - rightPaddle.Height / 2.0);
            //ball.Position = new Point(boardWidth / 2.0 + 30.0, boardHeight / 2.0 + 30.0);

            if (isLeftPlayersTurn)
            {
                ball.Position = new Point(leftPaddle.Position.X + leftPaddle.Width + ball.Speed, leftPaddle.Position.Y + leftPaddle.Height / 2.0 - ball.Height / 2.0);
                ball.currentAngle = Ball.Angle.BOTTOM_RIGHT;
                ball.currentDirection = Ball.Direction.COUNTER_CLOCKWISE;
            }
            else
            {
                ball.Position = new Point(rightPaddle.Position.X - ball.Width - ball.Speed, rightPaddle.Position.Y + rightPaddle.Height / 2.0 - ball.Height / 2.0);
                ball.currentAngle = Ball.Angle.BOTTOM_LEFT;
                ball.currentDirection = Ball.Direction.CLOCKWISE;
            }
            //ball.Position = new Point(boardWidth / 2.0, boardHeight / 2.0);
        }
        public void Reset()
        {
            Pause();
            Update();
            gameOver = false;
            ball.Speed = 10.0;
        }

        // Completely restarts the initial state of the game
        public void Restart()
        {
            scoreLeft = 0;
            scoreRight = 0;
            gamePoint = 10000;

            ResetBricks();
            Reset();
            ball.Speed = 10.0;
        }
        public void NewGame()
        {
            scoreLeft = 0;
            scoreRight = 0;
            gamePoint = 10000;
            bricks.Clear();
            bricksCache.Clear();
            Reset();
            ball.Speed = 10.0;
        }
        private void ResetBricks()
        {
            bricks.Clear();
            bricks.AddRange(bricksCache.ToArray());

            foreach (Rectangle brick in bricks)
            {
                brick.Visibility = Visibility.Visible;
            }
        }
        public void Pause()
        {
            if(gameIsInPlay)
                gameIsInPlay = false;
        }
        public bool IsInPlay()
        {
            return gameIsInPlay;
        }
        public void Continue()
        {
            if(!gameIsInPlay)
                gameIsInPlay = true;
        }
        public void Run()
        {
            CheckKeyboardPress();

            paddles.Add(leftPaddle.GetRectangle);
            paddles.Add(rightPaddle.GetRectangle);

            //ball.WillCollide(walls, false);
            //ball.WillCollide(paddles, false);
            //ball.WillCollide(bricks, true);
            ball.SwitchDirection(ball.WillCollide(walls, false));
            ball.SwitchDirection(ball.WillCollide(paddles, false));
            ball.SwitchDirection(ball.WillCollide(bricks, true));
            //if (ball.Collides(topWall) || ball.Collides(bottomWall))
               // ball.SwitchDirection();
            paddles.Clear();
            ball.Move();

            if(bricks.Count == 0)
            {
                if (scoreLeft > scoreRight)
                    gamePoint = scoreLeft + 1;
                else
                    gamePoint = scoreRight + 1;
            }

            if (BallIsOutOfBounds())
            {
                if(IsGameOver())
                {
                    Pause();
                }
                else
                {
                    Reset();
                }
            }
        }
        private void CheckKeyboardPress()
        {
            byte[] keys = new byte[256];
            GetKeyboardState(keys);

            //if (keys[(int)VirtualKey.Up] == 128 || keys[(int)VirtualKey.Up] == 129)
            if(rightPaddle.Position.Y + rightPaddle.Height / 2> ball.Position.Y + ball.Height / 2)
            {
                if (rightPaddle.Position.Y - topWall.Height - HumanPaddle.Speed > 0.0)
                    rightPaddle.MovePaddleUp();
                else
                    rightPaddle.Position.Y = topWall.Height;
            }
            //if (keys[(int)VirtualKey.Down] == 128 || keys[(int)VirtualKey.Down] == 129)
            else if(rightPaddle.Position.Y + rightPaddle.Height / 2 < ball.Position.Y + ball.Height / 2)
            {
                if (rightPaddle.Position.Y + rightPaddle.Height + HumanPaddle.Speed + bottomWall.Height < boardHeight)
                    rightPaddle.MovePaddleDown();
                else
                    rightPaddle.Position.Y = boardHeight - rightPaddle.Height - bottomWall.Height;
            }

            if (keys[(int)VirtualKey.W] == 128 || keys[(int)VirtualKey.W] == 129)
            //if (leftPaddle.Position.Y + leftPaddle.Height / 2 > ball.Position.Y + ball.Height / 2)
            {
                if (leftPaddle.Position.Y - HumanPaddle.Speed - topWall.Height > 0.0)
                    leftPaddle.MovePaddleUp();
                else
                    leftPaddle.Position.Y = topWall.Height;
            }
            if (keys[(int)VirtualKey.S] == 128 || keys[(int)VirtualKey.S] == 129)
            //else if(leftPaddle.Position.Y + leftPaddle.Height / 2 < ball.Position.Y + ball.Height / 2)
            {
                if (leftPaddle.Position.Y + leftPaddle.Height + HumanPaddle.Speed  + bottomWall.Height < boardHeight)
                    leftPaddle.MovePaddleDown();
                else
                    leftPaddle.Position.Y = boardHeight - leftPaddle.Height - bottomWall.Height;
            }
            //if (keys[(int)VirtualKey.Escape] == 128 || keys[(int)VirtualKey.Escape] == 129)
            //{
            //    game.gameOver = true;
            //}
        }

        public void AddBrick(Rectangle brick)
        {
            bricks.Add(brick);
            bricksCache.Add(brick);
        }

        private bool BallIsOutOfBounds()
        {
            if (ball.Position.X + ball.Width < 0.0)
            {
                isLeftPlayersTurn = false;
                scoreRight++;
            }
            if (ball.Position.X > boardWidth)
            {
                isLeftPlayersTurn = true;
                scoreLeft++;
            }

            return (ball.Position.X + ball.Width < 0.0 ||
                    ball.Position.X > boardWidth);
        }

        public bool IsGameOver()
        {
            bool result = false;
            if (scoreLeft == gamePoint || scoreRight == gamePoint)
                result = true;

            return result;
        }
    }
}
