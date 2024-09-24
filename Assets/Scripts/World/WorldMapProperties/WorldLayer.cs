using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct WorldLayer
    {
        [SerializeField]
        private Color debugLayerColor;
        public Color DebugLayerColor => debugLayerColor;
        [SerializeField]    // ‚±‚Ì’n‘w‚ð\¬‚·‚éÞŽ¿
        private TileBase materialTileID;
        public TileBase MaterialTile => materialTileID;
        [SerializeField]    // ‚±‚Ì’n‘w‚É¶¬‚³‚ê‚ézÎ
        private PrimevalObject[] primevalOres;
        public PrimevalObject[] PrimevalOres => primevalOres;
        [SerializeField]    // ‚±‚Ì’n‘w‚É‘¶Ý‚·‚éƒ_ƒ“ƒWƒ‡ƒ“
        private PrimevalObject[] primevalDungeons;
        public PrimevalObject[] PrimevalDungeons => primevalDungeons;
    }
}