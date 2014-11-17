using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace BrickBreakerPong
{
    public class Ball
    {
        #region MOVEMENT
        public static double Speed; // In constructor
        public enum Angle
        {
            BOTTOM_RIGHT, BOTTOM_LEFT,
            TOP_LEFT, TOP_RIGHT
        };
        public Angle currentAngle;

        public enum Direction { CLOCKWISE, COUNTER_CLOCKWISE };
        public Direction currentDirection;

        public enum Collision { TOP, BOTTOM, LEFT, RIGHT, NONE};
        #endregion

        #region DIMENSIONS
        private double _radius;
        private double _width;
        private double _height;

        public Point Position;
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
        public double Width
        {
            get { return _width; }
            set
            {
                if (value < 0.0)
                    _width = 0.0;
                else
                    _width = value;
            }
        }
        public double Height
        {
            get { return _height; }
            set
            {
                if (value < 0.0)
                    _height = 0.0;
                else
                    _height = value;
            }
        }
        private Point Origin
        {
            get { return new Point(Position.X + (Width/2.0), Position.Y - (Height/2.0)); }
        }
        public List<Point> Boundaries
        {
            get
            {
                List<Point> pointLists = new List<Point>();
                Point point;
                for (int i = 0; i < 360; i = i + 24)
                {
                    point = new Point();

                    point.X = (int)Math.Round(Origin.X + (int)Radius * Math.Sin(i));
                    point.Y = (int)(Width + Math.Round(Origin.Y - (int)Radius * Math.Cos(i)));
                    pointLists.Add(point);
                }

                return pointLists;
            }
        }
        #endregion
        TimeSpan bounce;
        public Ball(double ballWidth = 50.0, double ballHeight = 50.0, double ballSpeed = 10.0)
        {
            this.Width = ballWidth;
            this.Height = ballHeight;
            this.Position = new Point(0, 0);
            Ball.Speed = ballSpeed;
            bounce = new TimeSpan(DateTime.Now.Ticks);
            currentAngle = Angle.BOTTOM_RIGHT;
            currentDirection = Direction.COUNTER_CLOCKWISE;
        }
        public Collision[] WillCollide(List<Rectangle> objectBallMayCollideWith, bool deleteObject)
        {
            List<Point> ballCoordinates = Boundaries;
            var collidedObject = objectBallMayCollideWith.Where(s => ballCoordinates.Any(p =>
                                                                    p.X >= s.Margin.Left - Speed &&
                                                                    p.X <= s.Margin.Left + s.Width + Speed &&
                                                                    p.Y <= s.Margin.Top + s.Height + Speed &&
                                                                    p.Y >= s.Margin.Top - Speed));

 

            Collision[] whereBallWillCollide = new Collision[2]; 
            if (collidedObject.ElementAtOrDefault(0) != null)
            {
                MainPage.sfx.Play();
                 Rectangle rec = collidedObject.ElementAt(0);
                 if (ballCoordinates.All(p =>
                                         p.Y + Speed >= rec.Margin.Top + rec.Height))
                     whereBallWillCollide[0] = Collision.BOTTOM;
                 else if (ballCoordinates.All(p =>
                                             p.Y - Speed <= rec.Margin.Top))
                     whereBallWillCollide[0] = Collision.TOP;
                 else
                     whereBallWillCollide[0] = Collision.NONE;

                 if (ballCoordinates.All(p =>
                                         p.X - Speed <= rec.Margin.Left))
                     whereBallWillCollide[1] = Collision.LEFT;
                 else if (ballCoordinates.All(p =>
                                         p.X + Speed >= rec.Margin.Left + rec.Width))
                     whereBallWillCollide[1] = Collision.RIGHT;
                 else
                     whereBallWillCollide[1] = Collision.NONE;
                 if (deleteObject)
                 {
                     rec.Visibility = Visibility.Collapsed;
                     int index = objectBallMayCollideWith.FindIndex(o =>
                         {
                             return (rec.Equals(o));
                         });
                     objectBallMayCollideWith.RemoveAt(index);
                 }
                 
            }
            else
                whereBallWillCollide = null;

            return whereBallWillCollide;

            //return (HitsTopWall() ||
            //        HitsBottomWall() ||
            //        HitsLeftPaddle() ||
            //        HitsRightPaddle());
        }
        public void SwitchDirection(Collision[] objectCollided)
        {
            if(objectCollided != null)
            // If the angle is switching, it is assumed that the  collided
            if (currentDirection == Direction.CLOCKWISE)
            {
                switch (currentAngle)
                {
                    case Angle.BOTTOM_LEFT:
                        currentAngle = Angle.TOP_LEFT;
                        break;
                    case Angle.BOTTOM_RIGHT:
                        if (objectCollided[0] == Collision.TOP)
                        {
                            currentDirection = Direction.COUNTER_CLOCKWISE;
                            currentAngle = Angle.TOP_RIGHT;
                        }
                        else // ball hit right paddle
                            currentAngle = Angle.BOTTOM_LEFT;
                        break;
                    case Angle.TOP_LEFT:
                        if (objectCollided[0] == Collision.BOTTOM)
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
                        if (objectCollided[0] == Collision.TOP)
                        {
                            if (objectCollided[1] == Collision.RIGHT)
                                currentAngle = Angle.TOP_RIGHT;
                            else
                                currentAngle = Angle.TOP_LEFT;

                            currentDirection = Direction.CLOCKWISE;
                            
                        }
                        else // ball hit left paddle
                            currentAngle = Angle.BOTTOM_RIGHT;
                        break;
                    case Angle.BOTTOM_RIGHT:
                        if(objectCollided[1] == Collision.LEFT)
                        {
                            if (objectCollided[0] == Collision.TOP)
                                currentAngle = Angle.TOP_LEFT;
                            else
                                currentAngle = Angle.BOTTOM_LEFT;
                        }
                        else 
                            currentAngle = Angle.TOP_RIGHT;
                        break;
                    case Angle.TOP_LEFT:
                        // Most likely hit a brick
                        if(objectCollided[1] == Collision.RIGHT)
                        {
                            // Hits the corner of a brick
                            if (objectCollided[0] == Collision.BOTTOM)
                                currentAngle = Angle.BOTTOM_RIGHT;
                            else
                                currentAngle = Angle.TOP_RIGHT;

                            currentDirection = Direction.CLOCKWISE;
                        }
                        else // Hit the bottom 
                            currentAngle = Angle.BOTTOM_LEFT;
                        break;
                    case Angle.TOP_RIGHT:
                        if (objectCollided[0] == Collision.BOTTOM)
                        {
                            // Hit bottom corner of right paddle
                            if (objectCollided[1] == Collision.LEFT)
                                currentAngle = Angle.BOTTOM_LEFT;
                            else // Hit top wall
                                currentAngle = Angle.BOTTOM_RIGHT;
                            currentDirection = Direction.CLOCKWISE;
                            
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
