﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices;
using Windows.Graphics.Display;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BrickBreakerPong
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] keystate);

        private bool gameIsInPlay = true;
        private DispatcherTimer timer;
        private double BALL_SPEED; // In reset game
        private const double PADDLE_SPEED = 25.0;
        private const double LOSE_ZONE = -5.0;
        private enum BallAngle { BOTTOM_RIGHT, BOTTOM_LEFT,
                                     TOP_LEFT, TOP_RIGHT };
        private enum BallDirection { CLOCKWISE, COUNTER_CLOCKWISE };
        private BallAngle currentBallAngle;
        private BallDirection currentBallDirection;

        #region EVENTS
        void MainPage_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey.ToString() == "Space")
            {
                if (gameIsInPlay)
                    gameIsInPlay = false;
                else
                    gameIsInPlay = true;
            }
        }
        void timer_Tick(object sender, object e)
        {
            if (gameIsInPlay)
            {
                CheckKeyboardPress();
                MoveBall();

           
            }
        }
        #endregion

        public MainPage()
        {
            this.InitializeComponent();

            mainGrid.Height = Window.Current.Bounds.Height;
            mainGrid.Width = Window.Current.Bounds.Width;

            ResetGame();

            CoreWindow.GetForCurrentThread().KeyDown += MainPage_KeyDown;

            timer = new DispatcherTimer();
            timer.Start();
            timer.Tick += timer_Tick;
        }

        
        private void ResetGame()
        {
            leftPaddle.Margin = new Thickness(LOSE_ZONE, (mainGrid.Height / 2.0) - (leftPaddle.Height / 2.0),
                                               0, 0);
            rightPaddle.Margin = new Thickness(0, (mainGrid.Height / 2.0) - (leftPaddle.Height / 2.0),
                                              LOSE_ZONE, 0);

            ball.Margin = new Thickness((mainGrid.Width / 2.0) - (ball.Width / 2.0) - 200.0, (mainGrid.Height / 2.0) - (ball.Height / 2.0),
                                              0, 0);
            BALL_SPEED = PADDLE_SPEED / 3.0;
            currentBallAngle = BallAngle.TOP_RIGHT;
            currentBallDirection = BallDirection.CLOCKWISE;
            gameIsInPlay = false;
        }
       
        private void CheckKeyboardPress()
        {
            byte[] keys = new byte[256];
            GetKeyboardState(keys);

            if (keys[(int)VirtualKey.Up] == 128 || keys[(int)VirtualKey.Up] == 129)
            {
                MoveRightPaddleUp();
            }
            if (keys[(int)VirtualKey.Down] == 128 || keys[(int)VirtualKey.Down] == 129)
            {
                MoveRightPaddleDown();
            }

            if (keys[(int)VirtualKey.W] == 128 || keys[(int)VirtualKey.W] == 129)
            //if (leftPaddle.Margin.Top + leftPaddle.Height / 2.0  > ball.Margin.Top + ball.Height / 2.0)
            {
                MoveLeftPaddleUp();
            }
            if (keys[(int)VirtualKey.S] == 128 || keys[(int)VirtualKey.S] == 129)
            //else if(leftPaddle.Margin.Top + leftPaddle.Height / 2.0  < ball.Margin.Top + ball.Height / 2.0)
            {
                MoveLeftPaddleDown();
            }
        }

        #region PADDLE FUNCTIONS
        private void MoveRightPaddleDown()
        {
            if (rightPaddle.Margin.Top + rightPaddle.Height != mainGrid.Height) // No need to move down
            {
                // Prevent the right paddle from going beneath the bottom wall
                if (rightPaddle.Margin.Top + rightPaddle.Height + PADDLE_SPEED > mainGrid.Height)
                    rightPaddle.Margin = new Thickness(rightPaddle.Margin.Left,
                                                            mainGrid.Height - rightPaddle.Height,
                                                            rightPaddle.Margin.Right,
                                                            rightPaddle.Margin.Bottom);
                else // if right paddle is above the bottom wall, move down
                    rightPaddle.Margin = new Thickness(rightPaddle.Margin.Left,
                                                            rightPaddle.Margin.Top + PADDLE_SPEED,
                                                            rightPaddle.Margin.Right,
                                                            rightPaddle.Margin.Bottom);
            }
            
        }
        private void MoveRightPaddleUp()
        {
            if (rightPaddle.Margin.Top != 0.0) // No need to move up
            {
                // Prevent the right paddle from going above the top wall
                if (rightPaddle.Margin.Top - PADDLE_SPEED < 0.0)
                    rightPaddle.Margin = new Thickness(rightPaddle.Margin.Left,
                                                        0.0,
                                                        rightPaddle.Margin.Right,
                                                        rightPaddle.Margin.Bottom);
                else // if right paddle is below the top wall, move up
                    rightPaddle.Margin = new Thickness(rightPaddle.Margin.Left,
                                                        rightPaddle.Margin.Top - PADDLE_SPEED,
                                                        rightPaddle.Margin.Right,
                                                        rightPaddle.Margin.Bottom);
            }
        }
        private void MoveLeftPaddleUp()
        {
            if (leftPaddle.Margin.Top != 0.0) // No need to move up
            {
                // Prevent the left paddle from going above the top wall
                if (leftPaddle.Margin.Top - PADDLE_SPEED < 0.0)
                    leftPaddle.Margin = new Thickness(leftPaddle.Margin.Left,
                                                        0.0,
                                                        leftPaddle.Margin.Right,
                                                        leftPaddle.Margin.Bottom);
                else // if left paddle is below the top wall, move up
                    leftPaddle.Margin = new Thickness(leftPaddle.Margin.Left,
                                                            leftPaddle.Margin.Top - PADDLE_SPEED,
                                                            leftPaddle.Margin.Right,
                                                            leftPaddle.Margin.Bottom);
            }
        }
        private void MoveLeftPaddleDown()
        {
            if (leftPaddle.Margin.Top + leftPaddle.Height != mainGrid.Height) // No need to move down
            {
                // Prevent the left paddle from going beneath the bottom wall
                if (leftPaddle.Margin.Top + leftPaddle.Height + PADDLE_SPEED > mainGrid.Height)
                    leftPaddle.Margin = new Thickness(leftPaddle.Margin.Left,
                                                            mainGrid.Height - leftPaddle.Height,
                                                            leftPaddle.Margin.Right,
                                                            leftPaddle.Margin.Bottom);
                else // if left paddle is above the bottom wall, mvove down
                    leftPaddle.Margin = new Thickness(leftPaddle.Margin.Left,
                                                    leftPaddle.Margin.Top + PADDLE_SPEED,
                                                    leftPaddle.Margin.Right,
                                                    leftPaddle.Margin.Bottom);
            }
        }
