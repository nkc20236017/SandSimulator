using UnityEngine.Tilemaps;
using UnityEngine;

public struct Chunk
{
    private ManagedRandom _randamization;
    private Tilemap _Tilemap;
    private int[,] _cache;

    public Chunk(ManagedRandom random, Tilemap tilemap)
    {
        _randamization = random;
        _Tilemap = tilemap;
        _cache = null;
    }

    public int GetNoise(int executionOrder)
    {
        return _randamization.Order(executionOrder, 0, int.MaxValue);
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
}
