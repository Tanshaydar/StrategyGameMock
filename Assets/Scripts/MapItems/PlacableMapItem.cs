using System;
using System.Collections.Generic;
using System.Linq;
using AStar;
using EventAggregation;
using EventAggregation.Messages;
using UnityEngine;

public abstract class PlaceableMapItem : MapItem
{
    public bool IsMoving { get; private set; }
    private Vector3 _snapPosition;
    private HashSet<Collider2D> availableCells;
    private HashSet<Collider2D> forbiddenCells;
    protected int minX = Int32.MaxValue, minY = Int32.MaxValue;

    protected virtual void Awake()
    {
    }

    public virtual void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
        IsMoving = true;
        availableCells = new HashSet<Collider2D>();
        forbiddenCells = new HashSet<Collider2D>();
    }

    private void LateUpdate()
    {
        if (IsMoving)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(pos.x, pos.y);
        }
    }

    public virtual void OnMouseDown()
    {
        if (IsMoving && forbiddenCells.Count == 0)
        {
            IsMoving = false;
            float xPos = 0, yPos = 0;

            var points = new List<Point>();
            foreach (var availableGrid in availableCells)
            {
                xPos += availableGrid.transform.position.x;
                yPos += availableGrid.transform.position.y;
                string[] splitter = availableGrid.gameObject.name.Split(',');
                int x = int.Parse(splitter[0]);
                int y = int.Parse(splitter[1]);
                if (x < minX) minX = x;
                if (y < minY) minY = y;
                points.Add(new Point(x, y));
            }
            EventAggregator.Instance.Publish(new AddWallMessage(points));

            xPos = xPos / availableCells.Count;
            yPos = yPos / availableCells.Count;

            minX = minX - 1;
            minY = minY - 1;

            availableCells.Clear();
            transform.position = new Vector3(xPos, yPos, 0);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().size;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsMoving)
        {
            return;
        }

        var otherItem = other.GetComponent<PlaceableMapItem>();
        var otherSprite = other.GetComponent<SpriteRenderer>();

        // TODO: this check can be "if cell component"
        if (otherItem == null)
        {
            // Entered to an empty cell
            string[] splitter = other.gameObject.name.Split(',');
            int x = int.Parse(splitter[0]);
            int y = int.Parse(splitter[1]);

            if (!GameManager.Map[x, y].IsWalkable)
            {
                forbiddenCells.Add(other);
            }
            else
            {
                availableCells.Add(other);
                otherSprite.color = Color.blue;
            }

            ColorizeCells(forbiddenCells.Count > 0 ? Color.red : Color.blue);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsMoving)
        {
            return;
        }

        var otherItem = other.GetComponent<PlaceableMapItem>();

        // TODO: this check can be "if cell component"
        if (otherItem == null)
        {
            // Exited from an empty cell
            string[] splitter = other.gameObject.name.Split(',');
            int x = int.Parse(splitter[0]);
            int y = int.Parse(splitter[1]);

            if (!GameManager.Map[x, y].IsWalkable)
            {
                forbiddenCells.Remove(other);
            }
            else
            {
                availableCells.Remove(other);
            }

            ColorizeCells(forbiddenCells.Count > 0 ? Color.red : Color.blue);
            other.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void ColorizeCells(Color color)
    {
        foreach (var forbiddenGrid in forbiddenCells)
        {
            forbiddenGrid.GetComponent<SpriteRenderer>().color = color;
        }
        foreach (var availableGrid in availableCells)
        {
            availableGrid.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
