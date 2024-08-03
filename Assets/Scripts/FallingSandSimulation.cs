using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum BrushType
{
    Circle,
    Square,
    Smooth,
}

public enum TileType
{
    None,
    Sand,
    Mud,
    Water,
}

[Serializable]
public struct TileData
{
    public TileType type;
    public TileBase tile;
    public bool canFall;
    public bool canWater;
}

public class FallingSandSimulation : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private BrushType brushType;
    [SerializeField] private TileType tileType;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileData[] tiles;
    [SerializeField] private int size = 5;
    [SerializeField] private float updateInterval = 0.5f;

    private float _lastUpdateTime;
    private Camera _camera;

    private void Start()
    {
	    _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var centerCell = tilemap.WorldToCell(mouseWorldPos);
            foreach (var tile in tiles)
            {
                if (tile.type != tileType) { continue; }

                if (tileType == TileType.None)
                {
                    PlaceTiles(null, centerCell, size / 2);
                }
                else
                {
                    PlaceTiles(tile.tile, centerCell, size / 2);
                }
                break;
            }
        }
        
        if (Input.GetMouseButton(1))
        {
            var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var centerCell = tilemap.WorldToCell(mouseWorldPos);
            PlaceTiles(null, centerCell, size / 2);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            tilemap.ClearAllTiles();
        }
        
        if (Time.time - _lastUpdateTime > updateInterval)
        {
            UpdateTiles();
            _lastUpdateTime = Time.time;
        }
    }

    private void PlaceTiles(TileBase tile, Vector3Int center, int radius)
    {
        if (brushType == BrushType.Smooth)
        {
            // TODO: 平滑化処理を実装する
        }
        else
        {
            for (var y = -radius; y <= radius; y++)
            {
                for (var x = -radius; x <= radius; x++)
                {
                    var pos = new Vector3Int(center.x + x, center.y + y, center.z);
                    if (brushType == BrushType.Circle)
                    {
                        if (Vector3Int.Distance(center, pos) > radius) { continue; }
                    }
                    if (tilemap.HasTile(pos) && tile != null) { continue; }
                
                    tilemap.SetTile(pos, tile);
                }
            }
        }
    }

    private void UpdateTiles()
    {
        // TODO: 四分木を使って高速化する
        var bounds = tilemap.cellBounds;
        var allTiles = tilemap.GetTilesBlock(bounds);
        for (var y = bounds.yMin; y < bounds.yMax; y++)
        {
            // 左右の行を交互に処理することで、片方ばかりに流れるのを防ぐ
            if (y % 2 == 0)
            {
                for (var x = bounds.xMax - 1; x >= bounds.xMin; x--)
                {
                    Tiles(x, y, bounds, allTiles);
                }
            }
            else
            {
                for (var x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Tiles(x, y, bounds, allTiles);
                }
            }
        }
    }

    private void Tiles(int x, int y, BoundsInt bounds, TileBase[] allTiles)
    {
        foreach (var tile in tiles)
        {
            if (tile.canFall)
            {
                FallTiles(x, y, bounds, allTiles, tile);
            }
            else if (tile.canWater)
            {
                WaterTiles(x, y, bounds, allTiles, tile);
            }
        }
    }

    private void WaterTiles(int x, int y, BoundsInt bounds, TileBase[] allTiles, TileData tile)
    {
        var pos = new Vector3Int(x, y, 0);
        var index = pos.y * bounds.size.x + pos.x - (bounds.position.y * bounds.size.x + bounds.position.x);
        if (allTiles[index] != tile.tile) { return; }

        var right = new Vector3Int(x + 1, y, 0);
        var left = new Vector3Int(x - 1, y, 0);
        var below = new Vector3Int(x, y - 1, 0);
        var belowLeft = new Vector3Int(x - 1, y - 1, 0);
        var belowRight = new Vector3Int(x + 1, y - 1, 0);
        if (y <= -50) { return; }

        if (!tilemap.HasTile(below))
        {
            // 各々即時に落下することで、セルオートマトン特有の間が空く減少を回避
            tilemap.SetTile(below, tile.tile);
            tilemap.SetTile(pos, null);
        }
        else if (!tilemap.HasTile(belowLeft) || !tilemap.HasTile(belowRight))
        {
            // 左右ランダムに流れるようにする
            var random = Random.Range(0, 2);
            if (random == 0)
            {
                if (!tilemap.HasTile(belowLeft))
                {
                    tilemap.SetTile(belowLeft, tile.tile);
                    tilemap.SetTile(pos, null);
                }
            }
            else if (random == 1)
            {
                if (!tilemap.HasTile(belowRight))
                {
                    tilemap.SetTile(belowRight, tile.tile);
                    tilemap.SetTile(pos, null);
                }
            }
        }
        else if (!tilemap.HasTile(left) || !tilemap.HasTile(right))
        {
            var random = Random.Range(0, 2);
            if (random == 0)
            {
                if (!tilemap.HasTile(left))
                {
                    tilemap.SetTile(left, tile.tile);
                    tilemap.SetTile(pos, null);
                }
            }
            else if (random == 1)
            {
                if (!tilemap.HasTile(right))
                {
                    tilemap.SetTile(right, tile.tile);
                    tilemap.SetTile(pos, null);
                }
            }
        }
    }

    private void FallTiles(int x, int y, BoundsInt bounds, TileBase[] allTiles, TileData tile)
    {
        var pos = new Vector3Int(x, y, 0);
        var index = pos.y * bounds.size.x + pos.x - (bounds.position.y * bounds.size.x + bounds.position.x);
        if (allTiles[index] != tile.tile) { return; }

        var below = new Vector3Int(x, y - 1, 0);
        var belowLeft = new Vector3Int(x - 1, y - 1, 0);
        var belowRight = new Vector3Int(x + 1, y - 1, 0);
        if (y <= -50) { return; }

        if (!tilemap.HasTile(below))
        {
            // 各々即時に落下することで、セルオートマトン特有の間が空く減少を回避
            tilemap.SetTile(below, tile.tile);
            tilemap.SetTile(pos, null);
        }
        else
        {
            // 左右ランダムに流れるようにする
            var random = Random.Range(0, 2);
            if (random == 0)
            {
                if (!tilemap.HasTile(belowLeft))
                {
                    tilemap.SetTile(belowLeft, tile.tile);
                    tilemap.SetTile(pos, null);
                }
            }
            else if (random == 1)
            {
                if (!tilemap.HasTile(belowRight))
                {
                    tilemap.SetTile(belowRight, tile.tile);
                    tilemap.SetTile(pos, null);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var tileCenter = tilemap.cellBounds.center;
        var tileBounds = tilemap.cellBounds;
        Gizmos.DrawWireCube(tileCenter, tileBounds.size);
    }
}
