using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using WorldCreation;

public class EnemyDecisioner : DistributionDecisioner
{
    public async override UniTask<GameChunk> Execute(CancellationToken token)
    {
        // �����̒n�w�����ɍz�Ώ����擾����
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