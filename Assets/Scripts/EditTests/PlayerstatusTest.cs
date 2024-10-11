using NUnit.Framework;
using UnityEngine;

public class PlayerstatusTest
{
    [TestCase(150)]
    public void LevelTest(int exp)
    {
        var presenter = new MockLevelPresenter();
        var levelService = new LevelService(presenter);

        //for (int i = 0; i < exp; i++)
        //{
        //    levelService.AddExp(2);
        //}

        levelService.AddExp(300);

        Assert.AreEqual(2, levelService.mockStatusData.Level);
        Assert.AreEqual(18, levelService.currentExp);
    }

    public void SingleLevelTest(int exp)
    {
        var presenter = new MockLevelPresenter();   
        var levelService = new LevelService(presenter);
        levelService.AddExp(exp);
        Assert.AreEqual(2, levelService.mockStatusData.Level);
        Assert.AreEqual(18, levelService.currentExp);
    }
}

public class MockLevelPresenter : IExpOutPut
{
    public void OutPutExp(ExpOutPutData data)
    {
        var expPercent = (float)data.Exp / (float)data.NextLevelHurdle;

        Debug.Log(expPercent+"�p�[�Z���g");

        Debug.Log($"{data.Exp}�o���l��{data.NextLevelHurdle}���̃��x���K�v�o���l");

    }
}
