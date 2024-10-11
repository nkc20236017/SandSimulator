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
        [SerializeField]    // ���̒n�w���\������ގ�
        private TileBase materialTile;
        public TileBase MaterialTile => materialTile;
        [SerializeField]    // ���̒n�w�ɐ��������z��
        private PrimevalOre[] primevalOres;
        public PrimevalOre[] PrimevalOres => primevalOres;
        [SerializeField]    // ���̒n�w�ɑ��݂���_���W����
        private PrimevalOre[] primevalDungeons;
        public PrimevalOre[] PrimevalDungeons => primevalDungeons;
    }
}