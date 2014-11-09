using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace BrickBreakerPong
{
    public class Ball
    {
        public static double Speed; // In constructor
        private enum Angle
        {
            BOTTOM_RIGHT, BOTTOM_LEFT,
            TOP_LEFT, TOP_RIGHT
        };
        private Angle currentAngle;

        private enum Direction { CLOCKWISE, COUNTER_CLOCKWISE };
        private Direction currentDirection;

        public enum Collision { WALLS, SIDES};

        private double _radius;

        public Point Position;
        public double Height;
        public double Width;
        private double Radius
        {
            get
            {
                if (Height > 0.0 && Width > 0.0)
                {
                    _radius = Width / 2.0;
                }
                return _radius;
            }
        }
        private Point Origin
        {
            get { return new Point(Position.X + (Width/2.0), Position.Y + (Height/2.0)); }
        }
        public List<Point> Boundaries
        {
            get
            {

                List<Point> pointLists = new List<Point>();
                Point point;
                for (int i = 0; i < 360; i = i + 1)
                {
                    point = new Point();

                    point.X = (int)Math.Round(Origin.X + Radius * Math.Sin(i));
                    point.Y = (int)(Width + Math.Round(Origin.Y - Radius * Math.Cos(i)));
                    pointLists.Add(point);
                }

                return pointLists;
            }
        }


        public Ball(Point ballPosition, double ballHeight, double ballWidth, double ballSpeed = 20.0)
        {
            this.Height = ballHeight;
            this.Width = ballWidth;
            this.Position = ballPosition;
            Ball.Speed = ballSpeed;
        }

        

        public bool Collides(List<Rectangle> objectBallMayCollideWith)
        {
            List<Point> ballCoordinates = Boundaries;
            var collidedObject = objectBallMayCollideWith.Where(s => ballCoordinates.Any(p =>
                                                                    p.X >= s.Margin.Left - Speed &&
                                                                    p.X <= s.Margin.Left + s.Width + Speed &&
                                                                    p.Y <= s.Margin.Top + s.Height + Speed &&
                                                                    p.Y >= s.Margin.Top));



            return collidedObject.ElementAtOrDefault(0) != null;

            //return (HitsTopWall() ||
            //        HitsBottomWall() ||
            //        HitsLeftPaddle() ||
            //        HitsRightPaddle());
        }
        public void SwitchDirection(Collision objectCollided)
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
                        if (objectCollided == Collision.WALLS)
                        {
                            currentDirection = Direction.COUNTER_CLOCKWISE;
                            currentAngle = Angle.TOP_RIGHT;
                        }
                        else // ball hit right paddle
                            currentAngle = Angle.BOTTOM_LEFT;
                        break;
                    case Angle.TOP_LEFT:
                        if (objectCollided == Collision.WALLS)
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
                        if (objectCollided == Collision.WALLS)
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
                        if (objectCollided == Collision.WALLS)
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
        //private bool HitsTopWall()
        //{
        //    return (this.Position.Top - Speed) <= 0.0;
        //}
        //private bool HitsBottomWall()
        //{
        //    return true; //return (this.Margin.Top + this.Height + SPEED) > mainGrid.Height;
        //}
        //public bool HitsLeftPaddle()
        //{
        //    //GeneralTransform generalTransform = ball.TransformToVisual(leftPaddle);
        //    //Point distanceBetweenLeftPaddleAndBall = generalTransform.TransformPoint(new Point(0, 0));


        //    return true;// return (distanceBetweenLeftPaddleAndBall.X - SPEED <= leftPaddle.Width &&
        //             //distanceBetweenLeftPaddleAndBall.Y - SPEED <= leftPaddle.Height);

        //}
        //private bool HitsRightPaddle()
        //{
        //    //GeneralTransform generalTransform = ball.TransformToVisual(rightPaddle);
        //    //Point distanceBetweenBallAndRightPaddle = generalTransform.TransformPoint(new Point(0, 0));


        //    return true;// return ((distanceBetweenBallAndRightPaddle.X * -1.0) - ball.Width - SPEED <= 0.0 &&
        //             //distanceBetweenBallAndRightPaddle.Y - SPEED <= rightPaddle.Height);
        //}
        public void Move()
        {
            // TODO
            //if (Collides())
                //SwitchDirection();

            // Move [currentDirection] at a [currentAngle] angle
            switch (currentAngle)
            {
                case Angle.BOTTOM_LEFT:
                    this.Position.X -= Speed;
                    this.Position.Y += Speed;
                    break;
                case Angle.BOTTOM_RIGHT:
                    this.Position.X += Speed;
                    this.Position.Y += Speed;
                    break;
                case Angle.TOP_LEFT:
                    this.Position.X -= Speed;
                    this.Position.Y -= Speed;
                    break;
                case Angle.TOP_RIGHT:
                    this.Position.X += Speed;
                    this.Position.Y -= Speed;
                    break;
            }


            //switch (currentAngle)
            //{
            //    case Angle.BOTTOM_LEFT:
            //        this.Position = new Thickness(this.Position.Left - Speed, // pull ball left
            //                                    this.Position.Top + Speed,  // push ball down
            //                                    this.Position.Right,
            //                                    this.Position.Bottom);
            //        break;
            //    case Angle.BOTTOM_RIGHT:
            //        this.Position = new Thickness(this.Position.Left + Speed, // push ball right
            //                                    this.Position.Top + Speed,  // push ball down
            //                                    this.Position.Right,
            //                                    this.Position.Bottom);
            //        break;
            //    case Angle.TOP_LEFT:
            //        this.Position = new Thickness(this.Position.Left - Speed, // pull ball left
            //                                    this.Position.Top - Speed,  // pull ball up
            //                                    this.Position.Right,
            //                                    this.Position.Bottom);
            //        break;
            //    case Angle.TOP_RIGHT:
            //        this.Position = new Thickness(this.Position.Left + Speed, // push ball right
            //                                    this.Position.Top - Speed,  // pull ball up
            //                                    this.Position.Right,
            //                                    this.Position.Bottom);
            //        break;
            //}

            //return this.Position;
        }
    }
}
