using System.Collections.Generic;
using AStar;

namespace EventAggregation.Messages
{
    public class AddWallMessage
    {
        public List<Point> Points { get; private set; }

        public AddWallMessage(List<Point> points)
        {
            Points = points;
        }

        public AddWallMessage(Point point)
        {
            Points = new List<Point> { point };
        }
    }
}