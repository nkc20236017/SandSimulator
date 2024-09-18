using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: �w�̎擾

namespace WorldCreation
{
    public class WorldMapCreator : MonoBehaviour
    {
        [SerializeField]
        private int seed;
        public int Seed => seed;
        [SerializeField]
        private WorldMap worldMap;
        [SerializeField]
        private Transform tileMapParent;
        [SerializeField]
        private GameObject tileMapPrefab;
        [Space]
        [SerializeField]
        private Transform point;


        private bool _isQuitting;
        private LayerGenerate _layer;
        private CancellationTokenSource _cancelTokenSource;
        private Vector2 _tileMapOrigin;
        private ManagedRandom _randomization;
        private Chunk[,] _chunks;
        private IWorldGeneratable[] _worldGenerators =
        {
            new ChunkLoader()
        };

        private void Start()
        {
            if (seed == 0)
            {
                // seed���ݒ肳��Ă��Ȃ���ΐV�K�Ő�������
                seed = Random.Range(0, int.MaxValue);
            }

            _randomization = new(seed);

            // ������
            _layer = new(_randomization.Range(0, 0));
            _cancelTokenSource = new();

            // ��������
            Initalize();
            GenerateAll(_cancelTokenSource.Token);
        }
        public Tilemap GetChunk(Vector2 position)
        {
            return GetChunk(position, Vector2Int.zero);
        }

        public Tilemap GetChunk(Vector2 position, Vector2Int chunkVector)
        {
            // ���_����̋��������߂�
            Vector2 originDistance = position - _tileMapOrigin;

            // �����}�C�i�X�ł���΃G���[�Ƃ���
            if (originDistance.x < 0 || originDistance.y < 0)
            {
                return null;
            }

            Vector2Int splitedVector = new
            (
                (int)(originDistance.x / worldMap.OneChunkSize.x),
                (int)(originDistance.y / worldMap.OneChunkSize.y)
            );

            // �`�����N�����݂����
            Tilemap result;
            try
            {
                result
                   = _chunks[splitedVector.x + chunkVector.x, splitedVector.y + chunkVector.y].TileMap;
            }
            catch
            {
                result = null;
            }

            return result;
        }

        private void Update()
        {
            Debug.Log($"{GetChunk(point.position, Vector2Int.one)}", GetChunk(point.position, Vector2Int.one).gameObject);
        }

        private void Initalize()
        {
            // �`�����N�A���C�͈̔͂����߂�
            Vector2Int split = new Vector2Int
            (
                worldMap.WorldSize.x / worldMap.OneChunkSize.x,
                worldMap.WorldSize.y / worldMap.OneChunkSize.y
            );

            _chunks = new Chunk[split.x, split.y];

            // �`�����N�̐������_���擾
            Vector2 origin = new
            (
                _randomization.Range(worldMap.MinOriginGapRange.x, worldMap.MaxOriginGapRange.x),
                _randomization.Range(worldMap.MinOriginGapRange.y, worldMap.MaxOriginGapRange.y)
            );
            _tileMapOrigin = origin;

            // �`�����N�P�ʂ̍��W���v�Z���A����
            for (int y = 0; y < split.y; y++)
            {
                for (int x = 0; x < split.x; x++)
                {
                    // �`�����N�𐶐�
                    GameObject tileMap = Instantiate(tileMapPrefab, origin, Quaternion.identity, tileMapParent);

                    _chunks[x, y] = new Chunk(_randomization, tileMap.GetComponent<Tilemap>());

                    // ����X���W��ݒ�
                    origin.x += worldMap.OneChunkSize.x;
                }

                // ���̍��W��ݒ�
                origin.x = _tileMapOrigin.x;
                origin.y += worldMap.OneChunkSize.y;
            }
        }

        private void OnApplicationQuit()
        {
            _cancelTokenSource.Cancel();
            _cancelTokenSource.Dispose();
            _isQuitting = true;
        }


        private async void GenerateAll(CancellationToken token)
        {
            // �S�Ẵ`�����N��ǂݍ���
            for (int x = 0; x < _chunks.GetLength(0); x++)
            {
                for (int y = 0; y < _chunks.GetLength(1); y++)
                {
                    foreach (IWorldGeneratable worldGenerator in _worldGenerators)
                    {
                        _chunks[x, y] = await worldGenerator.Execute(_chunks[x, y], worldMap, token);
                        worldGenerator.ExecutionOrder = _randomization.UsageCount;
                    }
                }
            }
        }
        /*
                private async UniTask Fill(TileBase[,] worldTile, CancellationToken token)
                {
                    for (int y = 0; y < worldMap.OneChunkSize.y; y++)
                    {
                        for (int x = 0; x < worldMap.OneChunkSize.x; x++)
                        {
                            for (int i = 0; i < _chunks.Length; i++)
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
                }*/
    }
}