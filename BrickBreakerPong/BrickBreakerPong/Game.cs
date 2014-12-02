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
        private const double SPEED = 10.0;

        public static double boardHeight;
        public static double boardWidth;

        int numPlayers;

        public Game(int numOfPlayers, double boardWidth = 0.0, double boardHeight = 0.0)
        {
            this.numPlayers = numOfPlayers;

            if (boardWidth <= 0.0)
                Game.boardWidth = Window.Current.Bounds.Width;
            else
                Game.boardWidth = boardWidth;

            if (boardHeight <= 0.0)
                Game.boardHeight = Window.Current.Bounds.Height;
            else
                Game.boardHeight = boardHeight;

            ball = new Ball();

            leftPaddle = new HumanPaddle("left");
            rightPaddle = new HumanPaddle("right");

            paddles = new List<Rectangle>();

            bricks = new List<Rectangle>();
            bricksCache = new List<Rectangle>();
            
            walls = new List<Rectangle>();

            scoreLeft = Convert.ToInt32(GamePage.leftScore);
            scoreRight = Convert.ToInt32(GamePage.rightScore);
            gamePoint = 10000;

            CreateWalls();
            Reset();
        }

        private void CreateWalls()
        {
            topWall = new Rectangle();
            topWall.HorizontalAlignment = HorizontalAlignment.Left;
            topWall.VerticalAlignment = VerticalAlignment.Top;
            topWall.Width = boardWidth;
            topWall.Height = 50.0;
            topWall.Margin = new Thickness(0, (topWall.Height * -1.0) + 5.0, 0, 0);

            bottomWall = new Rectangle();
            bottomWall.HorizontalAlignment = HorizontalAlignment.Left;
            bottomWall.VerticalAlignment = VerticalAlignment.Top;
            bottomWall.Width = boardWidth;
            bottomWall.Height = 50.0;
            bottomWall.Margin = new Thickness(0, boardHeight - 5.0, 0, 0);

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
            ball.Speed = SPEED;
        }

        // Completely restarts the initial state of the game
        public void Restart()
        {
            scoreLeft = 0;
            scoreRight = 0;
            gamePoint = 10000;

            ResetBricks();
            Reset();
            ball.Speed = SPEED;
        }
        public void NewGame()
        {
            scoreLeft = 0;
            scoreRight = 0;
            gamePoint = 10000;
            bricks.Clear();
            bricksCache.Clear();
            Reset();
            ball.Speed = SPEED;
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

            //ball.SetAngle(paddles);
            ball.Move();
            ball.CollidesWith(walls, false);
            ball.CollidesWith(paddles, false);
            ball.CollidesWith(bricks, true);
            ////ball.WillCollide(walls, false);
            ////ball.WillCollide(paddles, false);
            ////ball.WillCollide(bricks, true);
            //ball.SwitchDirection(ball.WillCollide(walls, false));
            //ball.SwitchDirection(ball.WillCollide(paddles, false));
            //ball.SwitchDirection(ball.WillCollide(bricks, true));
            ////if (ball.Collides(topWall) || ball.Collides(bottomWall))
            //// ball.SwitchDirection();
            paddles.Clear();
            
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
        private bool BallAndPaddleIntersects(HumanPaddle paddle)
        {
            double leftOfPaddle = paddle.Position.X;
            double rightOfPaddle = leftOfPaddle + paddle.Width;
            double topOfPaddle = paddle.Position.Y;
            double bottomOfPaddle = topOfPaddle + paddle.Height;

            return ball.Boundaries.Any(p =>
                                        p.X >= paddle.Position.X  &&
                                        p.X <= rightOfPaddle &&
                                        p.Y <= bottomOfPaddle &&
                                        p.Y >= topOfPaddle);
        }
        private void CheckKeyboardPress()
        {
            byte[] keys = new byte[256];
            GetKeyboardState(keys);

            if (numPlayers == 1)
            {
                //if (keys[(int)VirtualKey.Up] == 128 || keys[(int)VirtualKey.Up] == 129)
                if (rightPaddle.Position.Y + rightPaddle.Height / 2 > ball.Position.Y + ball.Height / 2)
                {
                    MoveRightPaddleUp();
                    if(BallAndPaddleIntersects(rightPaddle))
                    {
                        ball.Position.Y = rightPaddle.Position.Y - ball.Height;
                        ball.Move();
                    }
                }
                //if (keys[(int)VirtualKey.Down] == 128 || keys[(int)VirtualKey.Down] == 129)
                else if (rightPaddle.Position.Y + rightPaddle.Height / 2 < ball.Position.Y + ball.Height / 2)
                {
                    MoveRightPaddleDown();
                    if (BallAndPaddleIntersects(rightPaddle))
                    {
                        ball.Position.Y = rightPaddle.Position.Y + rightPaddle.Height;
                        ball.Move();
                    }
                }
            }
            else if(numPlayers == 2)
            {
                if (keys[(int)VirtualKey.Up] == 128 || keys[(int)VirtualKey.Up] == 129)
                //if (rightPaddle.Position.Y + rightPaddle.Height / 2 > ball.Position.Y + ball.Height / 2)
                {
                    MoveRightPaddleUp();
                    if (BallAndPaddleIntersects(rightPaddle))
                    {
                        ball.Position.Y = rightPaddle.Position.Y - ball.Height;
                        ball.Move();
                    }
                }
                if (keys[(int)VirtualKey.Down] == 128 || keys[(int)VirtualKey.Down] == 129)
                //else if (rightPaddle.Position.Y + rightPaddle.Height / 2 < ball.Position.Y + ball.Height / 2)
                {
                    MoveRightPaddleDown();
                    if (BallAndPaddleIntersects(rightPaddle))
                    {
                        ball.Position.Y = rightPaddle.Position.Y + rightPaddle.Height;
                        ball.Move();
                    }
                }
            }

            if (keys[(int)VirtualKey.W] == 128 || keys[(int)VirtualKey.W] == 129)
            //if (leftPaddle.Position.Y + leftPaddle.Height / 2 > ball.Position.Y + ball.Height / 2)
            {
                MoveLeftPaddleUp();

                if (BallAndPaddleIntersects(leftPaddle))
                {
                    ball.Position.Y = leftPaddle.Position.Y - ball.Height;
                    ball.Move();
                }
            }
            if (keys[(int)VirtualKey.S] == 128 || keys[(int)VirtualKey.S] == 129)
            //else if(leftPaddle.Position.Y + leftPaddle.Height / 2 < ball.Position.Y + ball.Height / 2)
            {
                MoveLeftPaddleDown();
                if (BallAndPaddleIntersects(leftPaddle))
                {
                    ball.Position.Y = leftPaddle.Position.Y + leftPaddle.Height;
                    ball.Move();
                }
            }
            //if (keys[(int)VirtualKey.Escape] == 128 || keys[(int)VirtualKey.Escape] == 129)
            //{
            //    game.gameOver = true;
            //}
        }
        private void MoveLeftPaddleUp()
        {
            if (leftPaddle.Position.Y - HumanPaddle.Speed > topWall.Margin.Top + topWall.Height)
                leftPaddle.MovePaddleUp();
            else
                leftPaddle.Position.Y = topWall.Margin.Top + topWall.Height;
        }
        private void MoveLeftPaddleDown()
        {
            if (leftPaddle.Position.Y + leftPaddle.Height + HumanPaddle.Speed < bottomWall.Margin.Top)
                leftPaddle.MovePaddleDown();
            else
                leftPaddle.Position.Y = bottomWall.Margin.Top - leftPaddle.Height;
        }
        private void MoveRightPaddleUp()
        {
            if (rightPaddle.Position.Y - HumanPaddle.Speed > (topWall.Margin.Top + topWall.Height))
                rightPaddle.MovePaddleUp();
            else
                rightPaddle.Position.Y = topWall.Margin.Top + topWall.Height;
        }
        private void MoveRightPaddleDown()
        {
            if (rightPaddle.Position.Y + rightPaddle.Height + HumanPaddle.Speed < bottomWall.Margin.Top)
                rightPaddle.MovePaddleDown();
            else
                rightPaddle.Position.Y = bottomWall.Margin.Top - rightPaddle.Height;
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
