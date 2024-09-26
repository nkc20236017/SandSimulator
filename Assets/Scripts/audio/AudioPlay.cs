using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class AudioPlay : IPlaySFX, IPlayBGM, IStopBGM, IStopSFX
{

    private IAudioContainer audioContainer;
    private IBGMAudioSourece bgmAudioSourece;
    private ISFXAudioSourece sfxAudioSourece;

    [Inject]
    public AudioPlay
        (
        IAudioContainer audioContainer,
        IBGMAudioSourece bgmAudioSourece,
        ISFXAudioSourece sfxAudioSourece
        )
    {
        this.audioContainer = audioContainer;
        this.sfxAudioSourece = sfxAudioSourece;
        this.bgmAudioSourece = bgmAudioSourece;
    }

    public AudioSource PlayBGM(string clipName)
    {
        var targetClip = audioContainer.GetAudioClips()
            .Where(clip => clip.name == clipName)
            .FirstOrDefault();
        if (targetClip == null)
        {
            return null;
        }
        var targetSoruce = bgmAudioSourece.GetAudioSourece()
            .Where(soruce => !soruce.isPlaying)
            .FirstOrDefault();
        if (targetSoruce == null)
        {
            return null;
        }

        targetSoruce.clip = targetClip;
        targetSoruce.Play();
        return targetSoruce;

    }

    public AudioSource PlaySFX(string clipName)
    {
        var targetClip = audioContainer.GetAudioClips()
    .Where(clip => clip.name == clipName)
    .FirstOrDefault();
        if (targetClip == null)
        {
            return null;
        }

        var targetSoruce = sfxAudioSourece.GetAudioSourece()
            .Where(soruce => !soruce.isPlaying)
            .FirstOrDefault();
        if (targetSoruce == null)
        {
            return null;
        }
        targetSoruce.PlayOneShot(targetClip);
        return targetSoruce;
    }

    public AudioSource StopBGM(string clipName)
    {
        var targetSoruce = bgmAudioSourece.GetAudioSourece()
    .Where(soruce => soruce.isPlaying)
    .Where(soruce => soruce.clip != null)
    .Where(soruce => soruce.clip.name == clipName)
    .FirstOrDefault();
        if (targetSoruce == null)
        {
            return null;
        }

        targetSoruce.Stop();
        targetSoruce.clip = null;
        return targetSoruce ;
    }

    public AudioSource StopSFX(string clipName)
    {
        var targetSoruce = sfxAudioSourece.GetAudioSourece()
        .Where(soruce => soruce.isPlaying)
        .Where(soruce => soruce.clip != null)
        .Where(soruce => soruce.clip.name == clipName)
        .FirstOrDefault();
        if (targetSoruce == null)
        {
            return null;
        }

        targetSoruce.Stop();
        targetSoruce.clip = null;
        return targetSoruce;
    }
}
