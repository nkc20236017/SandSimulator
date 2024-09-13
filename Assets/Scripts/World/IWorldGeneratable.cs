using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using WorldCreation;
public interface IWorldGeneratable
{
    public UniTask<TileBase[,]> Execute(TileBase[,] worldTile, WorldMap worldMap, CancellationToken token);
}