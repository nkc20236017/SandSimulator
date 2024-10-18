using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BloomIntensity : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume volume;

    private Bloom Bloom;
    private float intensity = 0;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private float maxIntensity;
    [SerializeField]
    private float minIntensity;

    private void Start()
    {
        volume.profile.TryGetSettings(out Bloom bloom);
        if (bloom == null) { return; }
        Bloom = bloom;
        this.Bloom.intensity.value = intensity;
        AddIntensity();

    }
    private void AddIntensity()
    {
        DOVirtual.Float(minIntensity,maxIntensity,waitTime,(value)=> { Bloom.intensity.value = value; } )
            .SetEase(Ease.InBack).OnComplete(RemoveIntensity);
    }
    private void RemoveIntensity()
    {
        DOVirtual.Float(maxIntensity, minIntensity, waitTime, (value) => { Bloom.intensity.value = value; })
            .SetEase(Ease.InBack).OnComplete(AddIntensity);
    }
}
