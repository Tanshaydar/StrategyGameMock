using System;
using System.Collections;
using System.Collections.Generic;
using SettlersEngine;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

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


    public gridPosition currentGridPosition = new gridPosition();
    public gridPosition startGridPosition = new gridPosition();
    public gridPosition endGridPosition = new gridPosition();

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
    private enum Orientation
    {
        Horizontal,
        Vertical
    };

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
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
    void Start()
    {
        myColor = getRandomColor();

        startGridPosition = new gridPosition(0, Random.Range(0, _gameManager.GridHeight - 1));
        endGridPosition = new gridPosition(_gameManager.GridWidth - 1,
            Random.Range(0, _gameManager.GridHeight - 1));
        initializePosition();


        MySolver<PathNode, Object> aStar = new MySolver<PathNode, Object>(_gameManager.Grid);
        IEnumerable<PathNode> path = aStar.Search(new Vector2(startGridPosition.x, startGridPosition.y),
            new Vector2(endGridPosition.x, endGridPosition.y), null);


        foreach (GameObject g in GameObject.FindGameObjectsWithTag("GridBox"))
        {
            g.GetComponent<Renderer>().material.color = Color.white;
        }


        updatePath();

        this.GetComponent<Renderer>().material.color = myColor;
    }


    public void findUpdatedPath(int currentX, int currentY)
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
                if (g.GetComponent<SpriteRenderer>().material.color != Color.red &&
                    g.GetComponent<SpriteRenderer>().material.color == myColor)
                    g.GetComponent<SpriteRenderer>().material.color = Color.white;
            }


            foreach (PathNode node in path)
            {
                GameObject.Find(node.X + "," + node.Y).GetComponent<SpriteRenderer>().material.color = myColor;
            }
        }
    }


    Color getRandomColor()
    {
        Color tmpCol = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f),
            Random.Range(0f, 1f));
        return tmpCol;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            StartCoroutine(move());
        }
    }



        {
        }




    public IEnumerator move()
    {
        isMoving = true;
        startPosition = transform.position;
        t = 0;

        if (gridOrientation == Orientation.Horizontal)
        {
            endPosition = new Vector2(startPosition.x + Math.Sign(input.x) * _gameManager.GridSize,
                startPosition.y);
            currentGridPosition.x += Math.Sign(input.x);
        }
        else
        {
            endPosition = new Vector2(startPosition.x + Math.Sign(input.x) * _gameManager.GridSize,
                startPosition.y + Math.Sign(input.y) * _gameManager.GridSize);

            currentGridPosition.x += Math.Sign(input.x);
            currentGridPosition.y += Math.Sign(input.y);
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


        isMoving = false;
        getNextMovement();

        yield return 0;
    }

    void updatePath()
    {
        findUpdatedPath(currentGridPosition.x, currentGridPosition.y);
    }

    void getNextMovement()
    {
        updatePath();


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

        StartCoroutine(move());
    }

    public Vector2 getGridPosition(int x, int y)
    {
        float contingencyMargin = _gameManager.GridSize * .10f;
        float posX = _gameManager.GridBox.transform.position.x + (_gameManager.GridSize * x) - contingencyMargin;
        float posY = _gameManager.GridBox.transform.position.y + (_gameManager.GridSize * y) + contingencyMargin;
        return new Vector2(posX, posY);
    }


    public void initializePosition()
    {
        this.gameObject.transform.position = getGridPosition(startGridPosition.x, startGridPosition.y);
        currentGridPosition.x = startGridPosition.x;
        currentGridPosition.y = startGridPosition.y;
        isMoving = false;
        GameObject.Find(startGridPosition.x + "," + startGridPosition.y).GetComponent<SpriteRenderer>().material.color =
            Color.black;
    }
}