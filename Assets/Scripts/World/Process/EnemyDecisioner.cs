using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using WorldCreation;

public class EnemyDecisioner : DistributionDecisioner
{
    public async override UniTask<GameChunk> Execute(CancellationToken token)
    {
        // ’†‰›‚Ì’n‘w‚ðŒ³‚ÉzÎî•ñ‚ðŽæ“¾‚·‚é
        int indexX = _gameChunk.Size.x / 2;
        int indexY = _gameChunk.Size.y / 2;
        WorldLayer[] worldLayers
            = _createPrinciple
            .LayerDecision
            .WorldLayers;

        Vector2Int[] noisePoints = BlueNoise
        (
            GetWorldScale(),
            worldLayers[_gameChunk.GetLayerIndex(indexX, indexY)].OreDecision
        );

        return await UniTask.RunOnThreadPool(() => _gameChunk);
    }
}