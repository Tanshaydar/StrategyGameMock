using AStar;
using DG.Tweening;
using EventAggregation;
using EventAggregation.Messages;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public const float MoveDurationPerCell = 0.3f;
    private Point CurrentPoint { get; set; }

    private void OnMouseDown()
    {
        EventAggregator.Instance.Publish(new SoldierSelectedMessage(this));
    }

    // Use this for initialization
    public void Init(Point spawnPoint)
    {
        CurrentPoint = spawnPoint;
        Debug.LogFormat("Spawn point: {0}", spawnPoint);
    }

    public void StartMoving(Point endPoint, Node[,] map)
    {
        Debug.LogFormat("Starting to moving from {0} to {1}", CurrentPoint, endPoint);

        var parameters = new SearchParameters(CurrentPoint, endPoint, map);
        var finder = new PathFinder(parameters);

        var points = finder.FindPath();
        Debug.LogFormat("Found Path: {0}", Point.PointListToString(points));

        for (var i = 0; i < points.Count; i++)
        {
            transform.DOMove(GridToWorldPos(points[i]), MoveDurationPerCell)
                .SetDelay(i * MoveDurationPerCell)
                .SetEase(Ease.Linear);
        }

        CurrentPoint = endPoint;
    }

    private Vector2 GridToWorldPos(Point point)
    {
        var obj = GameObject.Find(string.Format("{0},{1}", point.X, point.Y));
        if (obj == null)
        {
            Debug.LogErrorFormat("Unable to find gameobject with coordinate: {0}", point);
            return Vector2.zero;
        }

        return obj.transform.position;
    }
}