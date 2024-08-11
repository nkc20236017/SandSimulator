using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Absorption : MonoBehaviour
{
	[Header("Tile Config")]
	[SerializeField] private Tilemap tilemap;
	
	[Header("Suction Config")]
	[SerializeField] private Transform pivot;
	[SerializeField] private float sightAngle;
	[SerializeField] private float absorptionDistance;
	[SerializeField] private float deleteDistance;
	[SerializeField] private int capacity;

	private int _currentCapacity;
	private int[] _tileCapacityCount;
	private Camera _camera;
	
	public List<Vector3Int> AbsorptionTilePositions { get; private set; } = new();

	private void Start()
	{
		_camera = Camera.main;
		_tileCapacityCount = new int[TilesUpdate.Instance.TileDatas.Length];
	}

	private void Update()
	{
		RotateToCursorDirection();

		if (capacity != 0)
		{
			if (_currentCapacity >= capacity) { return; }
		}

		if (Input.GetMouseButtonUp(0))
		{
			AbsorptionTilePositions.Clear();
		}
		
		if (!Input.GetMouseButton(0)) { return; }
		
		GetAbsorptionTilePositions();
		AbsorbTiles();
	}
	
	private void RotateToCursorDirection()
	{
		var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
		var direction = mouseWorldPos - pivot.position;
		var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		if (pivot.parent.localScale.x < 0)
		{
			angle += 180;
		}
		
		pivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}
	
	private void AbsorbTiles()
	{
		AbsorptionTilePositions = AbsorptionTilePositions.OrderBy(x => Random.value).ToList();
		
		foreach (var tilePosition in AbsorptionTilePositions)
		{
			var tile = tilemap.GetTile(tilePosition);
			if (tile == null) { continue; }
			
			var direction = (pivot.position - tilemap.GetCellCenterWorld(tilePosition)).normalized;
			// var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
			// var centerCell = (Vector3)tilemap.WorldToCell(mouseWorldPos);
			// var closestPoint = GetClosesPointOnLine(pivot.position, centerCell, tilemap.GetCellCenterWorld(tilePosition));
			var newPosition = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
			var newTilePosition = tilePosition + newPosition;
			tilemap.SetTile(newTilePosition, tile);
			tilemap.SetTile(tilePosition, null);
			// tilemap.SetColliderType(newTilePosition, UnityEngine.Tilemaps.Tile.ColliderType.None);
			// tilemap.SetColliderType(tilePosition, UnityEngine.Tilemaps.Tile.ColliderType.Sprite);
			
			if (Vector3.Distance(tilemap.GetCellCenterWorld(newTilePosition), pivot.position) <= deleteDistance)
			{
				if (capacity != 0)
				{
					var index = Array.FindIndex(TilesUpdate.Instance.TileDatas, t => t.tile == tile);
					_tileCapacityCount[index]++;
					_currentCapacity++;
				}
				tilemap.SetTile(newTilePosition, null);
			}
		}
	}
	
	// private bool CheckAbsorptionTilePosition(Vector3Int position)
	// {
	// 	return AbsorptionTilePositions.Any(updateTile => updateTile == position);
	// }
	
	// private Vector3 GetClosesPointOnLine(Vector3 start, Vector3 end, Vector3 point)
	// {
	// 	var line = end - start;
	// 	var length = line.magnitude;
	// 	line.Normalize();
	// 	
	// 	var v = point - start;
	// 	var d = Vector3.Dot(v, line);
	// 	d = Mathf.Clamp(d, 0f, length);
	// 	
	// 	return start + line * d;
	// }
	
	private void GetAbsorptionTilePositions()
	{
		AbsorptionTilePositions.Clear();
		var bounds = new BoundsInt(tilemap.WorldToCell(pivot.position) - new Vector3Int((int)absorptionDistance, (int)absorptionDistance, 0), new Vector3Int((int)absorptionDistance * 2, (int)absorptionDistance * 2, 1));
		foreach (var tilePosition in bounds.allPositionsWithin)
		{
			var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
			var centerCell = (Vector3)tilemap.WorldToCell(mouseWorldPos);
			
			var direction1 = (Vector3)tilemap.WorldToCell(tilePosition) - tilemap.WorldToCell(pivot.position);
			var direction2 = (Vector3)tilemap.WorldToCell(centerCell) - tilemap.WorldToCell(pivot.position);
			var angle = Vector3.Angle(direction1, direction2);
			
			var distance = Vector3.Distance(tilemap.GetCellCenterWorld(tilePosition), pivot.position);
			
			if (angle <= sightAngle && distance < absorptionDistance && distance > deleteDistance)
			{
				AbsorptionTilePositions.Add(tilePosition);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(pivot.position, absorptionDistance);
		
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(pivot.position, deleteDistance);
		
		Gizmos.color = Color.green;
		var camera = Camera.main;
		if (camera == null) { return; }
		
		var mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
		var centerCell = (Vector3)tilemap.WorldToCell(mouseWorldPos);
		
		var angleInRadians = sightAngle * Mathf.Deg2Rad;
		var direction2 = centerCell - pivot.position;
		var angle = Mathf.Atan2(direction2.y, direction2.x);
			
		var newAngle1 = angle - angleInRadians;
		var newX1 = absorptionDistance * Mathf.Cos(newAngle1);
		var newY1 = absorptionDistance * Mathf.Sin(newAngle1);			
		var newDirection1 = new Vector3(newX1, newY1, 0);
		var newCell1 = pivot.position + newDirection1;
		Gizmos.DrawLine(pivot.position, newCell1);
		
		var newAngle2 = angle + angleInRadians;
		var newX2 = absorptionDistance * Mathf.Cos(newAngle2);
		var newY2 = absorptionDistance * Mathf.Sin(newAngle2);
		var newDirection2 = new Vector3(newX2, newY2, 0);
		var newCell2 = pivot.position + newDirection2;
		Gizmos.DrawLine(pivot.position, newCell2);
	}
}
