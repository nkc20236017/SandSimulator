using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct CaveProcedures
    {
        [SerializeField]    // �m�C�Y�̑傫��
        private Vector2 scale;
        public Vector2 Scale => scale;
        [SerializeField]    // ���臒l
        [Range(0, 1)]
        private float lumpThreshold;
        public float LumpThreshold => lumpThreshold;
        [SerializeField]    // ���]����臒l
        [Range(0, 1)]
        private float hollowThreshold;
        public float HollowThreshold => hollowThreshold;
        [SerializeField]    // �ݒu����Ă��邩�Ɋւ�炸�㏑������
        private bool isOrverride;
        public bool IsOrverride => isOrverride;
        [SerializeField]    // �u���b�N�Ŗ��߂�悤�ɂ���
        private bool isBackfill;
        public bool IsBackfill => isBackfill;
        [SerializeField]    // �K�p�͈͂̔��]������
        private bool isInvert;
        public bool IsInvert => isInvert;
        [SerializeField]    // �^�C��
        private TileBase backfillTile;
        public TileBase BackfillTile => backfillTile;
    }
}