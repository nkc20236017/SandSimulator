//Player�̃f�[�^�ɃA�N�Z�X����ꏊ
using System.Collections.Generic;

public interface IPlayerStatusRepository
{
    public PlayerStatusData FindPlayerData();

    public void SetPlayerData(SetPlayerStatus setPlayerStatus);

    public List<StatusValue> GetPlayerStatusData();

}
