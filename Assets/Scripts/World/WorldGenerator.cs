using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WorldCreation;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]    // シード値
    private int seed;

    // チャンクの情報を保持しておく辞書型
    private Dictionary<Vector2Int, GameChunk> _gameChunkDictionary = new();
    private CancellationTokenSource _tokenSource;

    private void Start()
    {
        seed = (int)UnityEngine.Random.Range(float.MinValue, float.MaxValue);
    }

    public async void ChunksLoad(GameChunk gameChunk, WorldCreatePrinciple worldCreatePrinciple, WorldDecisionerBase[] worldDecidables)
    {
        // ロード用トークンを発行
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

        // ゲームオブジェクトを設置する

    }
}