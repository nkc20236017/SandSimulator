using UnityEngine;
using VContainer;
using DG.Tweening;
using UnityEngine.UI;

public class LevelService : IExp
{
    private ILevelUp levelUp;
    private readonly IExpOutPut expOutPut;
    public MockStatusData mockStatusData;
    public int currentExp;

    [Inject]
    public LevelService(IExpOutPut expOutPut, ILevelUp levelUp)
    {
        mockStatusData = new MockStatusData();
        this.expOutPut = expOutPut;
        this.levelUp = levelUp;
    }

    public void AddExp(int exp)
    {
        var nextLevelHurdle = mockStatusData.BaseLevelHurdle * mockStatusData.BaseHurdleRate * mockStatusData.Level;
        currentExp += exp;

        if (nextLevelHurdle <= currentExp)
        {
            var notMachExp = currentExp - nextLevelHurdle;
            levelUp.LevelUp();
            currentExp = 0;
            if (notMachExp > 0)
            {
                currentExp = 0;
                expOutPut.OutPutExp(new(nextLevelHurdle, nextLevelHurdle));
                AddExp(notMachExp);
                return;
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
