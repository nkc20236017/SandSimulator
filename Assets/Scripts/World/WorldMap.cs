using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    public enum MarginShape
    {
        Circle,
        Box
    }

    [Serializable]
    public struct CaveLayer
    {
        [SerializeField]    // 埋めるパーティクルタイル(空だと削除する)
        private TileBase fillingTile;
        [SerializeField]    // このレイヤーが影響を与えるエリアの最小座標
        private Vector2Int minImpactAreaPosition;
        [SerializeField]    // このレイヤーが影響を与えるエリアの最大座標
        private Vector2Int maxImpactAreaPosition;
        [SerializeField]    // 生成するワールドのシード値
        private Vector2 seed;
        [SerializeField]    // 変形する頻度(値を大きくすると塊が細かくなる)
        private float frequency;
        [SerializeField]    // 区切り値(値を大きくすると通路の幅が広くなる)
        [Range(0, 1)]
        private float extent;

        public TileBase FillingTile => fillingTile;
        public Vector2Int MinImpactAreaPosition => minImpactAreaPosition;
        public Vector2Int MaxImpactAreaPosition => maxImpactAreaPosition;
        public Vector2 Seed => seed;
        public float Frequency => frequency;
        public float Extent => extent;
    }

    [Serializable]
    public struct WorldLayer
    {
        [SerializeField]    // この地層を構成する材質
        private TileBase[] materialTiles;
        public TileBase[] MaterialTiles => materialTiles;
        [SerializeField]    // この地層に生成される鉱石
        private PrimevalObject[] primevalOres;
        public PrimevalObject[] PrimevalOres => primevalOres;
        [SerializeField]    // この地層に存在する敵
        private PrimevalObject[] primevalEnemies;
        public PrimevalObject[] PrimevalEnemies => primevalEnemies;
        [SerializeField]    // この地層に存在するダンジョン
        private PrimevalObject[] primevalDungeons;
        public PrimevalObject[] PrimevalDungeons => primevalDungeons;
    }

    [Serializable]
    public struct PrimevalObject
    {
        [SerializeField]    // 生成する優先度
        private float priority;
        public float Priority => priority;
        [SerializeField]    // 生成するオブジェクト
        private GameObject modelObject;
        public GameObject ModelObject => modelObject;
        [SerializeField]    // 生成する最小値
        private float minExistence;
        public float MinExistence => minExistence;
        [SerializeField]    // 生成する最大値
        private float maxExistence;
        public float MaxExistence => maxExistence;
        [SerializeField]    // 生成される確率
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
        [SerializeField]    // 生成余白の形状
        private MarginShape marginShape;
        public MarginShape MarginShape => marginShape;
        [SerializeField]    // 生成余白の形状がcircleの場合は半径、boxの場合は幅
        private float marginSizeWidth;
        [SerializeField]    // 生成余白の形状がboxの場合の高さ
        private float marginSizeHeight;
        public float MarginSizeHeight
        {
            set => marginSizeHeight = value;
        }

        public float MarginRadius => marginSizeWidth;
        public Vector2 MarginSize => new Vector2(marginSizeWidth, marginSizeHeight);
    }

    [CreateAssetMenu(fileName = "New world map", menuName = "Config/WorldMap")]
    public sealed class WorldMap : ScriptableObject
    {
        [Header("standard worldwide")]
        [SerializeField]    // 世界の最大サイズ
        private Vector2Int worldScale;
        // 分割の実装となった場合に使用
        /*[Header("分割数"), SerializeField]
        private Vector2Int chunk;*/
        [Tooltip("左のほうが深く、右に行くにつれて浅くなる。\n色は考慮しないため自由。色が馴染んでいる部分は地層の変化も緩やかになります")]
        [SerializeField]
        private Gradient layerGradient;
        [Space]
        [Header("each layer")]
        [SerializeField]
        private WorldLayer[] worldLayers;

#if UNITY_EDITOR
        private void OnValidate()
        {
            int layerNumber = layerGradient.colorKeys.Length / 2;
            if (worldLayers.Length != layerNumber)
            {
                WorldLayer[] worldLayersTemp = new WorldLayer[layerNumber];
                for (int i = 0; i < layerNumber; i++)
                {
                    if (worldLayers.Length <= i) { break; }
                    worldLayersTemp[i] = worldLayers[i];
                }
                worldLayers = worldLayersTemp;
            }
        }
        private void SetMarginSizeHeight(PrimevalObject primevalObject)
        {
            if (primevalObject.MarginShape == MarginShape.Circle)
            {
                primevalObject.MarginSizeHeight = 0;
            }
        }
#endif
    }
}
