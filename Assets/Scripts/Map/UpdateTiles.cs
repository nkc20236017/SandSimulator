using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class UpdateTile : MonoBehaviour, IWorldGenerateWaitable
{
    [Header("Datas Config")]
    [SerializeField] private BlockDatas blockDatas;
    [SerializeField] private LayerMask collisionLayerMask;

    [Header("Update Config")]
    [SerializeField] private float updateInterval = 0.01f;
    [SerializeField] private Vector2Int chunkSize = new(260, 150);
    
    [Header("Update Tile Config")]
    [SerializeField] private bool canUpdateSand;
    [SerializeField] private bool canUpdateWater;
    [SerializeField] private bool canUpdateSandToMud;
    
    private float _lastUpdateTime;
    private Transform _player;
    private Tilemap _updateTilemap;
    private List<Vector3Int> _clearTiles = new();
    private List<Vector3Int> _updateTiles = new();
    private IChunkInformation _chunkInformation;
    
    /// <summary>
    /// プレイヤーの設定
    /// プレイヤーを中心に更新する
    /// </summary>
    /// <param name="player">プレイヤー</param>
    public void SetPlayer(Transform player)
    {
        _player = player;
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
        
        if (chunkSize == Vector2Int.zero)
        {
            var tilePositions = _updateTilemap.cellBounds.allPositionsWithin;
            foreach (var position in tilePositions)
            {
                var tilemap = _chunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
                if (tilemap == null)
                {
                    _updateTilemap.SetTile(position, null);
                    continue;
                }
                
                var tile = _updateTilemap.GetTile(position);
                if (tile == null) { continue; }
            
                var index = Array.FindIndex(blockDatas.Block, t => t.tile == tile);
                if (index >= 0 && index < blockDatas.Block.Length)
                {
                    blockDatas.Block[index].tilePositions.Add(position);
                }
            }
        }
        else
        {
            for (var y = -chunkSize.y / 2; y < chunkSize.y / 2; y++)
            {
                for (var x = -chunkSize.x / 2; x < chunkSize.x / 2; x++)
                {
                    var position = new Vector3Int(x, y, 0) + (_player != null ? Vector3Int.RoundToInt(_player.transform.position) : Vector3Int.zero);
                    var tilemap = _chunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
                    var localPosition = _chunkInformation.WorldToChunk(new Vector2(position.x, position.y));
                    // if (tilemap == null || !tilemap.HasTile(localPosition))
                    // {
                    //     _updateTilemap.SetTile(position, null);
                    //     continue;
                    // }
                    
                    var tile = _updateTilemap.GetTile(position);
                    if (tile == null) { continue; }
                    var index = Array.FindIndex(blockDatas.Block, t => t.tile == tile);
                    if (index >= 0 && index < blockDatas.Block.Length)
                    {
                        blockDatas.Block[index].tilePositions.Add(position);
                    }
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
        foreach (var tilePosition in tilePositions)
        {
            // TODO: 地層の色を設定する
            var tileLayer = _chunkInformation.GetLayer(new Vector2(tilePosition.x, tilePosition.y));
            if (tile.GetStratumGeologyData(tileLayer) != null)
            {
                _updateTilemap.SetColor(tilePosition, tile.GetStratumGeologyData(tileLayer).color);
            }
            
            // 動いている砂は当たり判定をなくす
            if (tile.type != BlockType.Sand) { return; }
            
            _updateTilemap.SetColliderType(tilePosition, Tile.ColliderType.None);
        }
    }

    private void UpdateSand(Vector3Int position)
    {
        var pos = new Vector2(position.x, position.y);
        var mapTilemap = _chunkInformation.GetChunkTilemap(pos);
        if (mapTilemap == null) { return; }

        var localPosition = _chunkInformation.WorldToChunk(pos);
        
        var hasTileCount = 0;
        for (var i = -1; i <= 1; i++)
        {
            var checkPosition = position + new Vector3Int(i, -1, 0);
            var tilemap = _chunkInformation.GetChunkTilemap(new Vector2(checkPosition.x, checkPosition.y));
            var checkLocalPosition = _chunkInformation.WorldToChunk(new Vector2(checkPosition.x, checkPosition.y));
            if (tilemap == null || tilemap.HasTile(checkLocalPosition) || IsCollision(checkPosition))
            {
                hasTileCount++;
            }
        }

        if (hasTileCount == 3)
        {
            var block = blockDatas.GetBlock(BlockType.Sand);
            mapTilemap.SetTile(localPosition, block.tile);
            // TODO: 地層の色を設定する
            var tileLayer = _chunkInformation.GetLayer(pos);
            if (block.GetStratumGeologyData(tileLayer) != null)
            {
                mapTilemap.SetColor(localPosition, block.GetStratumGeologyData(tileLayer).color);
            }
            _clearTiles.Add(position);
            return;
        }
        
        var below = position + Vector3Int.down;
        var belowLeft = position + new Vector3Int(-1, -1, 0);
        var belowRight = position + new Vector3Int(1, -1, 0);
        
        var belowTilemap = _chunkInformation.GetChunkTilemap(new Vector2(below.x, below.y));
        var belowLeftTilemap = _chunkInformation.GetChunkTilemap(new Vector2(belowLeft.x, belowLeft.y));
        var belowRightTilemap = _chunkInformation.GetChunkTilemap(new Vector2(belowRight.x, belowRight.y));
        
        if (belowTilemap != null && !CheckHasTile(belowTilemap, below))
        {
            _clearTiles.Add(position);
            _updateTiles.Add(below);
        }
        else if (belowLeftTilemap != null && !belowLeftTilemap.HasTile(belowLeft) && !_updateTilemap.HasTile(belowLeft) || belowRightTilemap != null && !belowRightTilemap.HasTile(belowRight) && !_updateTilemap.HasTile(belowRight))
        {
            var random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    if (belowLeftTilemap != null && !CheckHasTile(belowLeftTilemap, belowLeft))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(belowLeft);
                    }
                    break;
                case 1:
                    if (belowRightTilemap != null && !CheckHasTile(belowRightTilemap, belowRight))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(belowRight);
                    }
                    break;
            }
        }
        
        _updateTilemap.SetColliderType(position, Tile.ColliderType.Sprite);
    }

    private bool CheckHasTile(Tilemap tilemap, Vector3Int position)
    {
        var localPosition = _chunkInformation.WorldToChunk(new Vector2(position.x, position.y));
        return tilemap.HasTile(localPosition) || _updateTilemap.HasTile(position) || CheckUpdateTilePosition(position) || IsCollision(position);
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
        
        var leftTilemap = _chunkInformation.GetChunkTilemap(new Vector2(left.x, left.y));
        var rightTilemap = _chunkInformation.GetChunkTilemap(new Vector2(right.x, right.y));
        var belowTilemap = _chunkInformation.GetChunkTilemap(new Vector2(below.x, below.y));
        var belowLeftTilemap = _chunkInformation.GetChunkTilemap(new Vector2(belowLeft.x, belowLeft.y));
        var belowRightTilemap = _chunkInformation.GetChunkTilemap(new Vector2(belowRight.x, belowRight.y));
        
        if (belowTilemap != null && !CheckHasTile(belowTilemap, below))
        {
            _clearTiles.Add(position);
            _updateTiles.Add(below);
        }
        else if (belowLeftTilemap != null && !_updateTilemap.HasTile(belowLeft) && !belowLeftTilemap.HasTile(belowLeft) || belowRightTilemap != null && !_updateTilemap.HasTile(belowRight) && !belowRightTilemap.HasTile(belowRight))
        {
            var random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    if (belowLeftTilemap != null && !CheckHasTile(belowLeftTilemap, belowLeft))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(belowLeft);
                    }
                    break;
                case 1:
                    if (belowRightTilemap != null && !CheckHasTile(belowRightTilemap, belowRight))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(belowRight);
                    }
                    break;
            }
        }
        else if (leftTilemap != null && !_updateTilemap.HasTile(left) && !leftTilemap.HasTile(left) || rightTilemap != null && !_updateTilemap.HasTile(right) && !rightTilemap.HasTile(right))
        {
            var random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                {
                    if (leftTilemap != null && !CheckHasTile(leftTilemap, left))
                    {
                        _clearTiles.Add(position);
                        _updateTiles.Add(left);
                    }
                    break;
                }
                case 1:
                {
                    if (rightTilemap != null && !CheckHasTile(rightTilemap, right))
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

        var pos = new Vector2(position.x, position.y);
        var mapTilemap = _chunkInformation.GetChunkTilemap(pos);
        if (mapTilemap == null) { return; }
        
        var tileLayer = _chunkInformation.GetLayer(pos);
        if (tilesBlock.Any(tileBase => tileBase == blockDatas.GetBlock(BlockType.Liquid).tile))
        {
            var localPosition = _chunkInformation.WorldToChunk(new Vector2(position.x, position.y));
            
            var block = blockDatas.GetBlock(BlockType.Liquid);
            mapTilemap.SetTile(localPosition, block.tile);
            
            if (block.GetStratumGeologyData(tileLayer) == null) { return; }
            var color = block.GetStratumGeologyData(tileLayer).color;
            mapTilemap.SetColor(localPosition, color);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) { return; }
        
        var color = _updateTilemap.color;
        color.a = 0.5f;
        _updateTilemap.color = color;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) { return; }
        
        var color = _updateTilemap.color;
        color.a = 1;
        _updateTilemap.color = color;
    }

    public void OnGenerated(IChunkInformation worldMapManager)
    {
        _chunkInformation = worldMapManager;
        _updateTilemap = GetComponent<Tilemap>();
        blockDatas.Block.ToList().ForEach(tile => tile.tilePositions ??= new List<Vector3Int>());
    }
}
