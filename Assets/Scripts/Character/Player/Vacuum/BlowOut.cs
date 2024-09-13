using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BlowOut : MonoBehaviour
{
	[Header("Tile Config")]
	[SerializeField] private Tilemap updateTilemap;
	[SerializeField] private Tilemap mapTilemap;
	[SerializeField] private BlockDatas blockDatas;
	
	[Header("BlowOut Config")]
	[SerializeField] private BlockType blockType;
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
	private PlayerMovement _playerMovement;
	private Camera _camera;
	private PlayerActions _playerActions;
	private PlayerActions.VacuumActions VacuumActions => _playerActions.Vacuum;

	private void Awake()
	{
		_playerActions = new PlayerActions();
		
		_playerMovement = GetComponentInParent<PlayerMovement>();
	}

	private void Start()
	{
		_camera = Camera.main;
		_lastUpdateTime = Time.time;
		
		VacuumActions.SpittingOut.started += _ => _playerMovement.IsMoveFlip = false;
		VacuumActions.SpittingOut.canceled += _ => CancelBlowOut();
	}

	private void Update()
	{
		if (VacuumActions.SpittingOut.IsPressed())
		{
			BlowOutTiles();
		}
	}

	private void BlowOutTiles()
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

		// UpdateTile();
		UpTiles();
	}
	
	private void GenerateTile()
	{
		var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
		var centerCell = (Vector3)updateTilemap.WorldToCell(mouseWorldPosition);

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
			var randomCell = updateTilemap.WorldToCell(randomPosition);
			updateTilemap.SetTile(randomCell, blockDatas.GetBlock(blockType).tile);
			// tilemap.SetColliderType(randomCell, Tile.ColliderType.None);
		}
	}

	private void UpTiles()
	{
		var bounds = new BoundsInt(updateTilemap.WorldToCell(pivot.position) - new Vector3Int((int)radius, (int)radius, 0), new Vector3Int((int)radius * 2, (int)radius * 2, 1));
		var getTilesBlock = updateTilemap.GetTilesBlock(bounds);
		var getTilesBlock2 = mapTilemap.GetTilesBlock(bounds);
		getTilesBlock = getTilesBlock.Concat(getTilesBlock2).ToArray();
		getTilesBlock = getTilesBlock.Where(x => x != null).ToArray();
		if (getTilesBlock.Length == 0) { return; }
        
		foreach (var tilePosition in bounds.allPositionsWithin)
		{
			Tilemap tilemap;
			if (updateTilemap.HasTile(tilePosition))
			{
				tilemap = updateTilemap;
			}
			else if (mapTilemap.HasTile(tilePosition))
			{
				tilemap = mapTilemap;
			}
			else
			{
				continue;
			}
            
			var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
			var centerCell = (Vector3)tilemap.WorldToCell(mouseWorldPosition);

			var direction1 = (Vector3)tilemap.WorldToCell(tilePosition) - tilemap.WorldToCell(pivot.position);
			var direction2 = (Vector3)tilemap.WorldToCell(centerCell) - tilemap.WorldToCell(pivot.position);
			var angle = Vector3.Angle(direction1, direction2);

			var distance = Vector3.Distance(tilemap.GetCellCenterWorld(tilePosition), pivot.position);

			if (angle <= 30 && distance <= radius)
			{
				var direction = (Vector3)tilemap.WorldToCell(tilePosition) - tilemap.WorldToCell(pivot.position);
				var newTilePosition = Vector3Int.RoundToInt(tilePosition + direction.normalized);
				if (updateTilemap.HasTile(newTilePosition) || mapTilemap.HasTile(newTilePosition)) { continue; }
            
				var tile = tilemap.GetTile(tilePosition);
				if (tile == null) { continue; }
				
				if (tile == blockDatas.GetBlock(BlockType.Sand).tile)
				{
					updateTilemap.SetTile(newTilePosition, tile);
				}
				else
				{
					mapTilemap.SetTile(newTilePosition, tile);
				}
				tilemap.SetTile(tilePosition, null);
			}
		}
	}
	
	private void UpdateTile()
	{
		var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
		var centerCell = (Vector3)updateTilemap.WorldToCell(mouseWorldPosition);

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
		
		var max = new Vector2(Mathf.Max(a.x, b.x, c.x, d.x), Mathf.Max(a.y, b.y, c.y, d.y));
		var min = new Vector2(Mathf.Min(a.x, b.x, c.x, d.x), Mathf.Min(a.y, b.y, c.y, d.y));

		var angleAC = Mathf.Atan2(c.y - a.y, c.x - a.x);
		var angleBD = Mathf.Atan2(d.y - b.y, d.x - b.x);
		
		angleAC = Mathf.Abs(angleAC);
		angleAC *= Mathf.Rad2Deg;
		
		angleBD = Mathf.Abs(angleBD);
		angleBD *= Mathf.Rad2Deg;

		for (y = min.y; y <= max.y; y++)
		{
			for (var x = min.x; x < max.x; x++)
			{
				var position = Vector3Int.RoundToInt(new Vector3(x, y, 0));
				if (!updateTilemap.HasTile(position) && !mapTilemap.HasTile(position)) { continue; }
				
				var tileToPivotDistance = Vector3.Distance(pivot.position, position);
				if (tileToPivotDistance > radius) { continue; }
				
				// 座標a, b, c, dの四角の内側にあるかどうかを判定する
				var angleAP = Mathf.Atan2(position.y - a.y, position.x - a.x);
				var angleBP = Mathf.Atan2(position.y - b.y, position.x - b.x);
				var angleCP = Mathf.Atan2(position.y - c.y, position.x - c.x);
				var angleDP = Mathf.Atan2(position.y - d.y, position.x - d.x);
				
				angleAP = Mathf.Abs(angleAP);
				angleAP *= Mathf.Rad2Deg;
				
				angleBP = Mathf.Abs(angleBP);
				angleBP *= Mathf.Rad2Deg;
				
				angleCP = Mathf.Abs(angleCP);
				angleCP *= Mathf.Rad2Deg;
				
				angleDP = Mathf.Abs(angleDP);
				angleDP *= Mathf.Rad2Deg;

				if (angleAP < angleAC && angleBP < angleBD && angleCP < angleAC && angleDP < angleBD)
				{
					var newTilePosition = Vector3Int.RoundToInt(position + (pivot.position - centerCell).normalized * 2);
					if (updateTilemap.HasTile(newTilePosition) || mapTilemap.HasTile(newTilePosition)) { continue; }

					Debug.DrawLine(newTilePosition, position, Color.red, 0.1f);
					updateTilemap.SetTile(newTilePosition, blockDatas.GetBlock(blockType).tile);
					updateTilemap.SetTile(position, null);
					mapTilemap.SetTile(position, null);
				}
			}
		}
	}
	
	private void CancelBlowOut()
	{
		_lastUpdateTime = Time.time;
		_playerMovement.IsMoveFlip = true;
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
		if (updateTilemap == null) { return; }
		
		var centerCell = (Vector3)updateTilemap.WorldToCell(mouseWorldPosition);

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
