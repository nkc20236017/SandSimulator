using UnityEngine;
using VContainer;

public class LevelService : IExp
{
    private readonly IExpOutPut expOutPut;
    public MockStatusData mockStatusData;
    public int currentExp;

    [Inject]
    public LevelService(IExpOutPut expOutPut)
    {
        mockStatusData = new MockStatusData();
        this.expOutPut = expOutPut;
    }

    public void AddExp(int exp)
    {
        var nextLevelHurdle = mockStatusData.BaseLevelHurdle * mockStatusData.BaseHurdleRate * mockStatusData.Level;
        currentExp += exp;
        
        if (nextLevelHurdle <= currentExp)
        {
            var notMachExp = currentExp - nextLevelHurdle;
            LevelUp();
            currentExp = 0;
            expOutPut.OutPutExp(new(notMachExp,nextLevelHurdle));
            if (notMachExp > 0)
            {
                currentExp = 0;
                AddExp(notMachExp);
            }
        }
        expOutPut.OutPutExp(new(currentExp, nextLevelHurdle));
    }

    private void LevelUp()
    {
        mockStatusData.Level++;
        mockStatusData.MaxHealth += mockStatusData.LevelUpBonus;
        Debug.Log(mockStatusData.Level + "Player‚ÌƒŒƒxƒ‹");
    }
}
