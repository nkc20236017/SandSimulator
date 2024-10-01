using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class Scope : LifetimeScope
{
    [SerializeField]
    private AudioClipObject _audioClip;
    [SerializeField]
    private AudioList audioList;

    protected override void Configure(IContainerBuilder builder)
    {
       

        builder.RegisterComponent(audioList)
            .As<ISFXAudioSourece>()
            .As<IBGMAudioSourece>();
        builder.RegisterComponent(_audioClip)
            .As<IAudioContainer>();
        builder.Register<AudioPlay>(Lifetime.Singleton)
            .As<IPlayBGM>()
            .As<IPlaySFX>()
            .As<IStopBGM>()
            .As<IStopSFX>();
    }
}
