using UnityEngine.Tilemaps;
using UnityEngine;
using WorldCreation;

public struct Chunk
{
    private ManagedRandom _randamization;
    private Tilemap _Tilemap;
    private Vector2Int _position;
    private int[,] _grid;
    private int[,] _layerIndex;

    public Vector2Int Position => _position;

    public Chunk(ManagedRandom random, Vector2Int position, Tilemap tilemap, int[,] grid)
    {
        _randamization = random;
        _Tilemap = tilemap;
        _position = position;
        _grid = grid;
        _layerIndex = new int[grid.GetLength(0), grid.GetLength(1)];
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
        return _grid[position.x, position.y];
    }

    public int GetBlockID(int x, int y)
    {
        return _grid[x, y];
    }

    public void SetBlock(Vector2Int position, int id)
    {
        _grid[position.x, position.y] = id;
    }
    public void SetBlock(int x, int y, int id)
    {
        _grid[x, y] = id;
    }
    public void SetLayerIndex(int x, int y, int index)
    {
        _layerIndex[x, y] = index;
    }
    public int GetLayerIndex(int x, int y)
    {
        return _layerIndex[x, y];
    }

    public int GetChunkLength(int dimension)
    {
        return _grid.GetLength(dimension);
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
