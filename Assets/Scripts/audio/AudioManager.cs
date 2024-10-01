using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class AudioManager : MonoBehaviour
{
    private IPlaySFX playSFX;
    private IPlayBGM playBGM;
    private IStopBGM stopBGM;
    private IStopSFX stopSFX;

    public static AudioManager Instance;

    [Inject]
    public void InjectAudio(IPlaySFX playSFX,IPlayBGM playBGM
        ,IStopBGM stopBGM,IStopSFX stopSFX)
    {
        this.playSFX = playSFX;
        this.playBGM = playBGM;
        this.stopBGM = stopBGM;
        this.stopSFX = stopSFX;
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlaySFX(string playSfxName) => playSFX.PlaySFX(playSfxName);

    public void PlayBGM (string playBgmName) => playBGM.PlayBGM(playBgmName);
    
    public void StopSFX(string stopSfxName) => stopSFX.StopSFX(stopSfxName);

    public void StopBGM (string stopBgmName) => stopBGM.StopBGM(stopBgmName);


}
