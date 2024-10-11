using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct CaveProcedures
    {
        [SerializeField]    // ノイズの大きさ
        private Vector2 scale;
        public Vector2 Scale => scale;
        [SerializeField]    // 塊の閾値
        [Range(0, 1)]
        private float lumpThreshold;
        public float LumpThreshold => lumpThreshold;
        [SerializeField]    // 反転する閾値
        [Range(0, 1)]
        private float hollowThreshold;
        public float HollowThreshold => hollowThreshold;
        [SerializeField]    // 設置されているかに関わらず上書きする
        private bool isOrverride;
        public bool IsOrverride => isOrverride;
        [SerializeField]    // ブロックで埋めるようにする
        private bool isBackfill;
        public bool IsBackfill => isBackfill;
        [SerializeField]    // 適用範囲の反転をする
        private bool isInvert;
        public bool IsInvert => isInvert;
        [SerializeField]    // タイル
        private TileBase backfillTile;
        public TileBase BackfillTile => backfillTile;
    }
}