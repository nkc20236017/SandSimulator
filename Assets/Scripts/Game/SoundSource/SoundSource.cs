using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour, ISoundSourceable
{
    [Header("Sound Config")]
    [SerializeField] private GameObject _soundSourcePrefab;

    private readonly Dictionary<string, float> _instantiationTimes = new();
    private bool _canInstantiate = true;

    public void SetInstantiation(string id, float time = 1f)
    {
        _instantiationTimes[id] = time;
    }

    public void InstantiateSound(string id, Vector3 position, float despawnTime = 5f)
    {
        if (_canInstantiate)
        {
            StartCoroutine(InstantiateSoundCoroutine(id, position, despawnTime));
        }
    }

    private IEnumerator InstantiateSoundCoroutine(string id, Vector3 position, float despawnTime)
    {
        _canInstantiate = false;
        var soundSource = Instantiate(_soundSourcePrefab, position, Quaternion.identity);
        Destroy(soundSource, despawnTime);

        float instantiationTime = _instantiationTimes[id];

        yield return new WaitForSeconds(instantiationTime);
        _canInstantiate = true;
    }
}