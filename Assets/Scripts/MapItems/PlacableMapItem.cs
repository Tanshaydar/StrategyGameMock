using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlaceableMapItem : MapItem {

	protected GameManager _gameManager;

	public bool IsMoving { get; private set; }
	private Vector3 _snapPosition;
	private List<Collider2D> availableGrids;
	private List<PlaceableMapItem> forbiddenGrids;
    protected int minX = Int32.MaxValue, minY = Int32.MaxValue;

    public virtual void Awake()
	{
		_gameManager = FindObjectOfType<GameManager>();
	}

	public virtual void Start()
	{
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
		IsMoving = true;
		availableGrids = new List<Collider2D>();
		forbiddenGrids = new List<PlaceableMapItem>();
	}

	void FixedUpdate()
	{
		if (IsMoving)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			transform.position = new Vector2(pos.x, pos.y);
		}
	}

	public virtual void OnMouseDown()
	{
		if (IsMoving && forbiddenGrids.Capacity == 0)
		{
			IsMoving = false;
			float xPos = 0, yPos = 0;
			foreach (Collider2D grid in availableGrids)
			{
				xPos += grid.transform.position.x;
				yPos += grid.transform.position.y;
				string[] splitter = grid.gameObject.name.Split(',');
			    int x = int.Parse(splitter[0]);
			    int y = int.Parse(splitter[1]);
			    if (x < minX) minX = x;
			    if (y < minY) minY = y;
                _gameManager.AddWall(x, y);
			}
			xPos = xPos / availableGrids.Count;
			yPos = yPos / availableGrids.Count;

		    minX = minX - 1;
		    minY = minY - 1;

			availableGrids.Clear();
			transform.position = new Vector3(xPos, yPos, 0);
			GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().size;
			GetComponent<SpriteRenderer>().color = Color.white;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(!IsMoving) return;
		if (other.gameObject.GetComponent<PlaceableMapItem>() == null)
		{
			if (forbiddenGrids.Count == 0)
			{
				other.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
				availableGrids.Add(other);
				availableGrids = availableGrids.Distinct().ToList();
			}
			else
			{
				other.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
			}
		}
		else
		{
			Debug.Log("Entered forbidden grid");
			forbiddenGrids.Add(other.GetComponent<PlaceableMapItem>());
			forbiddenGrids = forbiddenGrids.Distinct().ToList();
			foreach (PlaceableMapItem forbiddenGrid in forbiddenGrids)
			{
				forbiddenGrid.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
			}

			foreach (Collider2D availableGrid in availableGrids)
			{
				availableGrid.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if(!IsMoving) return;
		if (other.gameObject.GetComponent<PlaceableMapItem>() == null)
		{
			availableGrids.Remove(other);
		}
		else
		{
			Debug.Log("Exited forbidden grid");
			forbiddenGrids.Remove(other.GetComponent<PlaceableMapItem>());
			if (forbiddenGrids.Count == 0)
			{
				foreach (Collider2D availableGrid in availableGrids)
				{
					availableGrid.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
				}
			}
		}
		other.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
	}
}
