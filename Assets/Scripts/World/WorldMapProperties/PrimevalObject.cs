using System;
using UnityEngine;

namespace WorldCreation
{
    [Serializable]
    public struct PrimevalObject
    {
        [SerializeField]    // 生成する優先度
        private float priority;
        public float Priority => priority;
        [SerializeField]    // 生成するオブジェクト
        private GameObject modelObject;
        public GameObject ModelObject => modelObject;
        [SerializeField]    // 生成する最小値
        private float minExistence;
        public float MinExistence => minExistence;
        [SerializeField]    // 生成する最大値
        private float maxExistence;
        public float MaxExistence => maxExistence;
        [SerializeField]    // 生成される確率
        [Range(0f, 100f)]
        private float probability;
        public float Probability => probability;
        [SerializeField]    // 生成余白の形状
        private MarginShape marginShape;
        public MarginShape MarginShape => marginShape;
        [SerializeField]    // 生成余白の形状がcircleの場合は半径、boxの場合は幅
        private float marginSizeWidth;
        [SerializeField]    // 生成余白の形状がboxの場合の高さ
        private float marginSizeHeight;
        public float MarginSizeHeight
        {
            set => marginSizeHeight = value;
        }

        public float MarginRadius => marginSizeWidth;
        public Vector2 MarginSize => new Vector2(marginSizeWidth, marginSizeHeight);
    }
}