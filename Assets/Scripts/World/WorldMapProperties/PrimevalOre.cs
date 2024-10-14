using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct PrimevalOre
    {
        [SerializeField]    // �n��ɐ��������ꍇ�̃Q�[���I�u�W�F�N�g
        private Ore exposedOreData;
        public Ore ExposedOreData => exposedOreData;
        [SerializeField]    // �n���ɐ��������ꍇ�̃^�C���x�[�X
        private TileBase buriedOre;
        public TileBase BuriedOre => buriedOre;
        [SerializeField]    // ��̍ŏ����a
        private int oreMinRadius;
        public int OreMinRadius => oreMinRadius;
        [SerializeField]    // ��̍ő唼�a
        private int oreMaxRadius;
        public int OreMaxRadius => oreMaxRadius;
        [SerializeField]    // �n���z�΂̌����
        [Range(0f, 100f)]
        private float chipped;
        public float Chipped => chipped;
        [SerializeField]    // ���������m��
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
    }
}