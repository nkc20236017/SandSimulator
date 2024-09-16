using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class MockPlayerDataAccess : IDemoPlayeRepository
{
    private readonly DemoPlayerDataObject demoPlayerData;

    public MockPlayerDataAccess(DemoPlayerDataObject demoPlayerData)
    {
        this.demoPlayerData = demoPlayerData;
    }

    DemoPlayerDataObject IDemoPlayeRepository.Find()
    {
        return demoPlayerData;
    }
}
