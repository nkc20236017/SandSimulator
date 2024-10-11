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
        [SerializeField]    // 塊となる場所の大きさ
        [Range(0, 1)]
        private float lumpSize;
        public float LumpSize => lumpSize;
        [SerializeField]    // 
        [Range(0, 1)]
        private float hollowSize;
        public float HollowSize => hollowSize;
        [SerializeField]    // 設置されているかに関わらず上書きする
        private bool isOrverride;
        public bool IsOrverride => isOrverride;
        [SerializeField]    // ブロックで埋めるようにする
        private bool isBackfill;
        public bool IsBackfill => isBackfill;
        [SerializeField]
        private bool isInvert;
        public bool IsInvert => isInvert;
        [SerializeField]    // タイルマップのID
        private TileBase backfillTileID;
        public TileBase BackfillTileID => backfillTileID;
    }
}