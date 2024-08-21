using UnityEngine;
using UnityEngine.Tilemaps;

public class SpittingOut : MonoBehaviour
{
	[Header("Tile Config")]
	[SerializeField] private Tilemap tilemap;
	
	[Header("SpittingOut Config")]
	[SerializeField] private Transform pivot;
	[SerializeField] private float radius;
	[SerializeField] private float distance;
	[SerializeField] private float range;
	
	[Header("Instantiation Config")]
	[SerializeField] private float interval;
	
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
		// if (Input.GetMouseButtonUp(1))
		// {
		// 	_lastUpdateTime = Time.time;
		// }
		//
		// if (Input.GetMouseButton(1))
		// {
		// 	if (Time.time - _lastUpdateTime > interval)
		// 	{
		// 		_lastUpdateTime = Time.time;
		// 		GenerateTile();
		// 	}
		// 	
		// 	UpdateTile();
		// }
	}
	
	private void GenerateTile()
	{
		
	}
	
	private void UpdateTile()
	{
		
	}

	private void OnDrawGizmos()
	{
		if (!debug) { return; }
		
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(pivot.position, radius);
		
		var camera = Camera.main;
		if (camera == null) { return; }

		var mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
		var centerCell = (Vector3)tilemap.WorldToCell(mouseWorldPosition);

		var direction = centerCell - pivot.position;
		var theta = Mathf.Atan2(direction.y, direction.x);

		var a1b1 = pivot.position + new Vector3(range * Mathf.Cos(theta + radius), range * Mathf.Sin(theta + radius));
		var a2b2 = pivot.position + new Vector3(range * Mathf.Cos(theta - radius), range * Mathf.Sin(theta - radius));

		var c1d1 = a1b1 + new Vector3(radius * Mathf.Cos(theta + distance), radius * Mathf.Sin(theta + distance));
		var c2d2 = a2b2 + new Vector3(radius * Mathf.Cos(theta - distance), radius * Mathf.Sin(theta - distance));
		
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(a1b1, a2b2);
		
		Gizmos.color = Color.green;
		Gizmos.DrawLine(a1b1, c1d1);
		Gizmos.DrawLine(a2b2, c2d2);
	}
}
