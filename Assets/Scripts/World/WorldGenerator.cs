using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WorldCreation;
using RandomExtensions;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]    // シード値
    private uint seed;

    // チャンクの情報を保持しておく辞書型
    private Dictionary<Vector2Int, GameChunk> _gameChunkDictionary = new();
    private CancellationTokenSource _tokenSource;

    private void Start()
    {
        seed = (uint)Random.Range(uint.MinValue, uint.MaxValue);
    }

    public async UniTask ChunksLoad(GameChunk gameChunk, WorldCreatePrinciple worldCreatePrinciple, WorldDecisionerBase[] worldDecidables)
    {
        // ロード用トークンを発行
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

        // ゲームオブジェクトを設置する

    }
}