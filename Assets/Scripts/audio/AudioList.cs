using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class AudioList : MonoBehaviour, ISFXAudioSourece,IBGMAudioSourece
{

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    AudioSource[] ISFXAudioSourece.GetAudioSourece() => sfx;
    AudioSource[] IBGMAudioSourece.GetAudioSourece() => bgm;
}
