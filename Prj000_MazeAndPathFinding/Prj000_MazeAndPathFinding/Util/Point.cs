using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Util
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point()
        {
            X = 0;
            Y = 0;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public static Point operator+(Point x, Point y)
        {
            Point value = new Point();

            value.X = x.X + y.X;
            value.Y = x.Y + y.Y;

            return value;
        }

        public static Point operator -(Point x, Point y)
        {
            Point value = new Point();

            value.X = x.X - y.X;
            value.Y = x.Y - y.Y;

            return value;
        }

        public static Point operator /(Point x, int dividor)
        {
            Point value = new Point();

            value.X = x.X  / dividor;
            value.Y = x.Y / dividor;

            return value;
        }

        public Point Copy()
        {
            Point value = new Point(this);

            return value;
        }

        public override bool Equals(object obj)
        {
            Point objAsPoint = obj as Point;

            return X == objAsPoint.X && Y == objAsPoint.Y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
