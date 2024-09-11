using UnityEngine;

public class GridView : MonoBehaviour
{
	[SerializeField] private bool viewGrid = true;
	[SerializeField] private Vector2Int gridSize;
	[SerializeField] private Vector2Int worldSize;
	
	private void OnDrawGizmos()
	{
		if (!viewGrid) { return; }
		if (worldSize.x == 0 || worldSize.y == 0) { return; }
		
		var startX = -worldSize.x / 2;
		var endX = worldSize.x / 2;
		var startY = -worldSize.y / 2;
		var endY = worldSize.y / 2;

		for (var x = startX; x <= endX; x++)
		{
			if (x % gridSize.x == 0)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
			}
			else
			{
				Gizmos.color = new Color(0, 0, 0, 0.5f);
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
				Gizmos.color = new Color(0, 0, 0, 0.5f);
			}
			
			Gizmos.DrawLine(new Vector3(startX, y), new Vector3(endX, y));
		}
	}
}

