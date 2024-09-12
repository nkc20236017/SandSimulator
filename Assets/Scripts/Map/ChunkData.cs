using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkData : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private TileData[] tiles;
	[SerializeField] private float updateInterval = 0.01f;
	[SerializeField] private Vector2Int chunkSize = new(84, 45);
	
	public bool HasTilemapChanged(Tilemap tilemap, Dictionary<Vector3Int, TileBase> previousTilemapState)
	{
		var currentTilemapState = new Dictionary<Vector3Int, TileBase>();
		for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
		{
			for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
			{
				var position = new Vector3Int(x, y, 0);
				var tile = tilemap.GetTile(position);
				currentTilemapState[position] = tile;
			}
		}

		var hasChanged = !currentTilemapState.SequenceEqual(previousTilemapState);
		previousTilemapState = currentTilemapState;
		return hasChanged;
	}
	
	public TileData GetTileData(BlockType type)
	{
		return (from tile in tiles where tile.type == type select tile).FirstOrDefault();
	}
}

