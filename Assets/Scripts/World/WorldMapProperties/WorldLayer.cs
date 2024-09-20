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
        [SerializeField]    // ���̒n�w���\������ގ�
        private TileBase materialTileID;
        public TileBase MaterialTile => materialTileID;
        [SerializeField]    // ���̒n�w�ɐ��������z��
        private PrimevalObject[] primevalOres;
        public PrimevalObject[] PrimevalOres => primevalOres;
        [SerializeField]    // ���̒n�w�ɑ��݂���_���W����
        private PrimevalObject[] primevalDungeons;
        public PrimevalObject[] PrimevalDungeons => primevalDungeons;
    }
}