using System.Collections.Generic;
using AStar;
using EventAggregation;
using EventAggregation.Messages;
using UnityEngine;

public class Barrack : PlaceableMapItem
{
    public int Identifier { get; set; }

    public List<Point> SoldierSpawnPositions { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SoldierSpawnPositions = new List<Point>();
    }

    public override void OnMouseDown()
    {
        if (!IsMoving)
        {
            EventAggregator.Instance.Publish(new ShowMenuMessage(MenuBuildingType.Barrack, Identifier));
        }
        else
        {
            base.OnMouseDown();
            if (!IsMoving)
            {
                for (int i = 0; i < 6; i++)
                {
                    int posX = minX;
                    int posY = minY + i;
                    SoldierSpawnPositions.Add(new Point(posX, posY));
                }
                for (int i = 0; i < 6; i++)
                {
                    int posX = minX + i;
                    int posY = minY;
                    SoldierSpawnPositions.Add(new Point(posX, posY));
                }
                for (int i = 0; i < 6; i++)
                {
                    int posX = minX + 5;
                    int posY = minY + i;
                    SoldierSpawnPositions.Add(new Point(posX, posY));
                }
                for (int i = 0; i < 6; i++)
                {
                    int posX = minX + 5;
                    int posY = minY + i;
                    SoldierSpawnPositions.Add(new Point(posX, posY));
                }
            }
        }
    }

    public void UpdateSoldierSpawnPositions()
    {
        SoldierSpawnPositions.RemoveAll(spawnPosition => !GameManager.Map[spawnPosition.X, spawnPosition.Y].IsWalkable);
    }
}