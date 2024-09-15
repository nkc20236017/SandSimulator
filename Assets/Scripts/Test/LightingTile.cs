using UnityEngine;
using UnityEngine.Tilemaps;

public class LightingTile : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] Tilemap mapTilemap;
	[SerializeField] Tilemap backgroundTilemap;
	[SerializeField] private int radius;
	
	[Header("Shadow")]
	[SerializeField] private bool mapShadow;
	[SerializeField] private bool backgroundShadow;

	private void Update()
	{
		if (mapShadow) { SetShadow(mapTilemap); }
		if (backgroundShadow) { SetShadow(backgroundTilemap); }
		SetLight();
	}
	
	private void SetShadow(Tilemap tilemap)
	{
		var bounds = tilemap.cellBounds.allPositionsWithin;
		foreach (var bound in bounds)
		{
			if (tilemap.GetTile(bound) == null) { continue; }
			
			tilemap.SetColor(bound, Color.black);
		}
	}

	private void SetLight()
	{
		var propBounds = mapTilemap.cellBounds.allPositionsWithin;
		foreach (var propBound in propBounds)
		{
			// if (mapTilemap.GetTile(propBound) != tile) { continue; }
			
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
