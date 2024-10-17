using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace WorldCreation
{
    public class TileInstaller : WorldDecisionerBase
    {
        public override async UniTask<GameChunk> Execute(CancellationToken token)
        {
            int limitter = 0;
            for (int y = 0; y < _gameChunk.Size.y; y++)
            {
                for (int x = 0; x < _gameChunk.Size.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    // ブロックが空気だったら飛ばす
                    int blockID = _gameChunk.GetBlockID(position);
                    int airIndex = _createPrinciple.Blocks.AirIndex;
                    if (_gameChunk.GetBlockID(position) == airIndex) { continue; }
                    _gameChunk.GameChunkTilemap.SetTile
                    (
                        (Vector3Int)position,
                        _createPrinciple.Blocks.GetBlock(blockID)
                    );

                    if (_gameChunk.GameChunkTilemap.GetTile((Vector3Int)position) != null)
                    {
                        if (_gameChunk.GetBlockID(x, y) != 1) { continue; }

                        _gameChunk.GameChunkTilemap.SetColor
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

            Debug.Log("<color=#00ff00ff>生成終了</color>");
            return await UniTask.RunOnThreadPool(() => _gameChunk);
        }
    }
}