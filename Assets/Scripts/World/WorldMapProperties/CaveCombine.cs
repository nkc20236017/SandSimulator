using System;
using UnityEngine;

namespace WorldCreation
{
    [Serializable]
    public struct CaveCombine
    {
        [SerializeField]    // ノイズの大きさ
        private Vector2 scale;
        public Vector2 Scale => scale;
        [SerializeField]    // 塊となる場所の大きさ
        [Range(0, 1)]
        private float lumpSize;
        public float LumpSize => lumpSize;
        [SerializeField]
        [Range(0, 1)]
        private float hollowSize;
        public float HollowSize => hollowSize;
        [SerializeField]    // ブロックで埋めるようにする
        private bool isBackfill;
        public bool IsBackfill => isBackfill;
        [SerializeField]
        private bool isInvert;
        public bool IsInvert => isInvert;
        [SerializeField]    // タイルマップのID
        private int backfillTileID;
        public int BackfillTileID => backfillTileID;
    }
}