using System.Collections.Generic;
using VContainer;

public class PlayerStatusDataAccess : IPlayerStatusRepository
{
    private readonly PlayerDatas playerDatas;

    public PlayerStatusDataAccess(PlayerDatas playerDatas)
    {
        this.playerDatas = playerDatas;
    }

    public PlayerStatusData FindPlayerData()
    {
        return new PlayerStatusData (this.playerDatas);
    }

    public List<StatusValue> GetPlayerStatusData()
    {
        return playerDatas.statusValues;
    }

    public void SetPlayerData(SetPlayerStatus setPlayerStatus)
    {
        
    }

}
