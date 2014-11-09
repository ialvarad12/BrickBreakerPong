using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace BrickBreakerPong
{
    public interface IPaddle
    {
        void MovePaddleUp();
        void MovePaddleDown();

        
        Point Position
        {
            get;
            set;
        }
        double Height
        {
            get;
            set;
        }
        double Width
        {
            get;
            set;
        }
    }
}
