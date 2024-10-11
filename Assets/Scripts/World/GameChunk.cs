using UnityEngine.Tilemaps;
using UnityEngine;
using WorldCreation;
using System.Collections.Generic;

public struct GameChunk
{
    private Tilemap _Tilemap;
    private Vector2Int _position;
    private int[,] _grid;
    private int[,] _layerIndex;
    private List<GameObject> _summonLaterObjects;

    public Vector2Int Position => _position;
    public Vector2Int Size
    {
        get
        {
            return new Vector2Int(_grid.GetLength(0), _grid.GetLength(1));
        }
    }

    public GameChunk(Vector2Int position, Tilemap tilemap, Vector2Int chunkSize)
    {
        _Tilemap = tilemap;
        _position = position;
        _grid = new int[chunkSize.x, chunkSize.y];
        _layerIndex = new int[chunkSize.x, chunkSize.y];
        _summonLaterObjects = new();
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

    public Vector2Int GetRawWorldPosition(int x, int y)
    {
        return new
        (
            _position.x * Size.x + x,
            _position.y * Size.y + y
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

    public Vector2Int GetWithinChunkPosition(Vector2 position)
    {
        return new
        (
            (int)position.x % _grid.GetLength(0),
            (int)position.y % _grid.GetLength(1)
        );
    }
}
