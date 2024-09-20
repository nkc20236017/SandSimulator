using UnityEngine.Tilemaps;
using UnityEngine;
using WorldCreation;

public struct Chunk
{
    private ManagedRandom _randamization;
    private Tilemap _Tilemap;
    private Vector2Int _position;
    private int[,] _cache;

    public Vector2Int Position => _position;

    public Chunk(ManagedRandom random, Vector2Int position, Tilemap tilemap, int[,] tile)
    {
        _randamization = random;
        _Tilemap = tilemap;
        _position = position;
        _cache = tile;
    }

    public int GetNoise(int executionOrder, int maxValue = int.MaxValue)
    {
        return _randamization.Order(executionOrder, 0, maxValue);
    }

    public Tilemap TileMap
    {
        get => _Tilemap;
        set
        {
            if (_Tilemap == null)
            {
                _Tilemap = value;
            }
        }
    }

    public int GetBlockID(Vector2Int position)
    {
        return _cache[position.x, position.y];
    }

    public int GetBlockID(int x, int y)
    {
        return _cache[x, y];
    }

    public void SetBlock(Vector2Int position, int id)
    {
        _cache[position.x, position.y] = id;
    }
    public void SetBlock(int x, int y, int id)
    {
        _cache[x, y] = id;
    }

    public int GetChunkLength(int dimension)
    {
        return _cache.GetLength(dimension);
    }

    public Vector2Int GetWorldPosition(int x, int y, Vector2Int chunkSize)
    {
        return new
        (
            _position.x * chunkSize.x + x,
            _position.y * chunkSize.y + y
        );
    }

    public Vector2Int GetWorldPosition(Vector2Int position, Vector2Int chunkSize)
    {
        return new
        (
            _position.x * chunkSize.x + position.x,
            _position.y * chunkSize.y + position.y
        );
    }
}
