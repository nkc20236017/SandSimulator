using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingTileDetect : MonoBehaviour
{
	[Header("Tilemap Config")]
	[SerializeField] private Tilemap updateTilemap;
	[SerializeField] private TileData[] tiles;
	
	[Header("Chunk Config")]
	[SerializeField] private Vector2Int chunkSize = new(84, 45);
	
	[Header("Falling Config")]
	[SerializeField, Min(0)] private int minClusterSize;
	
	private Tilemap _tilemap;
	private Dictionary<Vector3Int, TileBase> _previousTilemapState = new();
	
	private void Awake()
	{
		_tilemap = GetComponent<Tilemap>();
	}
	
	private void Update()
	{
		if (_tilemap.GetUsedTilesCount() == 0) { return; }
		if (!HasTilemapChanged()) { return; }

		foreach (var tile in tiles)
		{
			var positions = DetectClusters(tile.tile);
			_tilemap.SetTiles(positions.ToArray(), null);
			var tileArray = new TileBase[positions.Count];
			for (var i = 0; i < positions.Count; i++)
			{
				tileArray[i] = tile.tile;
			}
			
			updateTilemap.SetTiles(positions.ToArray(), tileArray);
		}
	}
	
	private bool HasTilemapChanged()
	{
		var currentTilemapState = new Dictionary<Vector3Int, TileBase>();
		for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
		{
			for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
			{
				var position = new Vector3Int(x, y, 0);
				var tile = _tilemap.GetTile(position);
				currentTilemapState[position] = tile;
			}
		}
		
		return !_previousTilemapState.SequenceEqual(currentTilemapState);
	}
	
	public List<Vector3Int> DetectClusters(TileBase tile)
	{
		var clusterTiles = new List<Vector3Int>();
		var bounds = _tilemap.cellBounds;

		for (var x = bounds.xMin; x < bounds.xMax; x++)
		{
			for (var y = bounds.yMin; y < bounds.yMax; y++)
			{
				var tilePosition = new Vector3Int(x, y, 0);
				if (clusterTiles.Contains(tilePosition) || _tilemap.GetTile(tilePosition) != tile) { continue; }

				var cluster = new List<Vector3Int>();
				FloodFill(tilePosition, cluster, tile);

				if (cluster.Count <= minClusterSize)
				{
					clusterTiles.AddRange(cluster);
				}
			}
		}

		return clusterTiles;
	}

	private void FloodFill(Vector3Int startPos, List<Vector3Int> cluster, TileBase tile)
	{
		var queue = new Queue<Vector3Int>();
		queue.Enqueue(startPos);

		while (queue.Count > 0)
		{
			var currentPos = queue.Dequeue();
			if (cluster.Contains(currentPos) || _tilemap.GetTile(currentPos) != tile) { continue; }

			cluster.Add(currentPos);
			
			queue.Enqueue(currentPos + Vector3Int.up);
			queue.Enqueue(currentPos + Vector3Int.down);
			queue.Enqueue(currentPos + Vector3Int.left);
			queue.Enqueue(currentPos + Vector3Int.right);
		}
	}
}

