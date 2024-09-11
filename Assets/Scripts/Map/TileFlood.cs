using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFlood : MonoBehaviour
{
    [Header("Tilemap Config")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap updateTilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] Vector2Int chunkSize = new(260, 150);

    [Header("Flood Config")]
    [SerializeField] private int detectTileCount;
    [SerializeField] private float updateInterval;

    private float _lastUpdateTime;
    private List<Vector3Int> _chunkPositions = new();
    private List<Vector3Int> _floodingTilePositions = new();
    private HashSet<Vector3Int> _usedTilePositions = new();
    private Dictionary<Vector3Int, TileBase> _previousTilemapState = new();

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
        {
            for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
            {
                var position = new Vector3Int(x, y, 0);
                _chunkPositions.Add(position);
            }
        }
    }

    private void Update()
    {
        if (UpdateFloodingTiles()) { return; }

        UpdateTilemaps();
    }

    private bool UpdateFloodingTiles()
    {
        if (!(Time.time - _lastUpdateTime > updateInterval)) { return true; }

        _lastUpdateTime = Time.time;
        if (tilemap.GetUsedTilesCount() == 0) { return true; }
        if (!HasTilemapChanged()) { return true; }

        foreach (var chunkPosition in _chunkPositions)
        {
            Flood(chunkPosition);
            if (_usedTilePositions.Count <= detectTileCount)
            {
                _floodingTilePositions.AddRange(_usedTilePositions);
            }
            _usedTilePositions.Clear();
        }

        return false;
    }

    private void Flood(Vector3Int startPosition)
    {
        var queue = new Queue<Vector3Int>();
        queue.Enqueue(startPosition);

        while (queue.Count > 0)
        {
            var position = queue.Dequeue();
            if (_usedTilePositions.Contains(position)) { continue; }
            if (tilemap.GetTile(position) != tile) { continue; }
            if (_usedTilePositions.Count > detectTileCount) { break; }

            _usedTilePositions.Add(position);

            queue.Enqueue(position + Vector3Int.up);
            queue.Enqueue(position + Vector3Int.down);
            queue.Enqueue(position + Vector3Int.left);
            queue.Enqueue(position + Vector3Int.right);
        }
    }

    private bool HasTilemapChanged()
    {
        for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
        {
            for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
            {
                var position = new Vector3Int(x, y, 0);
                var tileBase = tilemap.GetTile(position);
                if (_previousTilemapState.TryGetValue(position, out var previousTile))
                {
                    if (previousTile != tileBase)
                    {
                        _previousTilemapState[position] = tileBase;
                        return true;
                    }
                }
                else
                {
                    _previousTilemapState[position] = tileBase;
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateTilemaps()
    {
        var tilePos = new Vector3Int[_floodingTilePositions.Count];
        var mapTileArray = new TileBase[_floodingTilePositions.Count];
        var updateTileArray = new TileBase[_floodingTilePositions.Count];
        for (var i = 0; i < _floodingTilePositions.Count; i++)
        {
            tilePos[i] = _floodingTilePositions[i];
            mapTileArray[i] = null;
            updateTileArray[i] = tile;
        }
        tilemap.SetTiles(tilePos, mapTileArray);
        updateTilemap.SetTiles(tilePos, updateTileArray);
        _floodingTilePositions.Clear();
    }
}