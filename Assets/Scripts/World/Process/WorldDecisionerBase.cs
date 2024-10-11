using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using WorldCreation;

public abstract class WorldDecisionerBase
{
    protected GameChunk _gameChunk;
    protected WorldCreatePrinciple _createPrinciple;
    protected ManagedRandom _managedRandom;
    protected int _recycleProcessNumber;

    public abstract UniTask<GameChunk> Execute(CancellationToken token);

    /// <summary>
    /// ワールド生成処理を初期化します
    /// </summary>
    /// <param name="gameChunk">初期化対象のゲームチャンク</param>
    /// <param name="createPrinciple">作成するルール</param>
    /// <param name="managedRandom">シード値を元に決定された乱数</param>
    public virtual void Initalize(GameChunk gameChunk, WorldCreatePrinciple createPrinciple, ManagedRandom managedRandom)
    {
        _gameChunk = gameChunk;
        _createPrinciple = createPrinciple;
        _managedRandom = managedRandom;

        _recycleProcessNumber = managedRandom.UsageCount;
    }

    /// <summary>
    /// ワールド全体の大きさを返します
    /// </summary>
    /// <returns></returns>
    protected Vector2Int GetWorldScale()
    {
        return _gameChunk.Size * _createPrinciple.WorldSplidCount;
    }
}