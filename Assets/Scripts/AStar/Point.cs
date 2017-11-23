using System.Collections.Generic;
using System.Text;

namespace AStar
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static Point Zero
        {
            get { return new Point(0, 0); }
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        private bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", X, Y);
        }

        public static string PointListToString(List<Point> points)
        {
            if (points == null || points.Count == 0)
            {
                return string.Empty;
            }

            var str = new StringBuilder();
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                str.AppendFormat("{0}{1}", point, i == points.Count - 1 ? string.Empty : " -> ");
            }
            return str.ToString();
        }
    }
}