using System.Collections.Generic;
using System.Threading;
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
    private List<Vector3Int> _currentChunkPositions = new();
    private List<Vector3Int> _chunkPositions = new();
    private List<Vector3Int> _floodTilePositions = new();
    private List<Vector3Int> _updateTilePositions = new();

    private void Start()
    {
        _chunkPositions = Initialization();
    }

    private void Update()
    {
        if (!(Time.time - _lastUpdateTime > updateInterval)) { return; }

        _lastUpdateTime = Time.time;
        if (tilemap.GetUsedTilesCount() == 0) { return; }

        _currentChunkPositions = Initialization();
        if (_chunkPositions.Equals(_currentChunkPositions)) { return; }

        _chunkPositions = _currentChunkPositions;
        _floodTilePositions.Clear();
        _updateTilePositions.Clear();

        AddFloodPositions();
        UpdateTiles();
    }

    private List<Vector3Int> Initialization()
    {
        var _tilePositions = new List<Vector3Int>();
        for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
        {
            for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
            {
                var position = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(position) != tile) { continue; }

                _tilePositions.Add(position);
            }
        }

        return _tilePositions;
    }

    private void AddFloodPositions()
    {
        while (_currentChunkPositions.Count > 0)
        {
            var position = _currentChunkPositions[0];
            var thread = new Thread(() => DetectFloodThread(position), 100000);
            thread.Start();
            thread.Join();

            if (_floodTilePositions.Count <= detectTileCount)
            {
                _updateTilePositions.AddRange(_floodTilePositions);
            }

            _currentChunkPositions.RemoveAll(pos => _floodTilePositions.Contains(pos));
            _floodTilePositions.Clear();
        }
    }

    private void DetectFloodThread(Vector3Int position)
    {
        var stack = new Stack<Vector3Int>();
        stack.Push(position);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (_floodTilePositions.Contains(current)) { continue; }
            if (!_chunkPositions.Contains(current)) { continue; }

            _floodTilePositions.Add(current);

            stack.Push(current + Vector3Int.up);
            stack.Push(current + Vector3Int.down);
            stack.Push(current + Vector3Int.left);
            stack.Push(current + Vector3Int.right);
        }
    }

    private void UpdateTiles()
    {
        foreach (var position in _updateTilePositions)
        {
            tilemap.SetTile(position, null);
            updateTilemap.SetTile(position, tile);
        }
    }
}