using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using WorldCreation;
public interface IWorldGeneratable
{
    public int ExecutionOrder { get; set; }

    public UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token);
}