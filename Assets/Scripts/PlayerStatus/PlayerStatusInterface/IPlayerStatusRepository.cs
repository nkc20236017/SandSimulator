//Player�̃f�[�^�ɃA�N�Z�X����ꏊ
public interface IPlayerStatusRepository
{
    public PlayerStatusData FindPlayerData();

    public void SetPlayerData(SetPlayerStatus setPlayerStatus);

}
