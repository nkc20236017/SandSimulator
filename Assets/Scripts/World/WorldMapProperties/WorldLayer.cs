using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct WorldLayer
    {
        [SerializeField]    // ’n‘w‚ÌF
        private Color layerColor;
        public Color LayerColor => layerColor;
        [SerializeField]    // ’n‘w‚ð\¬‚·‚éÞŽ¿
        private TileBase materialTile;
        public TileBase MaterialTile => materialTile;
        [SerializeField]    // zÎ‚Ì¶¬î•ñ
        private OreDecisionData oreDecision;
        public OreDecisionData OreDecision => oreDecision;
        [SerializeField]    // ‚±‚Ì’n‘w‚É‘¶Ý‚·‚éƒ_ƒ“ƒWƒ‡ƒ“
        private PrimevalOre[] primevalDungeons;
        public PrimevalOre[] PrimevalDungeons => primevalDungeons;
    }
}