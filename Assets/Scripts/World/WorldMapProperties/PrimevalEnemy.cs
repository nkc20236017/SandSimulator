using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    public enum SpawnPoint
    {
        Exposed,    // ��
        Buried      // �n��
    }

    [Serializable]
    public struct PrimevalEnemy
    {
        [SerializeField]    // ���������m��
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;

        [SerializeField]
        private GameObject prefab;
        public GameObject Prefab;
        [SerializeField]
        private SpawnPoint spawnPoint;
        public SpawnPoint SpawnPoint => spawnPoint;

        [SerializeField]
        private float distance;
        public float Distance => distance;
        [SerializeField]
        private LayerMask groundLayerMask;
        public LayerMask GroundLayerMask => groundLayerMask;
    }
}