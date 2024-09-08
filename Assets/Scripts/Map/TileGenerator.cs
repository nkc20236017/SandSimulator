using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    [Header("Brush Config")]
    [SerializeField] private TileType tileType;
    [SerializeField] private int radius;
    
	[Header("Tile Config")]
	[SerializeField] private Tilemap tilemap;
    [SerializeField] private TileData[] tiles;
    
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }
    
    private TileData GetTileData(TileType type)
    {
        return (from tile in tiles where tile.type == type select tile).FirstOrDefault();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
            var cellPosition = tilemap.WorldToCell(worldPosition);
            PlaceTiles(GetTileData(tileType).tile, cellPosition, radius);
        }
        
        if (Input.GetMouseButton(1))
        {
            var mousePosition = Input.mousePosition;
            var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
            var cellPosition = tilemap.WorldToCell(worldPosition);
            PlaceTiles(null, cellPosition, radius);
        }
    }

    private void PlaceTiles(TileBase tile, Vector3Int center, int radius)
    {
        if (!IsCameraVisible(center)) { return; }
        
        var bounds = new BoundsInt(center.x - radius, center.y - radius, 0, radius * 2, radius * 2, 1);
        var tileBases = new TileBase[bounds.size.x * bounds.size.y];

        for (var i = 0; i < tileBases.Length; i++)
        {
            tileBases[i] = tile;
        }

        tilemap.SetTilesBlock(bounds, tileBases);
    }
    
    private bool IsCameraVisible(Vector3Int pos)
    {
        return _camera.pixelRect.Contains(_camera.WorldToScreenPoint(tilemap.GetCellCenterWorld(pos)));
    }
}

[Serializable]
public class TileData
{
	public TileType type;
	public TileBase tile;
    [HideInInspector] public List<Vector3Int> tilePositions = new();
}
