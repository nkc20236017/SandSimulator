using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public class TilesUpdate : Singleton<TilesUpdate>
{
    // [Header("Config")]
    // [SerializeField] private Tilemap tilemap;
    // [SerializeField] private TileData[] tiles;
    // [SerializeField] private float updateInterval = 0.5f;
    // [SerializeField] private Absorption _absorption;
    //
    // private float _lastUpdateTime;
    // private List<Vector3Int> _clearTiles = new();
    // private List<Vector3Int> _updateTiles = new();
    // private Camera _camera;
    //
    // public TileData[] TileDatas => tiles;
    //
    // private void Start()
    // {
	   //  _camera = Camera.main;
    //
    //     foreach (var tile in tiles.Where(tile => tile.tilePositions == null))
    //     {
    //         tile.tilePositions = new List<Vector3Int>();
    //     }
    // }
    //
    // private void Update()
    // {
    //     if (tilemap.GetUsedTilesCount() == 0) { return; }
    //     
    //     if (Time.time - _lastUpdateTime <= updateInterval) { return; }
    //     
    //     GetTilePosition();
    //     CheckUpdateTiles();
    //     _lastUpdateTime = Time.time;
    // }
    //
    // private void GetTilePosition()
    // {
    //     foreach (var tile in tiles.Where(tile => tile.tilePositions.Count > 0))
    //     {
    //         tile.tilePositions.Clear();
    //     }
    //     
    //     // TODO: boundsをもっと軽量化したい
    //     var bounds = tilemap.cellBounds.allPositionsWithin;
    //     foreach (var position in bounds)
    //     {
    //         if (!IsCameraVisible(position)) { continue; }
    //         
    //         var tile = tilemap.GetTile(position);
    //         var index = Array.FindIndex(tiles, t => t.tile == tile);
    //         if (index >= 0 && index < tiles.Length)
    //         {
    //             tiles[index].tilePositions.Add(position);
    //         }
    //     }
    // }
    //
    // private void CheckUpdateTiles()
    // {
    //     foreach (var tileData in tiles.Where(tile => tile.tilePositions.Count > 0))
    //     {
    //         _clearTiles.Clear();
    //         _updateTiles.Clear();
    //         
    //         var randomTilePositions = tileData.tilePositions.OrderBy(_ => Guid.NewGuid()).ToList();
    //         foreach (var position in randomTilePositions)
    //         {
    //             switch (tileData.type)
    //             {
    //                 case TileType.Sand:
    //                     UpdateSand(position);
    //                     SandToMud(position, tileData);
    //                     break;
    //                 case TileType.Mud:
    //                     break;
    //                 case TileType.Water:
    //                     UpdateWater(position);
    //                     break;
    //                 case TileType.Fire:
    //                     break;
    //                 case TileType.Smoke:
    //                     break;
    //                 default:
    //                     throw new ArgumentOutOfRangeException();
    //             }
    //         }
    //         
    //         UpdateTiles(tileData);
    //     }
    // }
    //
    // private void UpdateTiles(TileData tileData)
    // {
    //     if (_clearTiles.Count == 0 && _updateTiles.Count == 0) { return; }
    //     
    //     foreach (var position in _clearTiles)
    //     {
    //         tilemap.SetTile(position, null);
    //     }
    //
    //     foreach (var position in _updateTiles)
    //     {
    //         tilemap.SetTile(position, tileData.tile);
    //         // TODO: 動いているタイルの当たり判定を消す
    //         tilemap.SetColliderType(position, Tile.ColliderType.None);
    //     }
    // }
    //
    // private void UpdateSand(Vector3Int position)
    // {
    //     var checkBound = new BoundsInt(position.x - 1, position.y - 1, 0, 3, 3, 1);
    //     var tilesBlock = tilemap.GetTilesBlock(checkBound);
    //     tilesBlock = tilesBlock.Where(tileBase => tileBase).ToArray();
    //     if (tilesBlock.Length == 9)
    //     {
    //         tilemap.SetColliderType(position, Tile.ColliderType.Sprite);
    //         return;
    //     }
    //     if (!IsCameraVisible(position)) { return; }
    //     
    //     var below = position + Vector3Int.down;
    //     var belowLeft = position + new Vector3Int(-1, -1, 0);
    //     var belowRight = position + new Vector3Int(1, -1, 0);
    //     if (tilemap.HasTile(below) && tilemap.HasTile(belowLeft) && tilemap.HasTile(belowRight))
    //     {
    //         tilemap.SetColliderType(position, Tile.ColliderType.Sprite);
    //         return;
    //     }
    //     
    //     if (!tilemap.HasTile(below) && !CheckUpdateTilePosition(below) && IsCameraVisible(below))
    //     {
    //         _clearTiles.Add(position);
    //         _updateTiles.Add(below);
    //     }
    //     else if (!tilemap.HasTile(belowLeft) || !tilemap.HasTile(belowRight))
    //     {
    //         var random = Random.Range(0, 2);
    //         switch (random)
    //         {
    //             case 0:
    //                 if (!tilemap.HasTile(belowLeft) && !CheckUpdateTilePosition(belowLeft) && IsCameraVisible(belowLeft))
    //                 {
    //                     _clearTiles.Add(position);
    //                     _updateTiles.Add(belowLeft);
    //                 }
    //                 break;
    //             case 1:
    //                 if (!tilemap.HasTile(belowRight) && !CheckUpdateTilePosition(belowRight) && IsCameraVisible(belowRight))
    //                 {
    //                     _clearTiles.Add(position);
    //                     _updateTiles.Add(belowRight);
    //                 }
    //                 break;
    //         }
    //     }
    // }
    //
    // private void UpdateWater(Vector3Int position)
    // {
    //     var checkBound = new BoundsInt(position.x - 1, position.y - 1, 0, 3, 3, 1);
    //     var tilesBlock = tilemap.GetTilesBlock(checkBound);
    //     tilesBlock = tilesBlock.Where(tileBase => tileBase != null).ToArray();
    //     if (tilesBlock.Length == 9) { return; }
    //     if (!IsCameraVisible(position)) { return; }
    //     
    //     var left = position + Vector3Int.left;
    //     var right = position + Vector3Int.right;
    //     var below = position + Vector3Int.down;
    //     var belowLeft = position + new Vector3Int(-1, -1, 0);
    //     var belowRight = position + new Vector3Int(1, -1, 0);
    //     
    //     if (!tilemap.HasTile(below) && !CheckUpdateTilePosition(below) && IsCameraVisible(below))
    //     {
    //         _clearTiles.Add(position);
    //         _updateTiles.Add(below);
    //     }
    //     else if (!tilemap.HasTile(belowLeft) || !tilemap.HasTile(belowRight))
    //     {
    //         var random = Random.Range(0, 2);
    //         switch (random)
    //         {
    //             case 0:
    //                 if (!tilemap.HasTile(belowLeft) && !CheckUpdateTilePosition(belowLeft) && IsCameraVisible(belowLeft))
    //                 {
    //                     _clearTiles.Add(position);
    //                     _updateTiles.Add(belowLeft);
    //                 }
    //                 break;
    //             case 1:
    //                 if (!tilemap.HasTile(belowRight) && !CheckUpdateTilePosition(belowRight) && IsCameraVisible(belowRight))
    //                 {
    //                     _clearTiles.Add(position);
    //                     _updateTiles.Add(belowRight);
    //                 }
    //                 break;
    //         }
    //     }
    //     else if (!tilemap.HasTile(left) || !tilemap.HasTile(right))
    //     {
    //         var random = Random.Range(0, 2);
    //         switch (random)
    //         {
    //             case 0:
    //             {
    //                 if (!tilemap.HasTile(left) && !CheckUpdateTilePosition(left) && IsCameraVisible(left))
    //                 {
    //                     _clearTiles.Add(position);
    //                     _updateTiles.Add(left);
    //                 }
    //                 break;
    //             }
    //             case 1:
    //             {
    //                 if (!tilemap.HasTile(right) && !CheckUpdateTilePosition(right) && IsCameraVisible(right))
    //                 {
    //                     _clearTiles.Add(position);
    //                     _updateTiles.Add(right);
    //                 }
    //                 break;
    //             }
    //         }
    //     }
    // }
    //
    // private bool IsCameraVisible(Vector3Int pos)
    // {
    //     var cameraPosition = _camera.WorldToScreenPoint(tilemap.GetCellCenterWorld(pos));
    //     return _camera.pixelRect.Contains(cameraPosition);
    // }
    //
    // private bool CheckUpdateTilePosition(Vector3Int position)
    // {
    //     return _updateTiles.Any(updateTile => updateTile == position) || _clearTiles.Any(clearTile => clearTile == position);
    // }
    //
    // private void SandToMud(Vector3Int position, TileData tileData)
    // {
    //     if (tileData.type != TileType.Sand) { return; }
    //     if (GetTileData(TileType.Water).tilePositions.Count == 0) { return; }
    //     if (GetTileData(TileType.Sand).tilePositions.Count == 0) { return; }
    //         
    //     var checkBound = new BoundsInt(position.x - 1, position.y - 1, 0, 3, 3, 1);
    //     var tilesBlock = tilemap.GetTilesBlock(checkBound);
    //     tilesBlock = tilesBlock.Where(tileBase => tileBase).ToArray();
    //     if (tilesBlock.Length == 0) { return; }
    //
    //     if (tilesBlock.Any(tileBase => tileBase == GetTileData(TileType.Water).tile))
    //     {
    //         tilemap.SetTile(position, GetTileData(TileType.Mud).tile);
    //     }
    // }
    //
    // public TileData GetTileData(TileType type)
    // {
    //     return (from tile in tiles where tile.type == type select tile).FirstOrDefault();
    // }
}
