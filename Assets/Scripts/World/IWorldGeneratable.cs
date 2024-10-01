using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using WorldCreation;
public interface IWorldGeneratable
{
    public int ExecutionOrder { get; }
    public void Initalize(Chunk chunk, WorldMap worldMap, int executionOrder);

    public UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token);
}