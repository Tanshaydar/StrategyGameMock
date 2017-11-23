using System;

namespace AStar
{
    /// <summary>
    /// Represents a single node on a grid that is being searched for a path between two points
    /// </summary>
    public class Node
    {
        private Node _parentNode;

        /// <summary>
        /// The node's location in the grid
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// True when the node may be traversed, otherwise false
        /// </summary>
        public bool IsWalkable { get; set; }

        /// <summary>
        /// Cost from start to here
        /// </summary>
        public float G { get; private set; }

        /// <summary>
        /// Estimated cost from here to end
        /// </summary>
        private float H { get; set; }

        /// <summary>
        /// Flags whether the node is open, closed or untested by the PathFinder
        /// </summary>
        public NodeState State { get; set; }

        /// <summary>
        /// Estimated total cost (F = G + H)
        /// </summary>
        public float FValue(Point endLocation)
        {
            if (H < 0)
            {
                H = GetTraversalCost(Location, endLocation);
            }
            return G + H;
        }

        /// <summary>
        /// Gets or sets the parent node. The start node's parent is always null.
        /// </summary>
        public Node ParentNode
        {
            get { return _parentNode; }
            set
            {
                // When setting the parent, also calculate the traversal cost from the start node to here (the 'G' value)
                _parentNode = value;
                if (_parentNode != null)
                {
                    G = _parentNode.G + GetTraversalCost(Location, _parentNode.Location);
                }
            }
        }

        /// <summary>
        /// Creates a new instance of Node.
        /// </summary>
        /// <param name="x">The node's location along the X axis</param>
        /// <param name="y">The node's location along the Y axis</param>
        /// <param name="isWalkable">True if the node can be traversed, false if the node is a wall</param>
        public Node(int x, int y, bool isWalkable)
        {
            Location = new Point(x, y);
            State = NodeState.Untested;
            IsWalkable = isWalkable;

            H = -1;
            G = 0;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}: {2}", Location.X, Location.Y, State);
        }

        /// <summary>
        /// Gets the distance between two points
        /// </summary>
        public static float GetTraversalCost(Point location, Point otherLocation)
        {
            float deltaX = otherLocation.X - location.X;
            float deltaY = otherLocation.Y - location.Y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        public void Clear()
        {
            G = 0;
            H = -1;
            State = NodeState.Untested;
            ParentNode = null;
        }
    }
}
