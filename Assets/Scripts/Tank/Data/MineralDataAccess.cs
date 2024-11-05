using System.Linq;
using UnityEngine.Tilemaps;
using VContainer;

public class MineralDataAccess : ITankRepository
{
    private BlockData _mineralDataBase;

    [Inject]
    public MineralDataAccess(BlockData mineralDataBase)
    {
        this._mineralDataBase = mineralDataBase;
    }

    public Block Find(BlockType type)
    {
        return _mineralDataBase.Block
            .Where(mineral => mineral.type == type)
            .FirstOrDefault();
    }

    public Block Find(TileBase tile)
    {
        return _mineralDataBase.Block
            .Where(mineral=> mineral.tile == tile)
            .FirstOrDefault();
    }
}