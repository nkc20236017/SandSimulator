using UnityEngine;

public class SceneGrid : MonoBehaviour
{
	[SerializeField] private bool viewGrid = true;
	[SerializeField, Min(0)] private int gridSize = 16;
	[SerializeField, Min(1)] private int worldSize;
	
	private void OnDrawGizmos()
	{
		if (!viewGrid) { return; }
		
		var camera = Camera.main;
		if (camera == null) { return; }

		var cameraPosition = camera.transform.position;
		var cameraSize = camera.orthographicSize;
		var aspect = camera.aspect;

		var halfWidth = cameraSize * aspect * worldSize;
		var halfHeight = cameraSize * worldSize;

		var startX = Mathf.Floor(cameraPosition.x - halfWidth);
		var endX = Mathf.Ceil(cameraPosition.x + halfWidth);
		var startY = Mathf.Floor(cameraPosition.y - halfHeight);
		var endY = Mathf.Ceil(cameraPosition.y + halfHeight);

		for (var x = startX; x <= endX; x++)
		{
			if (x % gridSize == 0)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
			}
			else if (x % (gridSize / 2) == 0)
			{
				Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
			}
			else
			{
				Gizmos.color = new Color(0, 0, 0, 0.25f);
			}
			
			Gizmos.DrawLine(new Vector3(x, startY), new Vector3(x, endY));
		}

		for (var y = startY; y <= endY; y++)
		{
			if (y % gridSize == 0)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
			}
			else if (y % (gridSize / 2) == 0)
			{
				Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
			}
			else
			{
				Gizmos.color = new Color(0, 0, 0, 0.25f);
			}
			
			Gizmos.DrawLine(new Vector3(startX, y), new Vector3(endX, y));
		}
	}
}

