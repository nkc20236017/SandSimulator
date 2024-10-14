using UnityEngine;

[CreateAssetMenu(menuName = "AudioContainer")]
public class AudioClipObject : ScriptableObject, IAudioContainer
{
    [SerializeField]
    AudioClip[] clips;

    public AudioClip[] GetAudioClips()
    {
        return clips;
    }
}
