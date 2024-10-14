using Cysharp.Threading.Tasks;
using RandomExtensions;
using System.Threading;
using UnityEngine;
using WorldCreation;

public abstract class WorldDecisionerBase
{
    protected GameChunk _gameChunk;
    protected WorldCreatePrinciple _createPrinciple;
    protected IRandom _random;

    public abstract UniTask<GameChunk> Execute(CancellationToken token);

    /// <summary>
    /// ���[���h�������������������܂�
    /// </summary>
    /// <param name="gameChunk">�������Ώۂ̃Q�[���`�����N</param>
    /// <param name="createPrinciple">�쐬���郋�[��</param>
    /// <param name="managedRandom">�V�[�h�l�����Ɍ��肳�ꂽ����</param>
    public virtual void Initalize(GameChunk gameChunk, WorldCreatePrinciple createPrinciple, IRandom managedRandom)
    {
        _gameChunk = gameChunk;
        _createPrinciple = createPrinciple;
        _random = managedRandom;
    }

    /// <summary>
    /// ���[���h�S�̂̑傫����Ԃ��܂�
    /// </summary>
    /// <returns></returns>
    protected Vector2Int GetWorldScale()
    {
        return _gameChunk.Size * _createPrinciple.WorldSplidCount;
    }
}