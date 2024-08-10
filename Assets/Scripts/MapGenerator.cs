using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[Serializable]
public struct MapTileData
{
	public string name;
	public TileBase tile;
	public float generationRate;
}

public class MapGenerator : MonoBehaviour
{
	[Header("Tile Config")]
	[SerializeField] private Tilemap tilemap;
	[SerializeField] private MapTileData[] tiles;
	
	[Header("Map Config")]
	[SerializeField] private Vector2Int mapSize;
	[SerializeField] private float seed;
	[SerializeField] private float modifier;

	private void Start()
	{
		if (seed == 0)
		{
			seed = Random.Range(-10000, 10000);
		}
		GenerateMap();
	}
	
	private void GenerateMap()
	{
		for (var x = 0; x < mapSize.x; x++)
		{
			for (var y = 0; y < mapSize.y; y++)
			{
				var spawnPoint = Mathf.RoundToInt(Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));
				if (spawnPoint == 1)
				{
					var tileIndex = UnityEngine.Random.Range(0, tiles.Length);
					tilemap.SetTile(new Vector3Int(x, y, 0), tiles[tileIndex].tile);
				}
			}
		}
	}
}
