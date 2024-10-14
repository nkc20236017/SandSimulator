using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WorldCreation;
using RandomExtensions;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]    // �V�[�h�l
    private uint seed;

    // �`�����N�̏���ێ����Ă��������^
    private Dictionary<Vector2Int, GameChunk> _gameChunkDictionary = new();
    private CancellationTokenSource _tokenSource;

    private void Start()
    {
        seed = (uint)Random.Range(uint.MinValue, uint.MaxValue);
    }

    public async UniTask ChunksLoad(GameChunk gameChunk, WorldCreatePrinciple worldCreatePrinciple, WorldDecisionerBase[] worldDecidables)
    {
        // ���[�h�p�g�[�N���𔭍s
        _tokenSource = new();

        foreach (WorldDecisionerBase worldDecision in worldDecidables)
        {
            worldDecision.Initalize
            (
                gameChunk,
                worldCreatePrinciple,
                new Xoshiro256StarStarRandom(seed)
            );
            gameChunk = await worldDecision.Execute(_tokenSource.Token);
        }

        // �Q�[���I�u�W�F�N�g��ݒu����

    }
}