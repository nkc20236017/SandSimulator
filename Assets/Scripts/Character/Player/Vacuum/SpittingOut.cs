using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;

public class SpittingOut : MonoBehaviour
{
	[Header("Tile Config")]
	[SerializeField] private Tilemap tilemap;
	
	[Header("SpittingOut Config")]
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

	private void Start()
	{
		_camera = Camera.main;
		_lastUpdateTime = Time.time;
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(1))
		{
			_lastUpdateTime = Time.time;
		}
		
		if (Input.GetMouseButton(1))
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
	}
	
	private void GenerateTile()
	{
		var mousePosition = Input.mousePosition;
		mousePosition.z = _camera.nearClipPlane;
		var worldPosition = _camera.ScreenToWorldPoint(mousePosition);

		var direction = (worldPosition - pivot.position).normalized;
		var targetPosition = pivot.position + direction * distance;

		var randomOffset = Random.Range(-range / 2, range / 2);
		var randomPosition = targetPosition + new Vector3(randomOffset, 0, 0);
		var tilePosition = tilemap.WorldToCell(randomPosition);
		
		// TODO: タイルを生成する処理
		// tilemap.SetTile(tilePosition, タイル);
	}
	
	private void UpdateTile()
	{
		
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
}
