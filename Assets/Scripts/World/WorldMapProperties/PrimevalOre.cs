using System;
using UnityEngine;

namespace WorldCreation
{
    [Serializable]
    public struct PrimevalOre
    {
        [SerializeField]    // �n��Ɍ����Ă���z�΂̃Q�[���I�u�W�F�N�g
        private Ore exposedOreData;
        public Ore ExposedOreData => exposedOreData;
        [SerializeField]    // ��������I�u�W�F�N�g
        private int buriedOreID;
        public int BuriedOreID => buriedOreID;
        [SerializeField]    // ��̐�����
        private int lumpDispersion;
        public int LumpDispersion => lumpDispersion;
        [SerializeField]    // �ЂƂ̉�ɑ��݂����
        private int buriedOreRadius;
        public int BuriedOreRadius => buriedOreRadius;
        [SerializeField]    // ���������m��
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
        [SerializeField]    // �z�΂𐶐�����Ԋu
        private int space;
        public int Space => space;
    }
}