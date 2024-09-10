using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    public enum MarginShape
    {
        Circle,
        Box
    }

    [Serializable]
    public struct CaveLayer
    {
        [SerializeField]    // ���߂�p�[�e�B�N���^�C��(�󂾂ƍ폜����)
        private TileBase fillingTile;
        [SerializeField]    // ���̃��C���[���e����^����G���A�̍ŏ����W
        private Vector2Int minImpactAreaPosition;
        [SerializeField]    // ���̃��C���[���e����^����G���A�̍ő���W
        private Vector2Int maxImpactAreaPosition;
        [SerializeField]    // �������郏�[���h�̃V�[�h�l
        private Vector2 seed;
        [SerializeField]    // �ό`����p�x(�l��傫������Ɖ򂪍ׂ����Ȃ�)
        private float frequency;
        [SerializeField]    // ��؂�l(�l��傫������ƒʘH�̕����L���Ȃ�)
        [Range(0, 1)]
        private float extent;

        public TileBase FillingTile => fillingTile;
        public Vector2Int MinImpactAreaPosition => minImpactAreaPosition;
        public Vector2Int MaxImpactAreaPosition => maxImpactAreaPosition;
        public Vector2 Seed => seed;
        public float Frequency => frequency;
        public float Extent => extent;
    }

    [Serializable]
    public struct WorldLayer
    {
        [SerializeField]    // ���̒n�w���\������ގ�
        private TileBase[] materialTiles;
        public TileBase[] MaterialTiles => materialTiles;
        [SerializeField]    // ���̒n�w�ɐ��������z��
        private PrimevalObject[] primevalOres;
        public PrimevalObject[] PrimevalOres => primevalOres;
        [SerializeField]    // ���̒n�w�ɑ��݂���G
        private PrimevalObject[] primevalEnemies;
        public PrimevalObject[] PrimevalEnemies => primevalEnemies;
        [SerializeField]    // ���̒n�w�ɑ��݂���_���W����
        private PrimevalObject[] primevalDungeons;
        public PrimevalObject[] PrimevalDungeons => primevalDungeons;
    }

    [Serializable]
    public struct PrimevalObject
    {
        [SerializeField]    // ��������D��x
        private float priority;
        public float Priority => priority;
        [SerializeField]    // ��������I�u�W�F�N�g
        private GameObject modelObject;
        public GameObject ModelObject => modelObject;
        [SerializeField]    // ��������ŏ��l
        private float minExistence;
        public float MinExistence => minExistence;
        [SerializeField]    // ��������ő�l
        private float maxExistence;
        public float MaxExistence => maxExistence;
        [SerializeField]    // ���������m��
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
        [SerializeField]    // �����]���̌`��
        private MarginShape marginShape;
        public MarginShape MarginShape => marginShape;
        [SerializeField]    // �����]���̌`��circle�̏ꍇ�͔��a�Abox�̏ꍇ�͕�
        private float marginSizeWidth;
        [SerializeField]    // �����]���̌`��box�̏ꍇ�̍���
        private float marginSizeHeight;
        public float MarginSizeHeight
        {
            set => marginSizeHeight = value;
        }

        public float MarginRadius => marginSizeWidth;
        public Vector2 MarginSize => new Vector2(marginSizeWidth, marginSizeHeight);
    }

    [CreateAssetMenu(fileName = "New world map", menuName = "Config/WorldMap")]
    public sealed class WorldMap : ScriptableObject
    {
        [Header("standard worldwide")]
        [SerializeField]    // ���E�̍ő�T�C�Y
        private Vector2Int worldScale;
        // �����̎����ƂȂ����ꍇ�Ɏg�p
        /*[Header("������"), SerializeField]
        private Vector2Int chunk;*/
        [Tooltip("���̂ق����[���A�E�ɍs���ɂ�Đ󂭂Ȃ�B\n�F�͍l�����Ȃ����ߎ��R�B�F�������ł��镔���͒n�w�̕ω����ɂ₩�ɂȂ�܂�")]
        [SerializeField]
        private Gradient layerGradient;
        [Space]
        [Header("each layer")]
        [SerializeField]
        private WorldLayer[] worldLayers;

#if UNITY_EDITOR
        private void OnValidate()
        {
            int layerNumber = layerGradient.colorKeys.Length / 2;
            if (worldLayers.Length != layerNumber)
            {
                WorldLayer[] worldLayersTemp = new WorldLayer[layerNumber];
                for (int i = 0; i < layerNumber; i++)
                {
                    if (worldLayers.Length <= i) { break; }
                    worldLayersTemp[i] = worldLayers[i];
                }
                worldLayers = worldLayersTemp;
            }
        }
        private void SetMarginSizeHeight(PrimevalObject primevalObject)
        {
            if (primevalObject.MarginShape == MarginShape.Circle)
            {
                primevalObject.MarginSizeHeight = 0;
            }
        }
#endif
    }
}
