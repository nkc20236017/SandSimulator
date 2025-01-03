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
        [SerializeField]    // この地層を構成する材質
        private TileBase materialTileID;
        public TileBase MaterialTile => materialTileID;
        [SerializeField]    // この地層に生成される鉱石
        private PrimevalOre[] primevalOres;
        public PrimevalOre[] PrimevalOres => primevalOres;
        [SerializeField]    // この地層に存在するダンジョン
        private PrimevalOre[] primevalDungeons;
        public PrimevalOre[] PrimevalDungeons => primevalDungeons;
    }
}