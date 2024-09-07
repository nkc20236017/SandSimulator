using UnityEngine;

public class SceneGrid : MonoBehaviour
{
	[SerializeField] private bool viewGrid = true;
	[SerializeField] private Vector2Int gridSize;
	[SerializeField] private Vector2Int worldSize;
	
	private void OnDrawGizmos()
	{
		if (!viewGrid) { return; }
		
		var camera = Camera.main;
		if (camera == null) { return; }

		var cameraPosition = camera.transform.position;
		var cameraSize = camera.orthographicSize;
		var aspect = camera.aspect;

		var halfWidth = cameraSize * aspect * worldSize.x;
		var halfHeight = cameraSize * worldSize.y;

		var startX = -worldSize.x;
		var endX = worldSize.x;
		var startY = -worldSize.y;
		var endY = worldSize.y;

		for (var x = startX; x <= endX; x++)
		{
			if (x % gridSize.x == 0)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
			}
			else
			{
				Gizmos.color = new Color(0, 0, 0, 0.25f);
			}
			
			Gizmos.DrawLine(new Vector3(x, startY), new Vector3(x, endY));
		}

		for (var y = startY; y <= endY; y++)
		{
			if (y % gridSize.y == 0)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
			}
			else
			{
				Gizmos.color = new Color(0, 0, 0, 0.25f);
			}
			
			Gizmos.DrawLine(new Vector3(startX, y), new Vector3(endX, y));
		}
	}
}

