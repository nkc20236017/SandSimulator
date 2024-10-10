using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class LevelService : IExp
{
    private IPlayerStatusRepository playerStatusRepository;

    [Inject]
    public LevelService(IPlayerStatusRepository playerStatusRepository)
    {
        this.playerStatusRepository = playerStatusRepository;
    }

    public void AddExp(int exp)
    {
        var statusData = playerStatusRepository.FindPlayerData();

    }
}
