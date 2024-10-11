using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WorldCreation;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]    // �V�[�h�l
    private int seed;

    // �`�����N�̏���ێ����Ă��������^
    private Dictionary<Vector2Int, GameChunk> _gameChunkDictionary = new();
    private CancellationTokenSource _tokenSource;

    private void Start()
    {
        seed = (int)UnityEngine.Random.Range(float.MinValue, float.MaxValue);
    }

    public async void ChunksLoad(GameChunk gameChunk, WorldCreatePrinciple worldCreatePrinciple, WorldDecisionerBase[] worldDecidables)
    {
        // ���[�h�p�g�[�N���𔭍s
        _tokenSource = new();
        ManagedRandom random = new ManagedRandom(seed);

        foreach (WorldDecisionerBase worldDecision in worldDecidables)
        {
            worldDecision.Initalize
            (
                gameChunk,
                worldCreatePrinciple,
                random
            );

            gameChunk = await worldDecision.Execute(_tokenSource.Token);
        }

        // �Q�[���I�u�W�F�N�g��ݒu����

    }
}