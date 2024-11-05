using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class FallingSandSimulation : MonoBehaviour
{
    [FormerlySerializedAs("blockDatas")]
    [Header("Datas Config")]
    [SerializeField] private BlockData _blockData;

    [Header("Update Config")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float updateInterval = 0.01f;
    [SerializeField] private Vector2Int chunkSize = new(260, 150);
    
    [Header("Update Tile Config")]
    [SerializeField] private bool canUpdateSand;
    [SerializeField] private bool canUpdateWater;
    [SerializeField] private bool isFallingSandAlgorithm;
    
    private float _lastUpdateTime;
    private List<Vector3Int> _clearTiles = new();
    private List<Vector3Int> _updateTiles = new();
    
    private void Update()
    {
        if (Time.time - _lastUpdateTime <= updateInterval) { return; }
        if (tilemap.GetUsedTilesCount() == 0) { return; }
        
        GetTilePosition();
        _lastUpdateTime = Time.time;
    }

    private void GetTilePosition()
    {
        foreach (var tile in _blockData.Block.Where(tile => tile.tilePositions.Count > 0))
        {
            tile.tilePositions.Clear();
        }
        
        for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
        {
            for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
            {
                var position = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(position);
                if (tile == null) { continue; }
                
                var index = Array.FindIndex(_blockData.Block, t => t.tile == tile);
                if (index >= 0 && index < _blockData.Block.Length)
                {
                    _blockData.Block[index].tilePositions.Add(position);
                }
            }
        }
        if (_blockData.Block.All(tile => tile.tilePositions.Count == 0)) { return; }
        
        CheckUpdateTiles();
    }

    private void CheckUpdateTiles()
    {
        foreach (var tileData in _blockData.Block.Where(tile => tile.tilePositions.Count > 0))
        {
            _clearTiles.Clear();
            _updateTiles.Clear();
            if (isFallingSandAlgorithm)
            {
                foreach (var position in tileData.tilePositions)
                {
                    switch (tileData.type)
                    {
                        case BlockType.Sand:
                            if (canUpdateSand)
                            {
                                UpdateSand(position);
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
                continue;
            }

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

    private void UpdateSand(Vector3Int position)
    {
        var checkBound = new BoundsInt(position.x - 1, position.y - 1, 0, 3, 1, 1);
        var tilesBlock = tilemap.GetTilesBlock(checkBound);
        tilesBlock = tilesBlock.Where(tileBase => tileBase != null).ToArray();
        if (tilesBlock.Length == 3) { return; }
        
        var below = position + Vector3Int.down;
        var belowLeft = position + new Vector3Int(-1, -1, 0);
        var belowRight = position + new Vector3Int(1, -1, 0);

        if (isFallingSandAlgorithm)
        {
            var tile = tilemap.GetTile(position);
            if (!tilemap.HasTile(below))
            {
                tilemap.SetTile(below, tile);
                tilemap.SetTile(position, null);
            }
            else if (!tilemap.HasTile(belowLeft))
            {
                tilemap.SetTile(belowLeft, tile);
                tilemap.SetTile(position, null);
            }
            else if (!tilemap.HasTile(belowRight))
            {
                tilemap.SetTile(belowRight, tile);
                tilemap.SetTile(position, null);
            }
            
            return;
        }
        
        if (!CheckHasTile(below))
        {
            _clearTiles.Add(position);
            _updateTiles.Add(below);
        }
        else if (!tilemap.HasTile(belowLeft)  || !tilemap.HasTile(belowRight))
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
        return tilemap.HasTile(position) || CheckUpdateTilePosition(position);
    }

    private void UpdateWater(Vector3Int position)
    {
        var checkBound = new BoundsInt(position.x - 1, position.y - 1, 0, 3, 2, 1);
        var tilesBlock = tilemap.GetTilesBlock(checkBound);
        tilesBlock = tilesBlock.Where(tileBase => tileBase != null).ToArray();
        if (tilesBlock.Length == 6) { return; }
        
        var left = position + Vector3Int.left;
        var right = position + Vector3Int.right;
        var below = position + Vector3Int.down;
        var belowLeft = position + new Vector3Int(-1, -1, 0);
        var belowRight = position + new Vector3Int(1, -1, 0);
        
        if (!CheckHasTile(below))
        {
            _clearTiles.Add(position);
            _updateTiles.Add(below);
        }
        else if (!tilemap.HasTile(left) || !tilemap.HasTile(right))
        {
            var random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                {
                    if (!CheckHasTile(left))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(left);
                    }
                    break;
                }
                case 1:
                {
                    if (!CheckHasTile(right))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(right);
                    }
                    break;
                }
            }
        }
        else if (!tilemap.HasTile(belowLeft)  || !tilemap.HasTile(belowRight))
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
    
    private bool CheckUpdateTilePosition(Vector3Int position)
    {
        return _updateTiles.Any(updateTile => updateTile == position) || _clearTiles.Any(clearTile => clearTile == position);
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
        
        tilemap.SetTiles(tilePositions, tileArray);
    }
}