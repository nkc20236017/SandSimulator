using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaySFX 
{
    AudioSource PlaySFX(string clipName);
}

public interface IPlayBGM
{
    AudioSource PlayBGM(string clipName);
}

public interface IStopSFX
{
    AudioSource StopSFX(string clipName);
}

public interface IStopBGM
{
    AudioSource StopBGM(string clipName);
}

public interface IMonoPlaySFX
{
    void PlaySFX(IPlaySFX playSFX);
}


public interface IAudioContainer
{
    AudioClip[] GetAudioClips();
}

public interface ISFXAudioSourece
{
    AudioSource[] GetAudioSourece();
}
public interface IBGMAudioSourece
{
    AudioSource[] GetAudioSourece();
}