using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PlayerStatusService : IStatusModifier
{
    private Dictionary<PlayerStatusType, StatusValue> StatusDictionary;

    private IPlayerStatusRepository playerStatusRepository;

    [Inject]
    public PlayerStatusService(IPlayerStatusRepository playerStatusRepository)
    {
        this.playerStatusRepository = playerStatusRepository;
    }

    public void AddModifier(ModifierData data)
    {
        if(StatusDictionary.TryGetValue(data.StatusType, out StatusValue value))
        {
            value.AddModifier(data.ModifierAmount);
        }
    }

    public void StatusSetUp()
    {
        for (int i = 0; i < playerStatusRepository.GetPlayerStatusData().Count; i++)
        {
            var statusValue = playerStatusRepository.GetPlayerStatusData()[i];
            StatusDictionary.Add(statusValue.statusType, statusValue);
        }
    }

}
