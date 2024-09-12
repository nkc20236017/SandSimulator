using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField]
        private Color debugLayerColor;
        public Color DebugLayerColor => debugLayerColor;
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
        public Vector2Int WorldSize => worldScale;
        [SerializeField]    // 1�`�����N�̑傫��
        private Vector2Int oneChunkSize;
        public Vector2Int OneChunkSize => oneChunkSize;
        [Tooltip("���̂ق����[���A�E�ɍs���ɂ�Đ󂭂Ȃ�B\n�F�͍l�����Ȃ����ߎ��R�B�F�������ł��镔���͒n�w�̕ω����ɂ₩�ɂȂ�܂�")]
        [SerializeField]    // �n�w�̊���
        [Range(0f, 1f)]
        private float[] layerRatios;
        public float[] LayerRatios => layerRatios;
        [SerializeField]    // �����_���l�𐶐����鎞�̍ő�l
        private float randomLimit;
        public float RandomLimit => randomLimit;
        [SerializeField]    // �����_���l�̐U�ꕝ
        [Range(0f, 1f)]
        private float amplitude;
        public float Amplitude => amplitude;

        [Space]
        [Header("each layer")]
        [SerializeField]    // ���ꂼ��̒n�w�̏��
        private WorldLayer[] worldLayers;
        public WorldLayer[] WorldLayers => worldLayers;
        [SerializeField]    // �n�w�̋��E���̘c��
        private float borderNoiseSize;
        public float BorderNoiseSize => borderNoiseSize;

#if UNITY_EDITOR
        private float[] layerRatiosTemp = new float[0];

        private void OnValidate()
        {
            // �n�w�̐�����v���Ȃ��ꍇ�ɏC������
            if (worldLayers.Length != layerRatios.Length + 1)
            {
                WorldLayer[] worldLayersTemp = new WorldLayer[layerRatios.Length + 1];
                for (int i = 0; i < layerRatios.Length + 1; i++)
                {
                    if (worldLayers.Length <= i) { break; }
                    worldLayersTemp[i] = worldLayers[i];
                }
                worldLayers = worldLayersTemp;
            }

            // �n�w�̊����𒲐�����
            /*
             * �����ł��邽�߈ȍ~�̃}�W�b�N�i���o�[�u1�v��100%���w��
             */
            // �ω����Ă��Ȃ���Ώ������I��
            if (layerRatios.Length == 0)
            {
                layerRatiosTemp = new float[0];
                return;
            }
            if (layerRatiosTemp.Length != 0 && layerRatios.SequenceEqual(layerRatiosTemp))
            {
                return;
            }

            if (layerRatiosTemp.Length != layerRatios.Length)
            {
                layerRatiosTemp = layerRatios.ToArray();
            }

            float ratioTotal = layerRatios.Sum();
            // ���v���ق�1�ł���ΏI��
            if (Mathf.Approximately(1, ratioTotal) == true)
            {
                return;
            }

            // �ω�������ω������ꏊ���擾����
            int changedIndex = -1;
            for (int i = 0; i < layerRatios.Length; i++)
            {
                if (Mathf.Approximately(layerRatios[i], layerRatiosTemp[i]) == false)
                {
                    changedIndex = i;
                    break;
                }
            }
            // �z��̒ǉ��ɂ��ω��������ꍇ�V�����쐬���ꂽ�v�f���c��̐����ɂ���
            if (changedIndex == -1)
            {
                Debug.Log(ratioTotal);

                float layerRespite = 1 - (ratioTotal - layerRatios[layerRatios.Length - 1]);
                if (0 <= layerRespite && layerRespite <= 1)
                {
                    layerRatios[layerRatios.Length - 1] = layerRespite;
                }
                return;
            }

            // ���v�l�̕��ς��擾
            float changedRespite = 1 - layerRatios[changedIndex];
            int ignore = layerRatios.Where(_ => Mathf.Approximately(0, _)).ToArray().Length + 1;
            float otherTotal = 1 - ratioTotal;
            float changedQuantity = otherTotal / (layerRatios.Length - ignore);

            // 1���傫����Α��̒l��������
            for (int i = 0; i < layerRatios.Length; i++)
            {
                // �ύX���ꂽ�v�f�͒��߂��Ȃ�
                if (changedIndex == i) { continue; }
                layerRatios[i] += changedQuantity;
                layerRatios[i] = Mathf.Clamp01(layerRatios[i]);
            }

            layerRatiosTemp = layerRatios.ToArray();
        }
#endif
    }
}
