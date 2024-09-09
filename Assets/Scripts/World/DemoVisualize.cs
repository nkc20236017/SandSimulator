using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public struct WorldLayer
{
    [SerializeField]
    private TileBase fillingTile;               // 埋めるパーティクルタイル(空だと削除する)
    [SerializeField]
    private Vector2Int minImpactAreaPosition;   // このレイヤーが影響を与えるエリアの最小座標
    [SerializeField]
    private Vector2Int maxImpactAreaPosition;   // このレイヤーが影響を与えるエリアの最大座標
    [SerializeField]
    private Vector2 seed;                       // 生成するワールドのシード値
    [SerializeField]
    private float frequency;                    // 変形する頻度(値を大きくすると塊が細かくなる)
    [SerializeField, Range(0f, 1f)]
    private float extent;                       // 区切り値(値を大きくすると通路の幅が広くなる)

    public TileBase FillingTile { get => fillingTile; }
    public Vector2Int MinImpactAreaPosition { get => minImpactAreaPosition; }
    public Vector2Int MaxImpactAreaPosition { get => maxImpactAreaPosition; }
    public Vector2 Seed { get => seed; }
    public float Frequency { get => frequency; }
    public float Extent { get => extent; }
}

public class DemoVisualize : MonoBehaviour
{
    [SerializeField]
    private int solo;
    [SerializeField]
    private WorldLayer[] _worldLayers;
    [SerializeField]
    private Tilemap worldTilemap;
    [SerializeField]
    private TileBase tileBase;

    private IWorldGeneratable worldGenerator;

    private void OnValidate()
    {
        worldGenerator = new CaveGenerater(worldTilemap);
    }

    private void Start()
    {
        worldGenerator = new CaveGenerater(worldTilemap);
        worldGenerator.Execute(_worldLayers);
    }

    [ContextMenu("World/Generate")]
    public void Generate()
    {
        WorldLayer[] worldLayers = _worldLayers;
        if (0 <= solo)
        {
            worldLayers = new WorldLayer[] { _worldLayers[solo] };
        }
        Reset();
        worldGenerator.Execute(worldLayers);
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
}
