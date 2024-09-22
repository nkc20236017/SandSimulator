using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DemoPlayerDataAccess : IDemoPlayeRepository
{
    private DemoPlayerDataObject playerData;

    [Inject]
    public DemoPlayerDataAccess(DemoPlayerDataObject playerData)
    {
        this.playerData = playerData;
    }

    public DemoPlayerDataObject Find()
    {
        return playerData;
    }
}
