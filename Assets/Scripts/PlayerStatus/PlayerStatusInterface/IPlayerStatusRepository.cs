//Playerのデータにアクセスする場所
public interface IPlayerStatusRepository
{
    public PlayerStatusData FindPlayerData();

    public void SetPlayerData(SetPlayerStatus setPlayerStatus);

}
