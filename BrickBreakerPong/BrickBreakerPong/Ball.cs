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
        public  double Speed; // In constructor
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
                for (int i = 0; i < 360; i = i + 36)
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
        int skipTick = 100;
        Rectangle lastCollapsed;
        public Ball(double ballWidth = 50.0, double ballHeight = 50.0, double ballSpeed = 10.0)
        {
            this.Width = ballWidth;
            this.Height = ballHeight;
            this.Position = new Point(0, 0);
            this.Speed = ballSpeed;
            bounce = new TimeSpan(DateTime.Now.Ticks);
            currentAngle = Angle.BOTTOM_RIGHT;
            currentDirection = Direction.COUNTER_CLOCKWISE;
            lastCollapsed = null;
        }
        //private bool checkBottomBreakCollapse(List<Rectangle> paddles)
        //{
        //    foreach(Rectangle paddle in paddles)
        //    {
        //        double _RTBrick = paddle.Margin.Top;
        //        double _RLBrick = paddle.Margin.Left;
        //        double _RRBrick = paddle.Margin.Left + paddle.Width;
        //        double _ballBottom = this.Position.Y + this.Width;
        //        double _ballLeft = this.Position.X + (this.Width / 2);

        //        if ((_ballBottom >= _RTBrick && _ballLeft > _RLBrick && _ballLeft < _RRBrick))
        //        {
        //            if (currentAngle == Angle.BOTTOM_RIGHT)
        //            {
        //                currentAngle = Angle.TOP_RIGHT;
        //            }
        //            else if (currentAngle == Angle.BOTTOM_LEFT)
        //            {
        //                currentAngle = Angle.TOP_LEFT;
        //            }

        //            return true;
        //        }
        //    }
        //    return false;
        //}
        public void SetAngle(List<Rectangle> paddles)
        {
            double leftOfBall = this.Position.X;
            double topOfBall = this.Position.Y;

            //if (!checkBottomBreakCollapse(paddles))
            //{
                if (leftOfBall >= Game.boardWidth - this.Width)
                {
                    if (currentAngle == Angle.BOTTOM_RIGHT)
                    {
                        currentDirection = Direction.CLOCKWISE;
                        currentAngle =  Angle.BOTTOM_LEFT;
                    }
                    else
                    {
                        currentDirection = Direction.COUNTER_CLOCKWISE;
                        currentAngle =  Angle.TOP_LEFT;
                    }
                }
                else if (leftOfBall <= 0)
                {
                    if (currentAngle == Angle.TOP_LEFT)
                    {
                        currentDirection = Direction.CLOCKWISE;
                        currentAngle =  Angle.TOP_RIGHT;
                    }
                    else
                    {
                        currentDirection = Direction.COUNTER_CLOCKWISE;
                        currentAngle = Angle.BOTTOM_RIGHT;
                    }
                }
                else if (topOfBall <= 0)
                {
                    if (currentAngle == Angle.TOP_RIGHT)
                    {
                        currentDirection = Direction.CLOCKWISE;
                        currentAngle = Angle.BOTTOM_RIGHT;
                    }
                    else
                    {
                        currentDirection = Direction.COUNTER_CLOCKWISE;
                        currentAngle =  Angle.BOTTOM_LEFT;
                    }
                }
                else if (topOfBall >= Game.boardHeight - this.Width)
                {
                    if (currentAngle == Angle.BOTTOM_LEFT)
                    {
                        currentDirection = Direction.CLOCKWISE;
                        currentAngle =  Angle.TOP_LEFT;
                    }
                    else
                    {
                        currentDirection = Direction.COUNTER_CLOCKWISE;
                        currentAngle =  Angle.TOP_RIGHT;
                    }
                }
            //}
        }
        public bool CollidesWith(List<Rectangle> objectBallMayCollideWith, bool deleteObject)
        {
            //List<Point> ballCoordinates = Boundaries;
            //var collidedObject = objectBallMayCollideWith.Where(s => ballCoordinates.Any(p =>
            //                                                        p.X >= s.Margin.Left &&
            //                                                        p.X <= s.Margin.Left + s.Width &&
            //                                                        p.Y <= s.Margin.Top + s.Height &&
            //                                                        p.Y >= s.Margin.Top));
            List<Point> ballCoordinates = Boundaries;
            var collidedObject = objectBallMayCollideWith.Where(s => ballCoordinates.Any(p =>
                                                                    p.X >= s.Margin.Left - Speed &&
                                                                    p.X <= s.Margin.Left + s.Width + Speed &&
                                                                    p.Y <= s.Margin.Top + s.Height + Speed &&
                                                                    p.Y >= s.Margin.Top - Speed));
            //foreach(Rectangle rec in collidedObject)
            if(collidedObject.Count() > 0)
            {
                 GamePage.sfx.Play();
                Rectangle rec = collidedObject.FirstOrDefault();

                if (lastCollapsed == rec && skipTick > 0 && lastCollapsed.Width != Game.boardWidth)
                {
                    skipTick--;
                    return false;
                }
                else
                {
                    skipTick = 500;
                    lastCollapsed = rec;
                }

                var nearCoordinates = ballCoordinates.Where(p =>
                                        p.X >= rec.Margin.Left - Speed &&
                                        p.X <= rec.Margin.Left + rec.Width + Speed &&
                                        p.Y <= rec.Margin.Top + rec.Height + Speed &&
                                        p.Y >= rec.Margin.Top - Speed);
                //var nearCoordinates = ballCoordinates.Where(p =>
                //                        p.X >= rec.Margin.Left &&
                //                        p.X <= rec.Margin.Left + rec.Width &&
                //                        p.Y <= rec.Margin.Top + rec.Height &&
                //                        p.Y >= rec.Margin.Top);

                var xmin = nearCoordinates.Min(s => s.X);
                var ymin = nearCoordinates.Min(s => s.Y);

                var nearCoordinate = xmin < ymin ? nearCoordinates.OrderByDescending(s => s.X).First() : nearCoordinates.OrderByDescending(s => s.Y).First();



                double topOfBall = this.Position.Y;
                double bottomOfBall = this.Position.Y + Height;
                double leftOfBall = this.Position.X;
                double rightOfBall = this.Position.X + Width;
                double centerOfBallY = this.Origin.Y;
                double centerOfBallX = this.Origin.X;

                if (rec.Margin.Top <= bottomOfBall &&        // top
                     rec.Margin.Top + rec.Height > bottomOfBall &&
                     rec.Margin.Left <= centerOfBallY &&
                     rec.Margin.Left + rec.Width >= centerOfBallY)
                {
                    currentDirection = currentAngle == Angle.BOTTOM_RIGHT ? Direction.COUNTER_CLOCKWISE : Direction.CLOCKWISE;

                }
                else if (rec.Margin.Top + rec.Height >= centerOfBallX &&             // left
                     rec.Margin.Top < centerOfBallX &&
                     rec.Margin.Left <= rightOfBall &&
                     rec.Margin.Left + rec.Width > rightOfBall)
                {
                    currentDirection = currentAngle == Angle.TOP_RIGHT ? Direction.COUNTER_CLOCKWISE : Direction.CLOCKWISE;
                }
                else if (rec.Margin.Top + rec.Height >= topOfBall &&                 // bottom
                                                        rec.Margin.Top < topOfBall &&
                                                        rec.Margin.Left <= centerOfBallY &&
                                                        rec.Margin.Left + rec.Width >= centerOfBallY)
                {
                    currentDirection = currentAngle == Angle.TOP_RIGHT ? Direction.CLOCKWISE : Direction.COUNTER_CLOCKWISE;
                    
                }
                else if (rec.Margin.Top + rec.Height >= centerOfBallX &&             // right
                                                        rec.Margin.Top < centerOfBallX &&
                                                        rec.Margin.Left < leftOfBall &&
                                                        rec.Margin.Left + rec.Width >= leftOfBall)
                {
                    currentDirection = currentAngle == Angle.TOP_LEFT ? Direction.CLOCKWISE : Direction.COUNTER_CLOCKWISE;
                }


                SwitchDirection(nearCoordinate, rec);
                // Delete a brick when collides
                if (deleteObject)
                {
                    rec.Visibility = Visibility.Collapsed;

                    int index = objectBallMayCollideWith.IndexOf(rec);
                    objectBallMayCollideWith.RemoveAt(index);

                    // Remove brick from the levelArray
                    GamePage.level.RemoveCellAtIndex(index);
                }
                return true;
            }

            return false;
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
                //GamePage.sfx.Play();
                GamePage.sfx.Play();

                Rectangle rec = collidedObject.FirstOrDefault();
                if (ballCoordinates.All(p => p.Y + Speed >= rec.Margin.Top + rec.Height))
                    whereBallWillCollide[0] = Collision.BOTTOM;
                else if (ballCoordinates.All(p => p.Y - Speed <= rec.Margin.Top))
                    whereBallWillCollide[0] = Collision.TOP;
                else
                    whereBallWillCollide[0] = Collision.NONE;

                if (ballCoordinates.All(p => p.X - Speed <= rec.Margin.Left))
                    whereBallWillCollide[1] = Collision.LEFT;
                else if (ballCoordinates.All(p => p.X + Speed >= rec.Margin.Left + rec.Width))
                    whereBallWillCollide[1] = Collision.RIGHT;
                else
                    whereBallWillCollide[1] = Collision.NONE;

                // Delete a brick when collides
                if (deleteObject)
                {
                    rec.Visibility = Visibility.Collapsed;

                    int index = objectBallMayCollideWith.IndexOf(rec);
                    objectBallMayCollideWith.RemoveAt(index);

                    // Remove brick from the levelArray
                    GamePage.level.RemoveCellAtIndex(index);
                }
                 
            }
            else
                whereBallWillCollide = null;

            return whereBallWillCollide;

        }
        
        public void SwitchDirection(Point nearCoordinate, Rectangle rec)
        {

            Collision hitAt;
            int left = (int)(nearCoordinate.X - rec.Margin.Left);
            int right = (int)(nearCoordinate.X - (rec.Margin.Left + rec.Width));
            int top = (int)(nearCoordinate.Y - rec.Margin.Top);
            int bottom = (int)(nearCoordinate.Y - (rec.Margin.Top + rec.Height));

            //int[] values = { left, right, top, bottom };
            int[] values = { Math.Abs(left), Math.Abs(right), Math.Abs(top), Math.Abs(bottom) };
            Array.Sort(values);

            if (values[0] == Math.Abs(left))
                hitAt = Collision.LEFT;
            else if (values[0] == Math.Abs(right))
                hitAt = Collision.RIGHT;
            else if (values[0] == Math.Abs(top))
                hitAt = Collision.TOP;
            else
                hitAt = Collision.BOTTOM;

            switch (currentAngle)
            {
                case Angle.BOTTOM_RIGHT:

                    if (hitAt == Collision.LEFT)
                        currentAngle = Angle.BOTTOM_LEFT;
                    else if (hitAt == Collision.TOP)
                        currentAngle = Angle.TOP_RIGHT;
                    else if (top < left)
                        currentAngle = Angle.TOP_RIGHT;
                    else
                        currentAngle = Angle.BOTTOM_LEFT;

                    break;

                case Angle.BOTTOM_LEFT:

                    if (hitAt == Collision.RIGHT)
                        currentAngle = Angle.BOTTOM_RIGHT;
                    else if (hitAt == Collision.TOP)
                        currentAngle = Angle.TOP_LEFT;
                    else if (top < right)
                        currentAngle = Angle.TOP_LEFT;
                    else
                        currentAngle = Angle.BOTTOM_RIGHT;
                    break;

                case Angle.TOP_LEFT:

                    if (hitAt == Collision.BOTTOM)
                        currentAngle = Angle.BOTTOM_LEFT;
                    else if (hitAt == Collision.RIGHT)
                        currentAngle = Angle.TOP_RIGHT;
                    else if (bottom < right)
                        currentAngle = Angle.BOTTOM_LEFT;
                    else
                        currentAngle = Angle.TOP_RIGHT;
                    break;

                case Angle.TOP_RIGHT:

                    if (hitAt == Collision.BOTTOM)
                        currentAngle = Angle.BOTTOM_RIGHT;
                    else if (hitAt == Collision.LEFT)
                        currentAngle = Angle.TOP_LEFT;
                    else if (bottom < right)
                        currentAngle = Angle.BOTTOM_RIGHT;
                    else
                        currentAngle = Angle.TOP_LEFT;
                    break;
            }

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
        }
    }
}
