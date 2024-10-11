using System;
using UnityEngine;

namespace WorldCreation
{
    public enum MarginShape
    {
        Circle,
        Box
    }

    [CreateAssetMenu(fileName = "New world create principle base", menuName = "ScriptableObjects/WorldCreatePrinciple/Base")]
    public class WorldCreatePrinciple : ScriptableObject
    {
        [Header("standard worldwide")]
        [SerializeField]    // �`�����N�̕�����
        private Vector2Int worldSplidCount;
        public Vector2Int WorldSplidCount => worldSplidCount;
        [SerializeField]    // ���_�����炷�͈͂̍ŏ�
        private Vector2Int minOriginGapRange;
        public Vector2Int MinOriginGapRange => minOriginGapRange;
        [SerializeField]    // ���_�����炷�͈͂̍ő�
        private Vector2Int maxOriginGapRange;
        public Vector2Int MaxOriginGapRange => maxOriginGapRange;

        [Header("Principle Properties Data")]
        [SerializeField]    // �u���b�N�̏��ScriptableObject
        private EnvironmentBlocks blocks;
        public EnvironmentBlocks Blocks => blocks;

        [SerializeField]    // �n�w�̏��ScriptableObject
        private LayerDecisionData layerDecision;
        public LayerDecisionData LayerDecision => layerDecision;

        [SerializeField]    // ���A�̏��ScriptableObject
        private CaveDecisionData caveDecision;
        public CaveDecisionData CaveDecision => caveDecision;

        [Space]
        [SerializeField]    // �u���b�N�Ŗ��߂鏈���̐���
        private int fillLimit;
        public int FillLimit => fillLimit;
/*
        [SerializeField, Range(0f, 1f)]
        private float turtleChance;
        public float TurtleChance => turtleChance;
        [SerializeField]
        private GameObject turtlePrefab;
        public GameObject TurtlePrefab => turtlePrefab;
        [SerializeField, Range(0f, 1f)]
        private float moleChance;
        public float MoleChance => moleChance;
        [SerializeField]
        private GameObject molePrefab;
        public GameObject MolePrefab => molePrefab;
        [SerializeField]
        private int enemySpase;
        public int EnemySpase => enemySpase;*/
    }
}
