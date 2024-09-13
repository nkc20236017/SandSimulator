using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    public struct Chunk
    {
        public Vector2Int ChunkID { get; }

    }

    public class WorldMapCreator : MonoBehaviour
    {
        [SerializeField]
        private WorldMap worldMap;
        [SerializeField]
        private Transform tileMapParent;
        [SerializeField]
        private GameObject tileMapPrefab;
        [SerializeField]
        private int maxSeedValueDigit;
        [Space]
        [SerializeField]
        private TileBase tileBaseDemo;

        private int _seed;
        private bool _isQuitting;
        private LayerGenerate _layer;
        private Tilemap[] _tilemaps;
        private CancellationTokenSource _cancelTokenSource;
        private Vector2Int[] _chunkPositions;
        private IWorldGeneratable[] worldGenerator =
        {

        };

        private void Start()
        {
            // ������
            _seed = CreateSeed();
            _layer = new(_seed);
            _cancelTokenSource = new();

            // ��������
            Generate(_cancelTokenSource.Token);
        }

        private void OnApplicationQuit()
        {
            _cancelTokenSource.Cancel();
            _cancelTokenSource.Dispose();
            _isQuitting = true;
        }

        public int CreateSeed()
        {
            int seed = Random.Range(0, maxSeedValueDigit);
            Debug.Log($"�������ꂽ�V�[�h�l�F{seed}");

            return seed;
        }

        private async void Generate(CancellationToken token)
        {
            int splitedChunk
                = (worldMap.WorldSize.x / worldMap.OneChunkSize.x)
                * (worldMap.WorldSize.y / worldMap.OneChunkSize.y);

            // �`�����N�P�ʂ̍��W���v�Z���A����
            _chunkPositions = new Vector2Int[splitedChunk];
            _tilemaps = new Tilemap[splitedChunk];
            Vector2 tileMapOrigin = Vector2.zero;
            int chunkID = 0;
            for (int y = 0; y < worldMap.WorldSize.y / worldMap.OneChunkSize.y; y++)
            {
                for (int x = 0; x < worldMap.WorldSize.x / worldMap.OneChunkSize.x; x++)
                {
                    // �`�����N�̔ԍ���ݒ�
                    _chunkPositions[chunkID] = new Vector2Int(x, y);

                    // �`�����N�𐶐�
                    GameObject tileMap = Instantiate(tileMapPrefab, tileMapOrigin, Quaternion.identity, tileMapParent);
                    _tilemaps[chunkID] = tileMap.GetComponent<Tilemap>();

                    // ����X���W��ݒ�
                    tileMapOrigin.x = worldMap.OneChunkSize.x * (x + 1);
                    chunkID++;
                }

                // ���̍��W��ݒ�
                tileMapOrigin.x = 0;
                tileMapOrigin.y = worldMap.OneChunkSize.y * (y + 1);
            }

            TileBase[,] tileBases = new TileBase[worldMap.WorldSize.x, worldMap.WorldSize.y];

            // 
            for (int y = 0; y < worldMap.WorldSize.y; y++)
            {
                for (int x = 0; x < worldMap.WorldSize.x; x++)
                {
                    tileBases[x, y] = tileBaseDemo;
                }
            }


            // ���ʂ����Ƀ^�C���𐶐�����
            await Fill(tileBases, token);

            /*int[,] worldTiles = new int[worldMap.WorldSize.x, worldMap.WorldSize.y];

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
            }*/
        }

        private async UniTask Fill(TileBase[,] worldTile, CancellationToken token)
        {
            for (int y = 0; y < worldMap.OneChunkSize.y; y++)
            {
                for (int x = 0; x < worldMap.OneChunkSize.x; x++)
                {
                    for (int i = 0; i < _chunkPositions.Length; i++)
                    {
                        if (_isQuitting)
                        {
                            return;
                        }
                        _tilemaps[i].SetTile
                        (
                            new Vector3Int(x, y),
                            worldTile[x, y]
                        );
                    }

                }
                // 1�t���[���̏������𐧌�
                await UniTask.Yield(token).SuppressCancellationThrow();
            }
        }
    }
}