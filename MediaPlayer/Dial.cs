using System;
using System.Windows;

namespace MediaPlayer
{
    public class Dial
    {
        private double angle;
        public enum Quadrants : int { nw=2, ne=1, sw=4, se=3};

        public Dial()
        { 

        }

        public double GetAngle (Point touchPoint, Size circleSize)
        {
            var X = touchPoint.X - (circleSize.Width / 2d);
            var Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
            var Hypot = Math.Sqrt(X * X + Y * Y);
            var Value = Math.Asin(Y / Hypot) * 180 / Math.PI;
            var Quadrant = (X >= 0) ?
                (Y >= 0) ? Quadrants.ne : Quadrants.se :
                (Y >= 0) ? Quadrants.nw : Quadrants.sw;
            switch (Quadrant)
            {
                case Quadrants.ne: Value = 090 - Value;break;
                case Quadrants.nw: Value = 270 + Value;break;
                case Quadrants.se: Value = 090 - Value;break;
                case Quadrants.sw: Value = 270 + Value;break;
            }
            return Value;
        }

        public double Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                this.angle = value;
            }
        }

    }
}
