using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField]
        private Color debugLayerColor;
        public Color DebugLayerColor => debugLayerColor;
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
        public Vector2Int WorldSize => worldScale;
        [SerializeField]    // 1チャンクの大きさ
        private Vector2Int oneChunkSize;
        public Vector2Int OneChunkSize => oneChunkSize;
        [Tooltip("左のほうが深く、右に行くにつれて浅くなる。\n色は考慮しないため自由。色が馴染んでいる部分は地層の変化も緩やかになります")]
        [SerializeField]    // 地層の割合
        [Range(0f, 1f)]
        private float[] layerRatios;
        public float[] LayerRatios => layerRatios;
        [SerializeField]    // ランダム値を生成する時の最大値
        private float randomLimit;
        public float RandomLimit => randomLimit;
        [SerializeField]    // ランダム値の振れ幅
        [Range(0f, 1f)]
        private float amplitude;
        public float Amplitude => amplitude;

        [Space]
        [Header("each layer")]
        [SerializeField]    // それぞれの地層の状態
        private WorldLayer[] worldLayers;
        public WorldLayer[] WorldLayers => worldLayers;
        [SerializeField]    // 地層の境界線の歪み
        private float borderNoiseSize;
        public float BorderNoiseSize => borderNoiseSize;

#if UNITY_EDITOR
        private float[] layerRatiosTemp = new float[0];

        private void OnValidate()
        {
            // 地層の数が一致しない場合に修正する
            if (worldLayers.Length != layerRatios.Length + 1)
            {
                WorldLayer[] worldLayersTemp = new WorldLayer[layerRatios.Length + 1];
                for (int i = 0; i < layerRatios.Length + 1; i++)
                {
                    if (worldLayers.Length <= i) { break; }
                    worldLayersTemp[i] = worldLayers[i];
                }
                worldLayers = worldLayersTemp;
            }

            // 地層の割合を調整する
            /*
             * 割合であるため以降のマジックナンバー「1」は100%を指す
             */
            // 変化していなければ処理を終了
            if (layerRatios.Length == 0)
            {
                layerRatiosTemp = new float[0];
                return;
            }
            if (layerRatiosTemp.Length != 0 && layerRatios.SequenceEqual(layerRatiosTemp))
            {
                return;
            }

            if (layerRatiosTemp.Length != layerRatios.Length)
            {
                layerRatiosTemp = layerRatios.ToArray();
            }

            float ratioTotal = layerRatios.Sum();
            // 合計がほぼ1であれば終了
            if (Mathf.Approximately(1, ratioTotal) == true)
            {
                return;
            }

            // 変化したら変化した場所を取得する
            int changedIndex = -1;
            for (int i = 0; i < layerRatios.Length; i++)
            {
                if (Mathf.Approximately(layerRatios[i], layerRatiosTemp[i]) == false)
                {
                    changedIndex = i;
                    break;
                }
            }
            // 配列の追加による変化だった場合新しく作成された要素を残りの数字にする
            if (changedIndex == -1)
            {
                Debug.Log(ratioTotal);

                float layerRespite = 1 - (ratioTotal - layerRatios[layerRatios.Length - 1]);
                if (0 <= layerRespite && layerRespite <= 1)
                {
                    layerRatios[layerRatios.Length - 1] = layerRespite;
                }
                return;
            }

            // 合計値の平均を取得
            float changedRespite = 1 - layerRatios[changedIndex];
            int ignore = layerRatios.Where(_ => Mathf.Approximately(0, _)).ToArray().Length + 1;
            float otherTotal = 1 - ratioTotal;
            float changedQuantity = otherTotal / (layerRatios.Length - ignore);

            // 1より大きければ他の値を下げる
            for (int i = 0; i < layerRatios.Length; i++)
            {
                // 変更された要素は調節しない
                if (changedIndex == i) { continue; }
                layerRatios[i] += changedQuantity;
                layerRatios[i] = Mathf.Clamp01(layerRatios[i]);
            }

            layerRatiosTemp = layerRatios.ToArray();
        }
#endif
    }
}
