using System.Linq;

public class MineralDataAccess : ITankRepository
{
    private MineralDataBase mineralDataBase;

    public MineralDataAccess(MineralDataBase mineralDataBase)
    {
        this.mineralDataBase = mineralDataBase;
    }

    public MineralData Find(MineralType type)
    {
        return mineralDataBase.data
            .Where(mineral => mineral.itemType == type)
            .FirstOrDefault();
    }
}