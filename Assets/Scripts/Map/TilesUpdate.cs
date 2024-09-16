using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilesUpdate : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Tilemap _updateTilemap;
    [SerializeField] private Tilemap _mapTilemap;
    [SerializeField] private BlockDatas blockDatas;
    [SerializeField] private float updateInterval = 0.01f;
    [SerializeField] private Vector2Int chunkSize = new(84, 45);
    [SerializeField] private LayerMask collisionLayerMask;
    
    [Header("Update Tile Config")]
    [SerializeField] private bool canUpdateSand;
    [SerializeField] private bool canUpdateWater;
    [SerializeField] private bool canUpdateSandToMud;
    
    private float _lastUpdateTime;
    private List<Vector3Int> _clearTiles = new();
    private List<Vector3Int> _updateTiles = new();
    
    private Dictionary<Vector3Int, TileBase> _previousTilemapState = new();
    
    private void Start()
    {
        blockDatas.Block.ToList().ForEach(tile => tile.tilePositions ??= new List<Vector3Int>());
    }

    private void Update()
    {
        if (Time.time - _lastUpdateTime <= updateInterval) { return; }
        if (_updateTilemap.GetUsedTilesCount() == 0) { return; }
        
        GetTilePosition();
        _lastUpdateTime = Time.time;
    }

    private void GetTilePosition()
    {
        foreach (var tile in blockDatas.Block.Where(tile => tile.tilePositions.Count > 0))
        {
            tile.tilePositions.Clear();
        }

        for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
        {
            for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
            {
                var position = new Vector3Int(x, y, 0);
                var tile = _updateTilemap.GetTile(position);
                if (tile == null) { continue; }
            
                var index = Array.FindIndex(blockDatas.Block, t => t.tile == tile);
                if (index >= 0 && index < blockDatas.Block.Length)
                {
                    blockDatas.Block[index].tilePositions.Add(position);
                }
            }
        }
        if (blockDatas.Block.All(tile => tile.tilePositions.Count == 0)) { return; }
        
        CheckUpdateTiles();
    }

    private void CheckUpdateTiles()
    {
        foreach (var tileData in blockDatas.Block.Where(tile => tile.tilePositions.Count > 0))
        {
            _clearTiles.Clear();
            _updateTiles.Clear();
            
            var randomTilePositions = tileData.tilePositions.OrderBy(_ => Guid.NewGuid()).ToList();
            foreach (var position in randomTilePositions)
            {
                switch (tileData.type)
                {
                    case BlockType.Sand:
                        if (canUpdateSand)
                        {
                            UpdateSand(position);
                        }
                        if (canUpdateSandToMud)
                        {
                            SandToMud(position);
                        }
                        break;
                    case BlockType.Liquid:
                        if (canUpdateWater)
                        {
                            UpdateWater(position);
                        }
                        break;
                }
            }
            
            UpdateTiles(tileData);
        }
    }

    private void UpdateTiles(Block tile)
    {
        if (_clearTiles.Count == 0 && _updateTiles.Count == 0) { return; }
        
        var clearTiles = _clearTiles.ToArray();
        var updateTiles = _updateTiles.ToArray();
        var tilePositions = new Vector3Int[clearTiles.Length + updateTiles.Length];
        var tileArray = new TileBase[clearTiles.Length + updateTiles.Length];
        for (var i = 0; i < clearTiles.Length; i++)
        {
            tilePositions[i] = clearTiles[i];
            tileArray[i] = null;
        }
        for (var i = 0; i < updateTiles.Length; i++)
        {
            tilePositions[i + clearTiles.Length] = updateTiles[i];
            tileArray[i + clearTiles.Length] = tile.tile;
        }
        
        _updateTilemap.SetTiles(tilePositions, tileArray);
    }

    private void UpdateSand(Vector3Int position)
    {
        var checkBound = new BoundsInt(position.x - 1, position.y - 1, 0, 3, 1, 1);
        var tilesBlock1 = _updateTilemap.GetTilesBlock(checkBound);
        var tilesBlock2 = _mapTilemap.GetTilesBlock(checkBound);
        var tilesBlock = tilesBlock1.Concat(tilesBlock2).ToArray();
        tilesBlock = tilesBlock.Where(tileBase => tileBase != null).ToArray();
        if (tilesBlock.Length == 3 || Physics2D.OverlapBox(_updateTilemap.GetCellCenterWorld(position + Vector3Int.down), new Vector2(2.9f, 0.9f), 0, collisionLayerMask))
        {
            _mapTilemap.SetTile(position, blockDatas.GetBlock(BlockType.Sand).tile); // TODO: プレイヤーの層の砂を取得する
            _clearTiles.Add(position);
            return;
        }
        
        var below = position + Vector3Int.down;
        var belowLeft = position + new Vector3Int(-1, -1, 0);
        var belowRight = position + new Vector3Int(1, -1, 0);
        
        if (!CheckHasTile(below))
        {
            _clearTiles.Add(position);
            _updateTiles.Add(below);
        }
        else if (!_mapTilemap.HasTile(belowLeft) && !_updateTilemap.HasTile(belowLeft) || !_mapTilemap.HasTile(belowRight) && !_updateTilemap.HasTile(belowRight))
        {
            var random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    if (!CheckHasTile(belowLeft))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(belowLeft);
                    }
                    break;
                case 1:
                    if (!CheckHasTile(belowRight))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(belowRight);
                    }
                    break;
            }
        }
    }

    private bool CheckHasTile(Vector3Int position)
    {
        return _mapTilemap.HasTile(position) || _updateTilemap.HasTile(position) || CheckUpdateTilePosition(position) || IsCollision(position);
    }
    
    private bool IsCollision(Vector3Int position)
    {
        var tilePosition = _updateTilemap.GetCellCenterWorld(position);
        var hit = Physics2D.OverlapBox(tilePosition, new Vector2(0.9f, 0.9f), 0, collisionLayerMask);
        return hit != null;
    }

    private void UpdateWater(Vector3Int position)
    {
        var checkBound = new BoundsInt(position.x - 1, position.y - 1, 0, 3, 3, 1);
        var tilesBlock = _updateTilemap.GetTilesBlock(checkBound);
        tilesBlock = tilesBlock.Where(tileBase => tileBase != null).ToArray();
        if (tilesBlock.Length == 9) { return; }
        
        var left = position + Vector3Int.left;
        var right = position + Vector3Int.right;
        var below = position + Vector3Int.down;
        var belowLeft = position + new Vector3Int(-1, -1, 0);
        var belowRight = position + new Vector3Int(1, -1, 0);
        
        if (!_updateTilemap.HasTile(below) && !_mapTilemap.HasTile(below) && !CheckUpdateTilePosition(below))
        {
            _clearTiles.Add(position);
            _updateTiles.Add(below);
        }
        else if (!_updateTilemap.HasTile(belowLeft) && !_mapTilemap.HasTile(belowLeft) || !_updateTilemap.HasTile(belowRight) && !_mapTilemap.HasTile(belowRight))
        {
            var random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    if (!_updateTilemap.HasTile(belowLeft) && !_mapTilemap.HasTile(belowLeft) && !CheckUpdateTilePosition(belowLeft))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(belowLeft);
                    }
                    break;
                case 1:
                    if (!_updateTilemap.HasTile(belowRight) && !_mapTilemap.HasTile(belowRight) && !CheckUpdateTilePosition(belowRight))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(belowRight);
                    }
                    break;
            }
        }
        else if (!_updateTilemap.HasTile(left) && !_mapTilemap.HasTile(left) || !_updateTilemap.HasTile(right) && !_mapTilemap.HasTile(right))
        {
            var random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                {
                    if (!_updateTilemap.HasTile(left) && !_mapTilemap.HasTile(left) && !CheckUpdateTilePosition(left))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(left);
                    }
                    break;
                }
                case 1:
                {
                    if (!_updateTilemap.HasTile(right) && !_mapTilemap.HasTile(right) && !CheckUpdateTilePosition(right))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(right);
                    }
                    break;
                }
            }
        }
    }
    
    private bool CheckUpdateTilePosition(Vector3Int position)
    {
        return _updateTiles.Any(updateTile => updateTile == position) || _clearTiles.Any(clearTile => clearTile == position);
    }

    private void SandToMud(Vector3Int position)
    {
        if (blockDatas.GetBlock(BlockType.Liquid).tilePositions.Count == 0) { return; }
        if (blockDatas.GetBlock(BlockType.Sand).tilePositions.Count == 0) { return; }
        
        var checkBound = new BoundsInt(position.x - 1, position.y - 1, 0, 3, 3, 1);
        var tilesBlock = _updateTilemap.GetTilesBlock(checkBound);
        tilesBlock = tilesBlock.Where(tileBase => tileBase != null).ToArray();
        if (tilesBlock.Length == 0) { return; }

        if (tilesBlock.Any(tileBase => tileBase == blockDatas.GetBlock(BlockType.Liquid).tile))
        {
            _mapTilemap.SetTile(position, blockDatas.GetBlock(BlockType.Mud).tile);
        }
    }
}
