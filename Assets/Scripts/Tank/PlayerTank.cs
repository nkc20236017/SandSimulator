using System.Collections.Generic;
using UnityEngine;

public enum CollectionType
{
    Sand,
    Water,
    made
}

public class PlayerTank
{
    private float tankMax = 100;
    private float currentTankAmount;
    private float watertank;
    private float sandtank;



    private Dictionary<CollectionType, int> tankCollection = new();

    public PlayerTank(float tankMax)
    {
        this.tankMax = tankMax;
    }

    public void InCollection(CollectionType collectionType, int amount)
    {
        if (tankCollection.TryGetValue(collectionType, out int _amount))
        {
            tankCollection[collectionType] = amount += _amount;
        }
        else
        {
            tankCollection.Add(collectionType, amount);
        }
    }

    public void WariaiLog()
    {
        
        foreach (KeyValuePair<CollectionType, int> pair in tankCollection)
        {
            if(pair.Key == CollectionType.Sand)
            {
                sandtank = pair.Value;
            }
            else if(pair.Key == CollectionType.Water)
            {
                watertank = pair.Value;
            }
        }

        currentTankAmount = sandtank + watertank;

        float tankresult = currentTankAmount/tankMax;

        var waterResult = watertank / currentTankAmount;
        var sandResult = sandtank / currentTankAmount;

        Debug.Log(tankresult+"çáåvíl");
        Debug.Log(waterResult+"waterTank");
        Debug.Log(sandResult+"sandTank");

    }

    public void DicLog()
    {
        foreach (KeyValuePair<CollectionType, int> pair in tankCollection)
        {
            Debug.Log(pair.Key + "+" + pair.Value);
        }
    }



}