#endregion
        #region BALL FUNCTIONS
        private void MoveBall()
        {
            if (BallCollides())
                  SwitchDirection();

            if (BallIsOutOfBounds())
                ResetGame();
            // Move [currentBallDirection] at a [currentBallAngle] angle
            switch(currentBallAngle)
            {
                case BallAngle.BOTTOM_LEFT:
                    ball.Margin = new Thickness(ball.Margin.Left - BALL_SPEED, // pull ball left
                                                ball.Margin.Top + BALL_SPEED,  // push ball down
                                                ball.Margin.Right,
                                                ball.Margin.Bottom);
                    break;
                case BallAngle.BOTTOM_RIGHT:
                    ball.Margin = new Thickness(ball.Margin.Left + BALL_SPEED, // push ball right
                                                ball.Margin.Top + BALL_SPEED,  // push ball down
                                                ball.Margin.Right,
                                                ball.Margin.Bottom);
                    break;
                case BallAngle.TOP_LEFT:
                    ball.Margin = new Thickness(ball.Margin.Left - BALL_SPEED, // pull ball left
                                                ball.Margin.Top - BALL_SPEED,  // pull ball up
                                                ball.Margin.Right,
                                                ball.Margin.Bottom);
                    break;
                case BallAngle.TOP_RIGHT:
                    ball.Margin = new Thickness(ball.Margin.Left + BALL_SPEED, // push ball right
                                                ball.Margin.Top - BALL_SPEED,  // pull ball up
                                                ball.Margin.Right, 
                                                ball.Margin.Bottom);
                    break;
            }
        }
        private void SwitchDirection()
        {
            // If the angle is switching, it is assumed that the ball collided
            if (currentBallDirection == BallDirection.CLOCKWISE)
            {
                switch (currentBallAngle)
                {
                    case BallAngle.BOTTOM_LEFT:
                        currentBallAngle = BallAngle.TOP_LEFT;
                        break;
                    case BallAngle.BOTTOM_RIGHT:
                        if (BallHitsBottomWall()) // || BallHitsTopOfRightPaddle()
                        {
                            currentBallDirection = BallDirection.COUNTER_CLOCKWISE;
                            currentBallAngle = BallAngle.TOP_RIGHT;
                        }
                        else // ball hit right paddle
                            currentBallAngle = BallAngle.BOTTOM_LEFT;
                        break;
                    case BallAngle.TOP_LEFT:
                        if (BallHitsTopWall())
                        {
                            currentBallDirection = BallDirection.COUNTER_CLOCKWISE;
                            currentBallAngle = BallAngle.BOTTOM_LEFT;
                        }
                        else // ball hit left paddle
                            currentBallAngle = BallAngle.TOP_RIGHT;
                        break;
                    case BallAngle.TOP_RIGHT:
                        currentBallAngle = BallAngle.BOTTOM_RIGHT;
                        break;
                }
            }
            else // Counter-clockwise
            {
                switch (currentBallAngle)
                {
                    case BallAngle.BOTTOM_LEFT:
                        if (BallHitsBottomWall())
                        {
                            currentBallDirection = BallDirection.CLOCKWISE;
                            currentBallAngle = BallAngle.TOP_LEFT;
                        }
                        else // ball hit left paddle
                            currentBallAngle = BallAngle.BOTTOM_RIGHT;
                        break;
                    case BallAngle.BOTTOM_RIGHT:
                        currentBallAngle = BallAngle.TOP_RIGHT;
                        break;
                    case BallAngle.TOP_LEFT:
                        currentBallAngle = BallAngle.BOTTOM_LEFT;
                        break;
                    case BallAngle.TOP_RIGHT:
                        if (BallHitsTopWall())
                        {
                            currentBallDirection = BallDirection.CLOCKWISE;
                            currentBallAngle = BallAngle.BOTTOM_RIGHT;
                        }
                        else // ball hit right paddle
                            currentBallAngle = BallAngle.TOP_LEFT;
                        break;
                }
            }
        }
        private bool BallCollides()
        {
            return (BallHitsTopWall()    ||
                    BallHitsBottomWall() ||
                    BallHitsLeftPaddle() || 
                    BallHitsRightPaddle());
        }
        private bool BallHitsTopWall()
        {
            return (ball.Margin.Top - BALL_SPEED) <= 0.0;
        }
        private bool BallHitsBottomWall()
        {
            return (ball.Margin.Top + ball.Height + BALL_SPEED) >= mainGrid.Height;
        }
        private bool BallHitsLeftPaddle()
        {
            List<Point> ballCoordinates = GetCircularPoints();
            bool ballHitPaddle = ballCoordinates.Any(p => p.X >= leftPaddle.Margin.Left &&
                                     p.X <= leftPaddle.Margin.Left + leftPaddle.Width + BALL_SPEED &&
                                     p.Y <= leftPaddle.Margin.Top + leftPaddle.Height &&
                                     p.Y >= leftPaddle.Margin.Top);

            return ballHitPaddle;

            // Where is the ball in reference to the left paddle?
            //GeneralTransform generalTransform = ball.TransformToVisual(leftPaddle);
            //Point distanceBetweenLeftPaddleAndBall = generalTransform.TransformPoint(new Point(0, 0));

            //distanceBetweenLeftPaddleAndBall.Y *= -1.0; // Regular coordinate system. 
            //distanceBetweenLeftPaddleAndBall.X -= leftPaddle.Width; //(0,0) at top-right corner of left paddle
            

            //double ballRadius = ball.Width / 2.0;

            //// Ball hits top corner of the left paddle
            //if (currentBallAngle == BallAngle.BOTTOM_LEFT && 
            //    distanceBetweenLeftPaddleAndBall.X - BALL_SPEED <= ballRadius &&  
            //    distanceBetweenLeftPaddleAndBall.Y - BALL_SPEED <= ballRadius)
            //{
            //    currentBallAngle = BallAngle.TOP_RIGHT;
            //    currentBallDirection = BallDirection.CLOCKWISE;
            //    return true;
            //}

            //return (distanceBetweenLeftPaddleAndBall.X - BALL_SPEED <= 0.0   &&
            //        distanceBetweenLeftPaddleAndBall.X - BALL_SPEED >= (leftPaddle.Width * -1.0) &&
            //       (distanceBetweenLeftPaddleAndBall.Y - BALL_SPEED <= ball.Height &&
            //        distanceBetweenLeftPaddleAndBall.Y - BALL_SPEED >= (leftPaddle.Height * -1.0)));
           

            
        }

        private List<Point> GetCircularPoints()
        {
            int distance = (int)ball.Width / 2;
            double originX = ball.Margin.Left + distance;
            double originY = ball.Margin.Top - distance;

            List<Point> pointLists = new List<Point>();
            Point point;
            for (int i = 0; i < 360; i = i + 1)
            {
                point = new Point();

                point.X = (int)Math.Round(originX + distance * Math.Sin(i));
                point.Y = (int)(ball.Width + Math.Round(originY - distance * Math.Cos(i)));
                pointLists.Add(point);
            }

            return pointLists;
        }
        private bool BallHitsRightPaddle()
        {

            List<Point> ballCoordinates = GetCircularPoints();
            bool ballHitPaddle = ballCoordinates.Any(p => p.X >= (mainGrid.Width - rightPaddle.Margin.Right -  + rightPaddle.Width) -BALL_SPEED &&
                                     p.X <= (mainGrid.Width - rightPaddle.Margin.Right) &&
                                     p.Y <= rightPaddle.Margin.Top + rightPaddle.Height &&
                                     p.Y >= rightPaddle.Margin.Top);

            return ballHitPaddle;

            //// Where is the right paddle in reference to the ball?
            //GeneralTransform generalTransform = rightPaddle.TransformToVisual(ball);
            //Point distanceBetweenBallAndRightPaddle = generalTransform.TransformPoint(new Point(0, 0));

            //distanceBetweenBallAndRightPaddle.X -= ball.Width; // (0,0) at the top-right corner of the ball

            //return (distanceBetweenBallAndRightPaddle.X  - BALL_SPEED <= 0.0 &&
            //        distanceBetweenBallAndRightPaddle.X  - BALL_SPEED >= (rightPaddle.Width * -1.0) &&
            //        (distanceBetweenBallAndRightPaddle.Y - BALL_SPEED <=  ball.Height &&
            //         distanceBetweenBallAndRightPaddle.Y - BALL_SPEED >= (rightPaddle.Height * -1.0)));
        }
        private bool BallIsOutOfBounds()
        {
            return (ball.Margin.Left <= 0.0 ||
                    ball.Margin.Left > mainGrid.Width);
        }
        
        #endregion
    }
}