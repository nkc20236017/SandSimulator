using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFlood : MonoBehaviour
{
    [Header("Tilemap Config")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap updateTilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] Vector2Int chunkSize = new(84, 45);

    [Header("Flood Config")]
    [SerializeField] private int detectTileCount;
    [SerializeField] private float updateInterval;

    private float _lastUpdateTime;
    private TileBase _currentTile;
    private List<Vector3Int> _chunkPositions = new();
    private List<Vector3Int> _currentTilePositions = new();
    private List<Vector3Int> _floodingTilePosition = new();
    private HashSet<Vector3Int> _visitedPositions = new();
    private Queue<Vector3Int> _positionsToFlood = new();

    public List<Vector3Int> FloodingTilePositions { get; private set; } = new();

    private void Start()
    {
        for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
        {
            for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
            {
                var position = new Vector3Int(x, y, 0);
                _chunkPositions.Add(position);
            }
        }

        _currentTilePositions = _chunkPositions;
    }

    private void Update()
    {
        if (!(Time.time - _lastUpdateTime > updateInterval)) { return; }

        _lastUpdateTime = Time.time;
        if (tilemap.GetUsedTilesCount() == 0) { return; }

        while (_currentTilePositions.Count > 0)
        {
            _visitedPositions.Clear();
            _positionsToFlood.Enqueue(_currentTilePositions[0]);

            while (_positionsToFlood.Count > 0)
            {
                var position = _positionsToFlood.Dequeue();
                Flood(position);
            }

            if (_floodingTilePosition.Count == 0)
            {
                _currentTilePositions.RemoveAt(0);
                return;
            }

            if (_floodingTilePosition.Count <= detectTileCount)
            {
                FloodingTilePositions.AddRange(_floodingTilePosition);
            }

            _currentTilePositions.RemoveAll(position => _floodingTilePosition.Contains(position));
            _floodingTilePosition.Clear();
        }

        _currentTile = null;
        _currentTilePositions = _chunkPositions;
        tilemap.SetTiles(FloodingTilePositions.ToArray(), null);
        var tileArray = new TileBase[FloodingTilePositions.Count];
        for (var i = 0; i < FloodingTilePositions.Count; i++)
        {
            tileArray[i] = tile;
        }
        updateTilemap.SetTiles(FloodingTilePositions.ToArray(), tileArray);
    }

    private void Flood(Vector3Int position)
    {
        if (_visitedPositions.Contains(position)) { return; }
        _visitedPositions.Add(position);

        if (_currentTile != null)
        {
            if (tilemap.GetTile(position) != _currentTile) { return; }
        }
        else
        {
            _currentTile = tilemap.GetTile(position);
        }

        _floodingTilePosition.Add(position);

        _positionsToFlood.Enqueue(position + Vector3Int.left);
        _positionsToFlood.Enqueue(position + Vector3Int.right);
        _positionsToFlood.Enqueue(position + Vector3Int.up);
        _positionsToFlood.Enqueue(position + Vector3Int.down);
    }
}