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
        if (setPlayerStatus.MaxTankCapacity == -1)
        {

        }
        else if (setPlayerStatus.MaxHealth == -1)
        {

        }
    }

}
