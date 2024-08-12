using NUnit.Framework;

public class TestTank 
{
    [Test]
    public void TestTankLog()
    {
        var playerTank = new PlayerTank(100);

        playerTank.InCollection(CollectionType.Sand, 6);
        playerTank.InCollection(CollectionType.Sand, 6);
        playerTank.InCollection(CollectionType.Water, 6);
        playerTank.DicLog();
        playerTank.WariaiLog();

    }
}
