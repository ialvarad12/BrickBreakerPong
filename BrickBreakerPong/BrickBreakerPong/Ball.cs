using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace BrickBreakerPong
{
    public class Ball
    {
        private double SPEED; // In constructor
        private enum Angle
        {
            BOTTOM_RIGHT, BOTTOM_LEFT,
            TOP_LEFT, TOP_RIGHT
        };
        private Angle currentAngle;

        private enum Direction { CLOCKWISE, COUNTER_CLOCKWISE };
        private Direction currentDirection;

        private double Height;
        private double Width;

        private Thickness Margin;

        public Ball(int speed, double height, double width, Thickness margin)
        {
            this.Height = height;
            this.Width = width;
            this.Margin = margin;
            SPEED = speed;
        }

        private bool Collides()
        {
            return (HitsTopWall() ||
                    HitsBottomWall() ||
                    HitsLeftPaddle() ||
                    HitsRightPaddle());
        }
        private void SwitchDirection()
        {
            // If the angle is switching, it is assumed that the  collided
            if (currentDirection == Direction.CLOCKWISE)
            {
                switch (currentAngle)
                {
                    case Angle.BOTTOM_LEFT:
                        currentAngle = Angle.TOP_LEFT;
                        break;
                    case Angle.BOTTOM_RIGHT:
                        if (HitsBottomWall())
                        {
                            currentDirection = Direction.COUNTER_CLOCKWISE;
                            currentAngle = Angle.TOP_RIGHT;
                        }
                        else // ball hit right paddle
                            currentAngle = Angle.BOTTOM_LEFT;
                        break;
                    case Angle.TOP_LEFT:
                        if (HitsTopWall())
                        {
                            currentDirection = Direction.COUNTER_CLOCKWISE;
                            currentAngle = Angle.BOTTOM_LEFT;
                        }
                        else // ball hit left paddle
                            currentAngle = Angle.TOP_RIGHT;
                        break;
                    case Angle.TOP_RIGHT:
                        currentAngle = Angle.BOTTOM_RIGHT;
                        break;
                }
            }
            else // Counter-clockwise
            {
                switch (currentAngle)
                {
                    case Angle.BOTTOM_LEFT:
                        if (HitsBottomWall())
                        {
                            currentDirection = Direction.CLOCKWISE;
                            currentAngle = Angle.TOP_LEFT;
                        }
                        else // ball hit left paddle
                            currentAngle = Angle.BOTTOM_RIGHT;
                        break;
                    case Angle.BOTTOM_RIGHT:
                        currentAngle = Angle.TOP_RIGHT;
                        break;
                    case Angle.TOP_LEFT:
                        currentAngle = Angle.BOTTOM_LEFT;
                        break;
                    case Angle.TOP_RIGHT:
                        if (HitsTopWall())
                        {
                            currentDirection = Direction.CLOCKWISE;
                            currentAngle = Angle.BOTTOM_RIGHT;
                        }
                        else // ball hit right paddle
                            currentAngle = Angle.TOP_LEFT;
                        break;
                }
            }
        }
        private bool HitsTopWall()
        {
            return (this.Margin.Top - SPEED) <= 0.0;
        }
        private bool HitsBottomWall()
        {
            return true; //return (this.Margin.Top + this.Height + SPEED) > mainGrid.Height;
        }
        public bool HitsLeftPaddle()
        {
            //GeneralTransform generalTransform = ball.TransformToVisual(leftPaddle);
            //Point distanceBetweenLeftPaddleAndBall = generalTransform.TransformPoint(new Point(0, 0));


            return true;// return (distanceBetweenLeftPaddleAndBall.X - SPEED <= leftPaddle.Width &&
                     //distanceBetweenLeftPaddleAndBall.Y - SPEED <= leftPaddle.Height);

        }
        private bool HitsRightPaddle()
        {
            //GeneralTransform generalTransform = ball.TransformToVisual(rightPaddle);
            //Point distanceBetweenBallAndRightPaddle = generalTransform.TransformPoint(new Point(0, 0));


            return true;// return ((distanceBetweenBallAndRightPaddle.X * -1.0) - ball.Width - SPEED <= 0.0 &&
                     //distanceBetweenBallAndRightPaddle.Y - SPEED <= rightPaddle.Height);
        }
        public Thickness Move()
        {
            if (Collides())
                SwitchDirection();

            // Move [currentDirection] at a [currentAngle] angle
            switch (currentAngle)
            {
                case Angle.BOTTOM_LEFT:
                    this.Margin = new Thickness(this.Margin.Left - SPEED, // pull ball left
                                                this.Margin.Top + SPEED,  // push ball down
                                                this.Margin.Right,
                                                this.Margin.Bottom);
                    break;
                case Angle.BOTTOM_RIGHT:
                    this.Margin = new Thickness(this.Margin.Left + SPEED, // push ball right
                                                this.Margin.Top + SPEED,  // push ball down
                                                this.Margin.Right,
                                                this.Margin.Bottom);
                    break;
                case Angle.TOP_LEFT:
                    this.Margin = new Thickness(this.Margin.Left - SPEED, // pull ball left
                                                this.Margin.Top - SPEED,  // pull ball up
                                                this.Margin.Right,
                                                this.Margin.Bottom);
                    break;
                case Angle.TOP_RIGHT:
                    this.Margin = new Thickness(this.Margin.Left + SPEED, // push ball right
                                                this.Margin.Top - SPEED,  // pull ball up
                                                this.Margin.Right,
                                                this.Margin.Bottom);
                    break;
            }

            return this.Margin;
        }
    }
}
