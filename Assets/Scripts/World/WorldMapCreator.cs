using UnityEngine;

namespace WorldCreation
{
    public class WorldMapCreator : MonoBehaviour
    {
        [SerializeField]
        private int maxSeedValueDigit;

        private int _seed;

        private void Start()
        {
            _seed = CreateSeed();
        }

        public int CreateSeed()
        {
            int seed = Random.Range(0, maxSeedValueDigit);
            Debug.Log($"生成されたシード値：{seed}");

            return seed;
        }
    }
}