using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace WorldCreation
{
    public class TileInstaller : WorldDecisionerBase
    {
        public override void Initalize(GameChunk gameChunk, WorldCreatePrinciple createPrinciple, ManagedRandom managedRandom)
        {
            base.Initalize(gameChunk, createPrinciple, managedRandom);
        }

        public override async UniTask<GameChunk> Execute(CancellationToken token)
        {
            int limitter = 0;
            for (int y = 0; y < _gameChunk.Size.y; y++)
            {
                for (int x = 0; x < _gameChunk.Size.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    // �u���b�N����C���������΂�
                    int blockID = _gameChunk.GetBlockID(position);
                    int airIndex = _createPrinciple.Blocks.AirIndex;
                    if (_gameChunk.GetBlockID(position) == airIndex) { continue; }
                    _gameChunk.TileMap.SetTile
                    (
                        (Vector3Int)position,
                        _createPrinciple.Blocks.GetBlock(blockID)
                    );

                    if (_gameChunk.TileMap.GetTile((Vector3Int)position) != null)
                    {
                        _gameChunk.TileMap.SetColor
                        (
                            (Vector3Int)position,
                            _createPrinciple
                                .LayerDecision.WorldLayers[_gameChunk.GetLayerIndex(x, y)]
                                .LayerColor
                        );
                    }

                    limitter++;

                    if (_createPrinciple.FillLimit < limitter)
                    {
                        limitter = 0;
                        await UniTask.NextFrame(token).SuppressCancellationThrow();
                    }
                }
            }

            Debug.Log("<color=#00ff00ff>�����I��</color>");
            return await UniTask.RunOnThreadPool(() => _gameChunk);
        }
    }
}