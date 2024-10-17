using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct PrimevalOre
    {
        [SerializeField]    // ���������m��
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
        [Space]
        [SerializeField]    // �n��ɐ��������ꍇ�̃Q�[���I�u�W�F�N�g
        private Ore exposedOreData;
        public Ore ExposedOreData => exposedOreData;
        [SerializeField]
        private float distance;
        public float Distance => distance;
        [SerializeField]
        private LayerMask groundLayerMask;
        public LayerMask GroundLayerMask => groundLayerMask;
        [Space]
        [SerializeField]    // �n���ɐ��������ꍇ�̃^�C���x�[�X
        private TileBase buriedOre;
        public TileBase BuriedOre => buriedOre;
        [SerializeField]    // ��̍ŏ����a
        private int minRadius;
        public int MinRadius => minRadius;
        [SerializeField]    // ��̍ő唼�a
        private int maxRadius;
        public int MaxRadius => maxRadius;
        [SerializeField]    // �n���z�΂̌����
        [Range(0f, 100f)]
        private float chipped;
        public float Chipped => chipped;
    }
}