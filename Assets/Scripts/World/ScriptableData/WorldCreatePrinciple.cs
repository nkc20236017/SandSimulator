using System;
using System.Collections.Concurrent;
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

    [CreateAssetMenu(fileName = "New world map", menuName = "Config/WorldCreatePrinciple")]
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
        [SerializeField]    // ランダム値の振れ幅
        [Range(0f, 1f)]
        private float borderAmplitude;
        public float BorderAmplitude => borderAmplitude;

        [Header("Principle Properties Data")]
        [SerializeField]
        private EnvironmentBlocks blocks;
        public EnvironmentBlocks Blocks => blocks;

        [SerializeField]
        private LayerDecisionData layerDecision;
        public LayerDecisionData LayerDecision => layerDecision;


        [Space]
        [SerializeField]
        private int fillLimit;
        public int FillLimit => fillLimit;

        [SerializeField]
        private CaveProcedures[] caveCombines;
        public CaveProcedures[] CaveCombines => caveCombines;

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
        public int EnemySpase => enemySpase;
    }
}
