using System;
using UnityEngine;

namespace WorldCreation
{
    [Serializable]
    public struct CaveCombine
    {
        [SerializeField]    // �m�C�Y�̑傫��
        private Vector2 scale;
        public Vector2 Scale => scale;
        [SerializeField]    // ��ƂȂ�ꏊ�̑傫��
        [Range(0, 1)]
        private float lumpSize;
        public float LumpSize => lumpSize;
        [SerializeField]
        [Range(0, 1)]
        private float hollowSize;
        public float HollowSize => hollowSize;
        [SerializeField]    // �u���b�N�Ŗ��߂�悤�ɂ���
        private bool isBackfill;
        public bool IsBackfill => isBackfill;
        [SerializeField]
        private bool isInvert;
        public bool IsInvert => isInvert;
        [SerializeField]    // �^�C���}�b�v��ID
        private int backfillTileID;
        public int BackfillTileID => backfillTileID;
    }
}