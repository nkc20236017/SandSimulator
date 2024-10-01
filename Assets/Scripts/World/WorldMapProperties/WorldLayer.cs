using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct WorldLayer
    {
        [SerializeField]
        private Color layerColor;
        public Color LayerColor => layerColor;
        [SerializeField]    // ‚±‚Ì’n‘w‚ð\¬‚·‚éÞŽ¿
        private TileBase materialTileID;
        public TileBase MaterialTile => materialTileID;
        [SerializeField]    // ‚±‚Ì’n‘w‚É¶¬‚³‚ê‚ézÎ
        private PrimevalOre[] primevalOres;
        public PrimevalOre[] PrimevalOres => primevalOres;
        [SerializeField]    // ‚±‚Ì’n‘w‚É‘¶Ý‚·‚éƒ_ƒ“ƒWƒ‡ƒ“
        private PrimevalOre[] primevalDungeons;
        public PrimevalOre[] PrimevalDungeons => primevalDungeons;
    }
}