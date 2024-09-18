using System;
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
        private int materialTileID;
        public int MaterialTileID => materialTileID;
        [SerializeField]    // ���̒n�w�ɐ��������z��
        private PrimevalObject[] primevalOres;
        public PrimevalObject[] PrimevalOres => primevalOres;
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
        [SerializeField]    // ���_�����炷�͈͂̍ŏ�
        private Vector2Int minOriginGapRange;
        public Vector2Int MinOriginGapRange => minOriginGapRange;
        [SerializeField]    // ���_�����炷�͈͂̍ő�
        private Vector2Int maxOriginGapRange;
        public Vector2Int MaxOriginGapRange => maxOriginGapRange;
        [SerializeField]    // �����_���l�𐶐����鎞�̍ő�l
        private float randomLimit;
        public float RandomLimit => randomLimit;
        [SerializeField]    // �����_���l�̐U�ꕝ
        [Range(0f, 1f)]
        private float amplitude;
        public float Amplitude => amplitude;
        [SerializeField]
        private TileBase[] blockList;
        public TileBase[] BlockList => blockList;

        [Space]
        [Header("Chunk")]
        [SerializeField]    // 1�`�����N�̑傫��
        private Vector2Int oneChunkSize;
        public Vector2Int OneChunkSize => oneChunkSize;
        [SerializeField]
        private int fillLimit;
        public int FillLimit => fillLimit;

        [Space]
        [Header("each layer")]
        [SerializeField]    // �n�w�̊���
        [Range(0f, 1f)]
        private float[] layerRatios;
        public float[] LayerRatios => layerRatios;
        [SerializeField]    // �n�w�̋��E���̘c��
        private float borderDistortionPower;
        public float BorderDistortionPower => borderDistortionPower;
        [SerializeField]    // ���ꂼ��̒n�w�̏��
        private WorldLayer[] worldLayers;
        public WorldLayer[] WorldLayers => worldLayers;

#if UNITY_EDITOR
        private float[] layerRatiosOld = new float[0];

        private void OnValidate()
        {
            DebugValidateNumberOfLayer();

            DebugValidateLayerRatio();

            // DebugValidateTileWeight();
        }

        /// <summary>
        /// �n�w�̐�����v���Ȃ��ꍇ�ɏC������
        /// </summary>
        private void DebugValidateNumberOfLayer()
        {
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
        }

        /// <summary>
        /// �n�w�̊�����100%�ɒ�������
        /// </summary>
        private void DebugValidateLayerRatio()
        {
            (int changedIndex, bool isChanged) = GetChangedValue(ref layerRatios, ref layerRatiosOld);
            if (!isChanged) { return; }

            float ratioTotal = layerRatios.Sum();
            // �z��̒ǉ��ɂ��ω��������ꍇ�V�����쐬���ꂽ�v�f���c��̐����ɂ���
            if (changedIndex == -1)
            {
                float layerRespite = 1 - (ratioTotal - layerRatios[layerRatios.Length - 1]);
                if (0 <= layerRespite && layerRespite <= 1)
                {
                    layerRatios[layerRatios.Length - 1] = layerRespite;
                }
                return;
            }

            // ���v�l�̕��ς��擾
            float changedRespite = 1 - layerRatios[changedIndex];
            int ignore = layerRatios
                .Where(_ => Mathf.Approximately(0, _))
                .ToArray()
                .Length + 1;
            float otherTotal = 1 - ratioTotal;
            float changedQuantity = otherTotal / (layerRatios.Length - ignore);

            // 1���傫����Α��̒l��������
            for (int i = 0; i < layerRatios.Length; i++)
            {
                // �ύX���ꂽ�v�f�����b�N����Ă���v�f�͒��߂��Ȃ�
                if (changedIndex >= i) { continue; }
                layerRatios[i] += changedQuantity;
                layerRatios[i] = Mathf.Clamp01(layerRatios[i]);
            }

            layerRatiosOld = layerRatios.ToArray();
        }

        private (int value, bool isChanged) GetChangedValue(ref float[] current, ref float[] old)
        {
            // �f�[�^���폜����Ă���ΌÂ����̂𓯊�
            if (current.Length == 0)
            {
                old = new float[0];
                return (0, false);
            }
            // ���݂ƌÂ����̂��قڈꏏ�ł���ΏI��
            if (old.Length != 0 && current.SequenceEqual(old))
            {
                return (0, false);
            }
            // ���݂Ɖߋ��̔z��̒������قȂ�ΐV�K�쐬
            if (old.Length != current.Length)
            {
                old = current.ToArray();
            }

            float ratioTotal = current.Sum();
            // ���v���ق�1�ł���ΏI��
            if (Mathf.Approximately(1, ratioTotal) == true)
            {
                return (0, false);
            }

            // �ω�������ω������ꏊ���擾����
            int changedIndex = -1;
            for (int i = 0; i < current.Length; i++)
            {
                if (Mathf.Approximately(current[i], old[i]) == false)
                {
                    changedIndex = i;
                    break;
                }
            }

            return (changedIndex, true);
        }
#endif
    }
}
