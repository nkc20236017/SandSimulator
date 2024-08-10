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

	private Camera _camera;
	
	public List<Vector3Int> AbsorptionTilePositions { get; private set; } = new();

	private void Start()
	{
		_camera = Camera.main;
	}

	private void Update()
	{
		RotateToCursorDirection();
		
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
			tilemap.SetColliderType(tilePosition, UnityEngine.Tilemaps.Tile.ColliderType.None);
			
			var direction = (pivot.position - tilemap.GetCellCenterWorld(tilePosition)).normalized;
			var newTilePosition = tilePosition + new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
			tilemap.SetTile(newTilePosition, tile);
			tilemap.SetTile(tilePosition, null);
			
			if (Vector3.Distance(tilemap.GetCellCenterWorld(newTilePosition), pivot.position) <= deleteDistance)
			{
				tilemap.SetTile(newTilePosition, null);
			}
		}
	}
	
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
		
		var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
