using System;
using UnityEngine;

namespace WorldCreation
{
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
}