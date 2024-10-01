using System;
using UnityEngine;

namespace WorldCreation
{
    [Serializable]
    public struct PrimevalOre
    {
        [SerializeField]    // 地上に見えている鉱石のゲームオブジェクト
        private Ore exposedOreData;
        public Ore ExposedOreData => exposedOreData;
        [SerializeField]    // 生成するオブジェクト
        private int buriedOreID;
        public int BuriedOreID => buriedOreID;
        [SerializeField]    // 塊の生成量
        private int lumpAmount;
        public int LumpAmount => lumpAmount;
        [SerializeField]    // ひとつの塊に存在する量
        private int blockAmount;
        public int BlockAmount => blockAmount;
        [SerializeField]    // 生成される確率
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
        [SerializeField]    // 鉱石を生成する間隔
        private int space;
        public int Space => space;
    }
}