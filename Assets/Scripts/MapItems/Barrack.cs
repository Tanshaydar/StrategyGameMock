using System.Collections.Generic;
using UnityEngine;

public class Barrack : PlaceableMapItem
{
    public int Identifier { get; set; }

    public List<Vector2> SoldierSpawnPositions { get; private set; }

    public override void Awake()
    {
        base.Awake();
        SoldierSpawnPositions = new List<Vector2>();
    }

    public override void OnMouseDown()
    {
        if (!IsMoving)
        {
            _gameManager.BarrackMenu.ShowBarrackMenu(Identifier);
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
                    SoldierSpawnPositions.Add(new Vector2(posX, posY));
                }
                for (int i = 0; i < 6; i++)
                {
                    int posX = minX + i;
                    int posY = minY;
                    SoldierSpawnPositions.Add(new Vector2(posX, posY));
                }
                for (int i = 0; i < 6; i++)
                {
                    int posX = minX + 5;
                    int posY = minY + i;
                    SoldierSpawnPositions.Add(new Vector2(posX, posY));
                }
                for (int i = 0; i < 6; i++)
                {
                    int posX = minX + 5;
                    int posY = minY + i;
                    SoldierSpawnPositions.Add(new Vector2(posX, posY));
                }
            }
        }        
    }

    public void UpdateSoldierSpawnPositions()
    {
        SoldierSpawnPositions.RemoveAll(spawnPosition => _gameManager.Grid[(int)spawnPosition.x, (int)spawnPosition.y].IsWall);
    }
}