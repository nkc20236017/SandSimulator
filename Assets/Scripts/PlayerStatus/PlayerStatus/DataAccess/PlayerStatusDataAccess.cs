using VContainer;

public class PlayerStatusDataAccess : IPlayerStatusRepository
{
    private readonly PlayerDatas playerDatas;

    [Inject]
    public PlayerStatusDataAccess(PlayerDatas playerDatas)
    {
        this.playerDatas = playerDatas;
    }

    public PlayerStatusData FindPlayerData()
    {
        return new PlayerStatusData (this.playerDatas);
    }

    public void SetPlayerData(SetPlayerStatus setPlayerStatus)
    {
        if (setPlayerStatus.MaxTankCapacity != -1)
        {
            playerDatas.MaxTankCapacity = setPlayerStatus.MaxTankCapacity;
        }

        if (setPlayerStatus.MaxHealth != -1)
        {
            playerDatas.MaxHealth = setPlayerStatus.MaxHealth;
        }

        if(setPlayerStatus.CurrentTankCapacity != -1)
        {
            playerDatas.CurrentTankCapacity = setPlayerStatus.CurrentTankCapacity;
        }

        if(setPlayerStatus.CurrentHealth != -1)
        {
            playerDatas.CurrentHealth = setPlayerStatus.CurrentHealth;
        }


    }

}
