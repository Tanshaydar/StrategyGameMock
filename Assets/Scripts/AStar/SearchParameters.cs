namespace AStar
{
    /// <summary>
    /// Defines the parameters which will be used to find a path across a section of the map
    /// </summary>
    public class SearchParameters
    {
        public Point StartLocation { get; private set; }

        public Point EndLocation { get; private set; }
        
        public Node[,] Map { get; private set; }

        public SearchParameters(Point startLocation, Point endLocation, Node[,] map)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
            Map = map;
        }
    }
}
