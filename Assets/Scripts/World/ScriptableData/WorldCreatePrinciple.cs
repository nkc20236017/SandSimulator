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
        [SerializeField]    // チャンクの分割数
        private Vector2Int worldSplidCount;
        public Vector2Int WorldSplidCount => worldSplidCount;
        [SerializeField]    // 原点をずらす範囲の最小
        private Vector2Int minOriginGapRange;
        public Vector2Int MinOriginGapRange => minOriginGapRange;
        [SerializeField]    // 原点をずらす範囲の最大
        private Vector2Int maxOriginGapRange;
        public Vector2Int MaxOriginGapRange => maxOriginGapRange;

        [Header("Principle Properties Data")]
        [SerializeField]    // ブロックの情報ScriptableObject
        private EnvironmentBlocks blocks;
        public EnvironmentBlocks Blocks => blocks;

        [SerializeField]    // 地層の情報ScriptableObject
        private LayerDecisionData layerDecision;
        public LayerDecisionData LayerDecision => layerDecision;

        [SerializeField]    // 洞窟の情報ScriptableObject
        private CaveDecisionData caveDecision;
        public CaveDecisionData CaveDecision => caveDecision;

        [Space]
        [SerializeField]    // ブロックで埋める処理の制限
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
