using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct WorldLayer
    {
        [SerializeField]    // �n�w�̐F
        private Color layerColor;
        public Color LayerColor => layerColor;
        [SerializeField]    // �n�w���\������ގ�
        private TileBase materialTile;
        public TileBase MaterialTile => materialTile;
        [SerializeField]    // �z�΂̐������
        private OreDecisionData oreDecision;
        public OreDecisionData OreDecision => oreDecision;
        [SerializeField]    // ���̒n�w�ɑ��݂���_���W����
        private PrimevalOre[] primevalDungeons;
        public PrimevalOre[] PrimevalDungeons => primevalDungeons;
    }
}