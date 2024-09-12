using UnityEngine;
using UnityEngine.Tilemaps;

public class LightingTile : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] Tilemap mapTilemap;
	[SerializeField] Tilemap backgroundTilemap;
	[SerializeField] Tilemap propTilemap;
	[SerializeField] TileBase tile;
	[SerializeField] private int radius;

	private void Update()
	{
		SetShadow();
		SetLight();
	}
	
	private void SetShadow()
	{
		var bounds = mapTilemap.cellBounds.allPositionsWithin;
		foreach (var bound in bounds)
		{
			if (mapTilemap.GetTile(bound) == null) { continue; }
			
			mapTilemap.SetColor(bound, Color.black);
		}
		
		// var backgroundBounds = backgroundTilemap.cellBounds.allPositionsWithin;
		// foreach (var bound in backgroundBounds)
		// {
		// 	if (backgroundTilemap.GetTile(bound) == null) { continue; }
		// 	
		// 	backgroundTilemap.SetColor(bound, Color.black);
		// }
	}

	private void SetLight()
	{
		var propBounds = propTilemap.cellBounds.allPositionsWithin;
		foreach (var propBound in propBounds)
		{
			if (propTilemap.GetTile(propBound) != tile) { continue; }
			
			var mapBound = new Vector3Int(propBound.x, propBound.y, 0);
			for (var y = -radius; y <= radius; y++)
			{
				for (var x = -radius; x <= radius; x++)
				{
					var position = new Vector3Int(mapBound.x + x, mapBound.y + y, 0);
					
					var distance = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
					if (distance > radius) { continue; }
					
					var color = mapTilemap.GetColor(position);
					var ratio = 1 - distance / radius;
					color.r = Mathf.Max(color.r, color.r + ratio);
					color.g = Mathf.Max(color.g, color.g + ratio);
					color.b = Mathf.Max(color.b, color.b + ratio);
					mapTilemap.SetColor(position, color);
					
					// color = backgroundTilemap.GetColor(position);
					// color.r = Mathf.Max(color.r, color.r + ratio);
					// color.g = Mathf.Max(color.g, color.g + ratio);
					// color.b = Mathf.Max(color.b, color.b + ratio);
					// backgroundTilemap.SetColor(position, color);
				}
			}
		}
	}
}
