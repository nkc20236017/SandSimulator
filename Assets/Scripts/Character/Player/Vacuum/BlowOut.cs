using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class BlowOut : MonoBehaviour
{
	[Header("Tile Config")]
	[SerializeField] private Tilemap tilemap;
	[SerializeField] private TileBase tile;
	
	[Header("BlowOut Config")]
	[SerializeField] private Transform pivot;
	[SerializeField, Min(0f)] private float radius;
	[SerializeField, Min(0f)] private float distance;
	[SerializeField, Min(0f)] private float range;
	
	[Header("Instantiation Config")]
	[SerializeField] private float interval;
	[SerializeField, MinMaxSlider(0, 10)] private Vector2Int generateTileCount;
	
	[Header("Debug Config")]
	[SerializeField] private bool debug;
	
	private float _lastUpdateTime;
	private Camera _camera;
	private PlayerActions _playerActions;
	private PlayerActions.VacuumActions VacuumActions => _playerActions.Vacuum;

	private void Awake()
	{
		_playerActions = new PlayerActions();
	}

	private void Start()
	{
		_camera = Camera.main;
		_lastUpdateTime = Time.time;
		
		VacuumActions.SpittingOut.canceled += _ => _lastUpdateTime = Time.time;
	}

	private void Update()
	{
		if (VacuumActions.SpittingOut.IsPressed())
		{
			SpittingOutTiles();
		}
	}

	private void SpittingOutTiles()
	{
		if (Time.time - _lastUpdateTime > interval)
		{
			_lastUpdateTime = Time.time;
			var randomGenerateTileCount = Random.Range(generateTileCount.x, generateTileCount.y);
			for (var i = 0; i < randomGenerateTileCount; i++)
			{
				GenerateTile();
			}
		}

		UpdateTile();
	}
	
	private void GenerateTile()
	{
		var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
		var centerCell = (Vector3)tilemap.WorldToCell(mouseWorldPosition);

		var direction = centerCell - pivot.position;
		var angle = Mathf.Atan2(direction.y, direction.x);
		
		var chordLength = Mathf.Sqrt(Mathf.Pow(distance, 2) + Mathf.Pow(range, 2));
		var angle2 = Mathf.Acos(distance / chordLength);
		var newCell1 = GetNewCell(angle - angle2, chordLength);
		var newCell2 = GetNewCell(angle + angle2, chordLength);
		
		var random = Random.Range(generateTileCount.x, generateTileCount.y);
		for (var i = 0; i < random; i++)
		{
			var randomX = Random.Range(newCell1.x, newCell2.x);
			var randomY = Random.Range(newCell1.y, newCell2.y);
			var randomPosition = new Vector3(randomX, randomY, 0);
			var randomCell = tilemap.WorldToCell(randomPosition);
			tilemap.SetTile(randomCell, tile);
			// tilemap.SetColliderType(randomCell, Tile.ColliderType.None);
		}
	}
	
	private void UpdateTile()
	{
		var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
		var centerCell = (Vector3)tilemap.WorldToCell(mouseWorldPosition);

		var direction = centerCell - pivot.position;
		var angle = Mathf.Atan2(direction.y, direction.x);
		
		var chordLength = Mathf.Sqrt(Mathf.Pow(distance, 2) + Mathf.Pow(range, 2));
		var angle2 = Mathf.Acos(distance / chordLength);
		var a = GetNewCell(angle - angle2, chordLength);
		var b = GetNewCell(angle + angle2, chordLength);
		
		var chordLength2 = Mathf.Sqrt(Mathf.Pow(0, 2) + Mathf.Pow(range, 2));
		var angle3 = Mathf.Acos(0 / chordLength2);
		var h = radius * (1 - Mathf.Cos(angle3));
		var y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(range, 2)) - (radius - h) - distance;
		
		var newX3 = y * Mathf.Cos(angle);
		var newY3 = y * Mathf.Sin(angle);
		var newDirection3 = new Vector3(newX3, newY3, 0);
		
		var c = a + newDirection3;
		var d = b + newDirection3;
		
		var bounds = new BoundsInt((int)Mathf.Min(a.x, b.x, c.x, d.x), (int)Mathf.Min(a.y, b.y, c.y, d.y), 0, (int)Mathf.Abs(a.x - b.x), (int)Mathf.Abs(a.y - b.y), 1);
		foreach (var position in bounds.allPositionsWithin)
		{
			Debug.Log("1");
			var tileData = tilemap.GetTile(position);
			if (tileData == null) { continue; }

			Debug.Log("2");
			var newPosition = Vector3Int.RoundToInt((pivot.position - position).normalized);
			var newTilePosition = position + newPosition;
			if (tilemap.HasTile(newTilePosition)) { continue; }

			Debug.Log("3");
			tilemap.SetTile(newTilePosition, tileData);
			tilemap.SetTile(position, null);
			Debug.DrawLine(tilemap.GetCellCenterWorld(position), tilemap.GetCellCenterWorld(newTilePosition), Color.red, 1f);
			// tilemap.SetColliderType(newTilePosition, Tile.ColliderType.None);
		}
	}

	private void OnDrawGizmos()
	{
		if (!debug) { return; }
		
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(pivot.position, radius);
		
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(pivot.position, distance);
		
		var camera = Camera.main;
		if (camera == null) { return; }
		
		var mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
		var centerCell = (Vector3)tilemap.WorldToCell(mouseWorldPosition);

		var direction = centerCell - pivot.position;
		
		var targetPosition = pivot.position + direction.normalized * distance;
		Gizmos.DrawLine(pivot.position, targetPosition);
		
		var angle = Mathf.Atan2(direction.y, direction.x);
		
		var chordLength = Mathf.Sqrt(Mathf.Pow(distance, 2) + Mathf.Pow(range, 2));
		var angle2 = Mathf.Acos(distance / chordLength);
		var newCell1 = GetNewCell(angle - angle2, chordLength);
		var newCell2 = GetNewCell(angle + angle2, chordLength);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(newCell1, newCell2);
		Gizmos.DrawWireSphere(pivot.position, chordLength);
		
		var chordLength2 = Mathf.Sqrt(Mathf.Pow(0, 2) + Mathf.Pow(range, 2));
		var angle3 = Mathf.Acos(0 / chordLength2);
		var h = radius * (1 - Mathf.Cos(angle3));
		var y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(range, 2)) - (radius - h) - distance;
		
		var newX3 = y * Mathf.Cos(angle);
		var newY3 = y * Mathf.Sin(angle);
		var newDirection3 = new Vector3(newX3, newY3, 0);
		
		Gizmos.color = Color.yellow;
		var newCell3 = newCell1 + newDirection3;
		Gizmos.DrawLine(newCell1, newCell3);
		
		var newCell4 = newCell2 + newDirection3;
		Gizmos.DrawLine(newCell2, newCell4);
	}

	private Vector3 GetNewCell(float angle, float chordLength)
	{
		var newX = chordLength * Mathf.Cos(angle);
		var newY = chordLength * Mathf.Sin(angle);
		var newDirection = new Vector3(newX, newY, 0);
		var newCell = pivot.position + newDirection;
		return newCell;
	}
	
	private void OnEnable()
	{
		_playerActions.Enable();
	}
	
	private void OnDisable()
	{
		_playerActions.Disable();
	}
}
