using System.Collections.Generic;

namespace AStar
{
    public class PathFinder
    {
        private readonly Node[,] _map;
        private readonly Node _startNode;
        private readonly Node _endNode;

        private HashSet<Point> _testedPoints;

        /// <summary>
        /// Create a new instance of PathFinder
        /// </summary>
        /// <param name="searchParameters"></param>
        public PathFinder(SearchParameters searchParameters)
        {
            _map = searchParameters.Map;
            _startNode = _map[searchParameters.StartLocation.X, searchParameters.StartLocation.Y];
            _startNode.State = NodeState.Open;
            _endNode = _map[searchParameters.EndLocation.X, searchParameters.EndLocation.Y];

            _testedPoints = new HashSet<Point> {_startNode.Location};
        }

        /// <summary>
        /// Attempts to find a path from the start location to the end location based on the supplied SearchParameters
        /// </summary>
        /// <returns>A List of Points representing the path. If no path was found, the returned list is empty.</returns>
        public List<Point> FindPath()
        {
            // The start node is the first entry in the 'open' list
            var path = new List<Point>();
            var success = Search(_startNode);
            if (success)
            {
                // If a path was found, follow the parents from the end node to build a list of locations
                var node = _endNode;
                while (node.ParentNode != null)
                {
                    path.Add(node.Location);
                    node = node.ParentNode;
                }

                // Reverse the list so it's in the correct order when returned
                path.Reverse();
            }

            ClearTestedLocations();
            return path;
        }

        private void ClearTestedLocations()
        {
            foreach (var point in _testedPoints)
            {
                _map[point.X, point.Y].Clear();
            }
        }

        /// <summary>
        /// Attempts to find a path to the destination node using <paramref name="currentNode"/> as the starting location
        /// </summary>
        /// <param name="currentNode">The node from which to find a path</param>
        /// <returns>True if a path to the destination has been found, otherwise false</returns>
        private bool Search(Node currentNode)
        {
            // Set the current node to Closed since it cannot be traversed more than once
            currentNode.State = NodeState.Closed;
            _testedPoints.Add(currentNode.Location);
            var nextNodes = GetAdjacentWalkableNodes(currentNode);

            // Sort by F-value so that the shortest possible routes are considered first
            nextNodes.Sort((node1, node2) => node1.FValue(_endNode.Location).CompareTo(node2.FValue(_endNode.Location)));
            foreach (var nextNode in nextNodes)
            {
                // Check whether the end node has been reached
                if (nextNode.Location == _endNode.Location)
                {
                    return true;
                }
                // If not, check the next set of nodes
                if (Search(nextNode)) // Note: Recurses back into Search(Node)
                    return true;
            }

            // The method returns false if this path leads to be a dead end
            return false;
        }

        /// <summary>
        /// Returns any nodes that are adjacent to <paramref name="fromNode"/> and may be considered to form the next step in the path
        /// </summary>
        /// <param name="fromNode">The node from which to return the next possible nodes in the path</param>
        /// <returns>A list of next possible nodes in the path</returns>
        private List<Node> GetAdjacentWalkableNodes(Node fromNode)
        {
            var walkableNodes = new List<Node>();
            var nextLocations = GetAdjacentLocations(fromNode.Location);

            foreach (var location in nextLocations)
            {
                var x = location.X;
                var y = location.Y;

                // Stay within the grid's boundaries
                if (x < 0 || x >= _map.GetLength(0) || y < 0 || y >= _map.GetLength(1))
                    continue;

                var node = _map[x, y];
                // Ignore non-walkable nodes
                if (!node.IsWalkable)
                    continue;

                // Ignore already-closed nodes
                if (node.State == NodeState.Closed)
                    continue;

                // Already-open nodes are only added to the list if their G-value is lower going via this route.
                if (node.State == NodeState.Open)
                {
                    var traversalCost = Node.GetTraversalCost(node.Location, node.ParentNode.Location);
                    var gTemp = fromNode.G + traversalCost;
                    if (gTemp < node.G)
                    {
                        node.ParentNode = fromNode;
                        walkableNodes.Add(node);
                    }
                }
                else
                {
                    // If it's untested, set the parent and flag it as 'Open' for consideration
                    node.ParentNode = fromNode;
                    node.State = NodeState.Open;
                    _testedPoints.Add(node.Location);
                    walkableNodes.Add(node);
                }
            }

            return walkableNodes;
        }

        /// <summary>
        /// Returns the eight locations immediately adjacent (orthogonally and diagonally) to <paramref name="fromLocation"/>
        /// </summary>
        /// <param name="fromLocation">The location from which to return all adjacent points</param>
        /// <returns>The locations as an IEnumerable of Points</returns>
        private static IEnumerable<Point> GetAdjacentLocations(Point fromLocation)
        {
            return new[]
            {
                new Point(fromLocation.X-1, fromLocation.Y-1),
                new Point(fromLocation.X-1, fromLocation.Y  ),
                new Point(fromLocation.X-1, fromLocation.Y+1),
                new Point(fromLocation.X,   fromLocation.Y+1),
                new Point(fromLocation.X+1, fromLocation.Y+1),
                new Point(fromLocation.X+1, fromLocation.Y  ),
                new Point(fromLocation.X+1, fromLocation.Y-1),
                new Point(fromLocation.X,   fromLocation.Y-1)
            };
        }
    }
}
