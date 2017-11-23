using System.Collections.Generic;
using System.Linq;
using AStar;
using EventAggregation;
using EventAggregation.Messages;
using UnityEngine;

public class GameManager : MonoBehaviour, IHandle<SoldierSelectedMessage>, IHandle<AddWallMessage>, IHandle<HideMenuExceptMessage>
{
    private float _gridSize = 0.32f;

    public PowerPlantMenu PowerPlantMenu;
    public BarrackMenu BarrackMenu;
    public Transform MapParent;
    public GameObject GridBox;
    public GameObject Barrack;
    public GameObject PowerPlant;

    private int GridWidth { get; set; }
    private int GridHeight { get; set; }

    public static Node[,] Map { get; private set; }
    public GameObject SoldierPrefab;

    private Soldier _selectedSoldier;
    private List<Barrack> _barracks;

    private void Awake()
    {
        EventAggregator.Instance.Subscribe(this);

        _barracks = new List<Barrack>();
    }

    private void Start()
    {
        var p1 = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane));
        GridBox.transform.position = new Vector3(p1.x + 0.16f, p1.y - 0.16f, 0);

        GridWidth = (Camera.main.pixelWidth) / 20;
        GridHeight = (Camera.main.pixelHeight / 20);

        //Generate a grid - nodes according to the specified size
        Map = new Node[GridWidth, GridHeight];

        for (var x = 0; x < GridWidth; x++)
        {
            for (var y = 0; y < GridHeight; y++)
            {
                //Boolean isWall = ((y % 2) != 0) && (rnd.Next (0, 10) != 8);
                Map[x, y] = new Node(x, y, true);
            }
        }

        //instantiate grid gameobjects to display on the scene
        CreateGrid();
    }

    public void Handle(SoldierSelectedMessage message)
    {
        if (message.Soldier != null)
        {
            _selectedSoldier = message.Soldier;
        }
    }

    public void Handle(AddWallMessage message)
    {
        if (message.Points != null)
        {
            foreach (var point in message.Points)
            {
                AddWall(point);
            }
        }
    }

    public void Handle(HideMenuExceptMessage message)
    {
        HideMenu(message.ExceptType);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (_selectedSoldier != null)
            {
                var hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
                if (hit.transform != null && hit.transform.name.Contains(","))
                {
                    var splitter = hit.transform.name.Split(',');
                    var x = int.Parse(splitter[0]);
                    var y = int.Parse(splitter[1]);
                    _selectedSoldier.StartMoving(new Point(x, y), Map);
                }
            }
        }
    }


    private void CreateGrid()
    {
        //Generate Gameobjects of GridBox to show on the Screen
        for (var i = 0; i < GridHeight; i++)
        {
            for (var j = 0; j < GridWidth; j++)
            {
                var newTileSet = Instantiate(GridBox);
                newTileSet.transform.position = new Vector2(GridBox.transform.position.x + (_gridSize * j),
                    GridBox.transform.position.y - (_gridSize * i));
                newTileSet.name = j + "," + i;

                newTileSet.gameObject.transform.parent = MapParent;
                newTileSet.SetActive(true);
            }
        }
    }

    public void CreateBarrack(int identifier)
    {
        HideMenu();
        var barrack = Instantiate(Barrack);
        var b = barrack.GetComponent<Barrack>();
        b.Identifier = identifier;
        _barracks.Add(b);
    }

    public void CreatePowerPlant(int identifier)
    {
        HideMenu();
        var powerPlant = Instantiate(PowerPlant);
        powerPlant.GetComponent<PowerPlant>().Identifier = identifier;
    }

    public void HideMenu(MenuBuildingType exceptType = MenuBuildingType.None)
    {
        if (exceptType != MenuBuildingType.Barrack)
        {
            BarrackMenu.gameObject.SetActive(false);
        }
        if (exceptType != MenuBuildingType.PowerPlant)
        {
            PowerPlantMenu.gameObject.SetActive(false);
        }
    }

    public void ShowBarrackMenu()
    {
        BarrackMenu.gameObject.SetActive(true);
        PowerPlantMenu.gameObject.SetActive(false);
    }

    public void ShowPowerPlantMenu()
    {
        BarrackMenu.gameObject.SetActive(false);
        PowerPlantMenu.gameObject.SetActive(true);
    }

    public void CreateSoldier(int identifier)
    {
        var barrack = _barracks.FirstOrDefault(b => b.Identifier == identifier);
        if (barrack == null)
        {
            Debug.LogErrorFormat("Unable to find selected barrack with id: {0}!", identifier);
            return;
        }

        barrack.UpdateSoldierSpawnPositions();
        if (barrack.SoldierSpawnPositions.Count == 0)
        {
            Debug.LogWarning("There is not enough place for soldier");
            return;
        }

        var spawnPoint = barrack.SoldierSpawnPositions[0];
        PlaceSoldier(spawnPoint);
        barrack.SoldierSpawnPositions.Remove(spawnPoint);
    }

    private void PlaceSoldier(Point spawnPoint)
    {
        var gridbox = GameObject.Find(spawnPoint.X + "," + spawnPoint.Y);
        transform.position = gridbox.transform.position;

        var soldierGo = Instantiate(SoldierPrefab, gridbox.transform.position, gridbox.transform.rotation);
        var soldier = soldierGo.GetComponent<Soldier>();
        soldier.Init(spawnPoint);
        soldierGo.SetActive(true);
    }

    private void AddWall(Point point)
    {
        Map[point.X, point.Y].IsWalkable = false;
    }

    public void RemoveWall(int x, int y)
    {
        Map[x, y].IsWalkable = true;
    }
}