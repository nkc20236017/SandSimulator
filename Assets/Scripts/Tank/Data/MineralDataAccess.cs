using System.Linq;
using VContainer;

public class MineralDataAccess : ITankRepository
{
    private BlockDatas mineralDataBase;

    [Inject]
    public MineralDataAccess(BlockDatas mineralDataBase)
    {
        this.mineralDataBase = mineralDataBase;
    }

    public Block Find(BlockType type)
    {
        return mineralDataBase.Block
            .Where(mineral => mineral.type == type)
            .FirstOrDefault();
    }
}