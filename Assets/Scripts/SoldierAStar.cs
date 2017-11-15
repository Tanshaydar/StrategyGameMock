using System;
using System.Collections;
using System.Collections.Generic;
using SettlersEngine;
using UnityEngine;
using Object = System.Object;

public class SoldierAStar : MonoBehaviour
{
    public Sprite SoldierUp;
    public Sprite SoldierDown;
    public Sprite SoldierRight;
    public Sprite SoldierLeft;

    private GameManager _gameManager;
    public PathNode nextNode;
    bool gray = false;
    public PathNode[,] grid;


    public GridPosition currentGridPosition = new GridPosition(0,0);
    public Vector2 startGridPosition;
    public GridPosition endGridPosition = new GridPosition(0,0);

    private Orientation gridOrientation = Orientation.Vertical;
    private bool allowDiagonals = false;
    private bool correctDiagonalSpeed = true;
    private Vector2 input;
    private bool isMoving = true;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float factor;
    private Color myColor;
    public float moveSpeed;

    private enum Orientation
    {
        Horizontal,
        Vertical
    };

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!isMoving)
        {
            StartCoroutine(Move());
        }
    }

    void OnMouseDown()
    {
        _gameManager.SelectedSoldier = gameObject;
    }

    public class MySolver<TPathNode, TUserContext> : SpatialAStar<TPathNode,
        TUserContext> where TPathNode : IPathNode<TUserContext>
    {
        protected override Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            int formula = GameManager.distance;
            int dx = Math.Abs(inStart.X - inEnd.X);
            int dy = Math.Abs(inStart.Y - inEnd.Y);

            switch (formula)
            {
                case 0:
                    return Math.Sqrt(dx * dx + dy * dy); //Euclidean distance
                case 1:
                    return (dx * dx + dy * dy); //Euclidean distance squared
                case 2:
                    return Math.Min(dx, dy); //Diagonal distance
                case 3:
                    return (dx * dy) + (dx + dy); //Manhatten distance
                default:
                    return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
            }
        }

        protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
        }

        public MySolver(TPathNode[,] inGrid) : base(inGrid)
        {
        }
    }


    // Use this for initialization
    public void InitializeSoldier(int x, int y)
    {
        if (_gameManager == null)
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        myColor = Color.yellow;
        startGridPosition = new Vector2(x, y);
        Debug.Log("Start Grid Position1 : " + startGridPosition.x + "-" + startGridPosition.y);
        InitializePosition();
    }

    public void StartMoving(int x, int y)
    {
        Debug.Log("Start moving to: " + x + "-" + y);
        endGridPosition = new GridPosition(x, y);

        isMoving = false;
        Debug.Log("Start Grid Position3 : " + startGridPosition.x + "-" + startGridPosition.y);
        GameObject.Find(startGridPosition.x + "," + startGridPosition.y).GetComponent<SpriteRenderer>().material.color =
            Color.black;

        MySolver<PathNode, Object> aStar = new MySolver<PathNode, Object>(_gameManager.Grid);
        IEnumerable<PathNode> path = aStar.Search(new Vector2(startGridPosition.x, startGridPosition.y),
            new Vector2(endGridPosition.x, endGridPosition.y), null);


        foreach (GameObject g in GameObject.FindGameObjectsWithTag("GridBox"))
        {
            g.GetComponent<SpriteRenderer>().color = Color.white;
        }


        UpdatePath();

        GetComponent<SpriteRenderer>().color = Color.yellow;
    }


    public void FindUpdatedPath(int currentX, int currentY)
    {
        MySolver<PathNode, Object> aStar = new MySolver<PathNode, Object>(_gameManager.Grid);
        IEnumerable<PathNode> path = aStar.Search(new Vector2(currentX, currentY),
            new Vector2(endGridPosition.x, endGridPosition.y), null);


        int x = 0;

        if (path != null)
        {
            foreach (PathNode node in path)
            {
                if (x == 1)
                {
                    nextNode = node;
                    break;
                }

                x++;
            }


            foreach (GameObject g in GameObject.FindGameObjectsWithTag("GridBox"))
            {
                if (g.GetComponent<SpriteRenderer>().color != Color.red &&
                    g.GetComponent<SpriteRenderer>().color == myColor)
                    g.GetComponent<SpriteRenderer>().material.color = Color.white;
            }


            foreach (PathNode node in path)
            {
                GameObject.Find(node.X + "," + node.Y).GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
    }

    public IEnumerator Move()
    {
        isMoving = true;
        startPosition = transform.position;
        Debug.Log("Start pos: " + startPosition);
        t = 0;

        if (gridOrientation == Orientation.Horizontal)
        {
            Debug.Log("horizontal");
            endPosition = new Vector2(startPosition.x + Math.Sign(input.x) * _gameManager.GridSize,
                startPosition.y);
            Debug.Log("End pos: " + endPosition);
            Debug.Log(input.x);
            Debug.Log(input.y);
            currentGridPosition.x += Math.Sign(input.x);
        }
        else
        {
            Debug.Log("vertical");
            endPosition = new Vector2(startPosition.x + Math.Sign(input.x) * _gameManager.GridSize,
                startPosition.y + Math.Sign(input.y) * _gameManager.GridSize);
            Debug.Log("End pos: " + endPosition);
            Debug.Log(input.x);
            Debug.Log(input.y);
            currentGridPosition.x += Math.Sign(input.x);
            currentGridPosition.y += Math.Sign(input.y);
            Debug.Log(currentGridPosition.x  + " - " + currentGridPosition.y);
        }

        if (allowDiagonals && correctDiagonalSpeed && input.x != 0 && input.y != 0)
        {
            factor = 0.9071f;
        }
        else
        {
            factor = 1f;
        }


        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed / _gameManager.GridSize) * factor;
            transform.position = Vector2.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        Debug.Log("---------------");
        isMoving = false;
        GetNextMovement();
        yield return 0;
    }

    void UpdatePath()
    {
        FindUpdatedPath(currentGridPosition.x, currentGridPosition.y);
    }

    void GetNextMovement()
    {
        UpdatePath();

        input.x = 0;
        input.y = 0;
        if (nextNode.X > currentGridPosition.x)
        {
            input.x = 1;
            this.GetComponent<SpriteRenderer>().sprite = SoldierRight;
        }
        if (nextNode.Y > currentGridPosition.y)
        {
            input.y = 1;
            this.GetComponent<SpriteRenderer>().sprite = SoldierUp;
        }
        if (nextNode.Y < currentGridPosition.y)
        {
            input.y = -1;
            this.GetComponent<SpriteRenderer>().sprite = SoldierDown;
        }
        if (nextNode.X < currentGridPosition.x)
        {
            input.x = -1;
            this.GetComponent<SpriteRenderer>().sprite = SoldierLeft;
        }

        StartCoroutine(Move());
    }
    
    private void InitializePosition()
    {
        GameObject gridbox = GameObject.Find(startGridPosition.x + "," + startGridPosition.y);
        transform.position = gridbox.transform.position;
        currentGridPosition.x = (int) startGridPosition.x;
        currentGridPosition.y = (int) startGridPosition.y;
        Debug.Log("Start Grid Position2 : " + startGridPosition.x + "-" + startGridPosition.y);
    }
}