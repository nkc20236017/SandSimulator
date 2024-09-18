using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

public class DemoVisualize : MonoBehaviour
{
    [SerializeField]
    private int solo;
    [SerializeField]
    private CaveLayer[] _worldLayers;
    [SerializeField]
    private Tilemap worldTilemap;
    [SerializeField]
    private TileBase tileBase;
    [SerializeField]
    private Terrain terrain;
    private TerrainData terrainData;

    private CaveGenerater worldGenerator;

    private void OnValidate()
    {
        worldGenerator = new CaveGenerater(worldTilemap);
    }

    private void Start()
    {
        worldGenerator = new CaveGenerater(worldTilemap);
        // worldGenerator.Execute(_worldLayers);
    }

    [ContextMenu("World/GenerateAll")]
    public void Generate()
    {
        CaveLayer[] worldLayers = _worldLayers;
        terrainData = terrain.terrainData;
        if (0 <= solo)
        {
            worldLayers = new CaveLayer[] { _worldLayers[solo] };
        }

        terrainData.heightmapResolution = worldLayers[0].MaxImpactAreaPosition.x;

        Reset();
        worldGenerator.Execute(worldLayers, terrainData);
    }

    [ContextMenu("World/Reset")]
    public void Reset()
    {
        // 開始座標と終了座標を明示的に宣言
        Vector2Int StartPosition = _worldLayers[0].MinImpactAreaPosition;
        Vector2Int EndPosition = _worldLayers[0].MinImpactAreaPosition + _worldLayers[0].MaxImpactAreaPosition;

        for (int x = StartPosition.x; x < EndPosition.x; x++)
        {
            for (int y = StartPosition.y; y < EndPosition.y; y++)
            {
                worldTilemap.SetTile(new Vector3Int(x, y), tileBase);
            }
        }
    }

    [ContextMenu("World/Clear")]
    public void Clear()
    {
        // 開始座標と終了座標を明示的に宣言
        Vector2Int StartPosition = _worldLayers[0].MinImpactAreaPosition;
        Vector2Int EndPosition = _worldLayers[0].MinImpactAreaPosition + _worldLayers[0].MaxImpactAreaPosition;

        for (int x = StartPosition.x; x < EndPosition.x; x++)
        {
            for (int y = StartPosition.y; y < EndPosition.y; y++)
            {
                worldTilemap.SetTile(new Vector3Int(x, y), null);
            }
        }
    }
}
