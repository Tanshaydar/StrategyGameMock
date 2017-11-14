using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float _gridSize = 0.32f;
    private readonly int _gridPixelSize = 32;

    public RectTransform ProductionPanel;
    public PowerPlantMenu PowerPlantMenu;
    public BarrackMenu BarrackMenu;
    public RectTransform InformationPanel;
    public Transform Map;
    public GameObject GridBox;
    public GameObject Barrack;
    public GameObject PowerPlant;
    public int GridWidth { get; private set; }
    public int GridHeight { get; private set; }

    public float GridSize
    {
        get { return _gridSize; }
    }

    public PathNode[,] Grid { get; private set; }
    public GameObject enemy;
    
    public Sprite carUp;
    public Sprite carDown;
    public Sprite carFront;
    public Sprite carBack;    

    public static string distanceType;


    //This is what you need to show in the inspector.
    public static int distance = 2;

    void Start()
    {
        Vector3 p1 = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane));
        GridBox.transform.position = new Vector3(p1.x + 0.16f, p1.y - 0.16f, 0);

        GridWidth = (int) ((Camera.main.pixelWidth) / 20);
        GridHeight = (Camera.main.pixelHeight / 20);
        
        //Generate a grid - nodes according to the specified size
        Grid = new PathNode[GridWidth, GridHeight];

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                //Boolean isWall = ((y % 2) != 0) && (rnd.Next (0, 10) != 8);
                Boolean isWall = false;
                Grid[x, y] = new PathNode()
                {
                    IsWall = isWall,
                    X = x,
                    Y = y,
                };
            }
        }

        //instantiate grid gameobjects to display on the scene
        CreateGrid();

        //instantiate enemy object
//        createEnemy();
    }


    private void CreateGrid()
    {
        //Generate Gameobjects of GridBox to show on the Screen
        for (int i = 0; i < GridHeight; i++)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                GameObject newTileSet = Instantiate(GridBox);
                newTileSet.transform.position = new Vector2(GridBox.transform.position.x + (GridSize * j),
                    GridBox.transform.position.y - (GridSize * i));
                newTileSet.name = j + "," + i;

                newTileSet.gameObject.transform.parent = Map;
                newTileSet.SetActive(true);
            }
        }
    }

    public void CreateBarrack(int identifier)
    {
        DisableMenus();
        GameObject barrack = Instantiate(Barrack);
        barrack.GetComponent<Barrack>().Identifier = identifier;
    }

    public void CreatePowerPlant(int identifier)
    {
        DisableMenus();
        GameObject powerPlant = Instantiate(PowerPlant);
        powerPlant.GetComponent<PowerPlant>().Identifier = identifier;
    }

    public void DisableMenus()
    {
        BarrackMenu.gameObject.SetActive(false);
        PowerPlantMenu.gameObject.SetActive(false);
    }

    void CreateSoldier()
    {
        GameObject nb = (GameObject) Instantiate(enemy);
        nb.SetActive(true);
    }

    public void AddWall(int x, int y)
    {
        Grid[x, y].IsWall = true;
    }

    public void RemoveWall(int x, int y)
    {
        Grid[x, y].IsWall = false;
    }

    
}