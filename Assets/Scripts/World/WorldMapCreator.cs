using System.Linq;
using UnityEngine;

namespace WorldCreation
{
    public class WorldMapCreator : MonoBehaviour
    {
        [SerializeField]
        private WorldMap worldMap;
        [SerializeField]
        private int maxSeedValueDigit;

        private int _seed;
        private LayerGenerate _layer;

        private void Start()
        {
            _seed = CreateSeed();
            _layer = new(_seed);
        }

        private void Generate()
        {
            int[,] worldTiles = new int[worldMap.WorldSize.x, worldMap.WorldSize.y];

            int layerHeight = worldMap.WorldSize.y;
            for (int i = 0; i < worldMap.LayerRatios.Length; i++)
            {
                layerHeight -= (int)(layerHeight * worldMap.LayerRatios[i]);

                Vector3[] border = _layer.GetBorder
                (
                    worldMap.WorldSize.x,
                    layerHeight,
                    worldMap.BorderNoiseSize,
                    (int)worldMap.RandomLimit,
                    worldMap.Amplitude
                )
                .Select(point => (Vector3)(Vector2)point)
                .ToArray();


            }
        }

        public int CreateSeed()
        {
            int seed = Random.Range(0, maxSeedValueDigit);
            Debug.Log($"生成されたシード値：{seed}");

            return seed;
        }
    }
}