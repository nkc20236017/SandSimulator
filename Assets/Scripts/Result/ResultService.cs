using System.Collections.Generic;
using VContainer;

public class ResultService : IResultAction
{
    private IOutputResultUI outResultUI;

    [Inject]
    public ResultService(IOutputResultUI outResultUI)
    {
        this.outResultUI = outResultUI;
    }

    public void ResultStart(Dictionary<Block, MineralTank> result)
    {
        var resultList = new List<MineralTank>();

        foreach (KeyValuePair<Block, MineralTank> pair in result)
        {
            var item = pair.Value;

            resultList.Add(item);
        }


        float resultAmount = 0f;


        for (int i = 0; i < resultList.Count; i++)
        {
            for (int j = 0; j < resultList[i].mineralAmount; j++)
            {
                resultAmount += resultList[i].mineralData.price;
            }
        }


        var outPutData = new ResultOutPutData(resultList, resultAmount);

        outResultUI.ResultUI(outPutData);

    }
}
