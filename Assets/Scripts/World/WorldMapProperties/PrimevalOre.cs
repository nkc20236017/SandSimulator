using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct PrimevalOre
    {
        [SerializeField]    // 生成される確率
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
        [Space]
        [SerializeField]    // 地上に生成される場合のゲームオブジェクト
        private Ore exposedOreData;
        public Ore ExposedOreData => exposedOreData;
        [SerializeField]
        private float distance;
        public float Distance => distance;
        [SerializeField]
        private LayerMask groundLayerMask;
        public LayerMask GroundLayerMask => groundLayerMask;
        [Space]
        [SerializeField]    // 地中に生成される場合のタイルベース
        private TileBase buriedOre;
        public TileBase BuriedOre => buriedOre;
        [SerializeField]    // 塊の最小半径
        private int minRadius;
        public int MinRadius => minRadius;
        [SerializeField]    // 塊の最大半径
        private int maxRadius;
        public int MaxRadius => maxRadius;
        [SerializeField]    // 地中鉱石の欠け具合
        [Range(0f, 100f)]
        private float chipped;
        public float Chipped => chipped;
    }
}