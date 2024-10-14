using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct PrimevalOre
    {
        [SerializeField]    // 地上に生成される場合のゲームオブジェクト
        private Ore exposedOreData;
        public Ore ExposedOreData => exposedOreData;
        [SerializeField]    // 地中に生成される場合のタイルベース
        private TileBase buriedOre;
        public TileBase BuriedOre => buriedOre;
        [SerializeField]    // 塊の最小半径
        private int oreMinRadius;
        public int OreMinRadius => oreMinRadius;
        [SerializeField]    // 塊の最大半径
        private int oreMaxRadius;
        public int OreMaxRadius => oreMaxRadius;
        [SerializeField]    // 地中鉱石の欠け具合
        [Range(0f, 100f)]
        private float chipped;
        public float Chipped => chipped;
        [SerializeField]    // 生成される確率
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
    }
}