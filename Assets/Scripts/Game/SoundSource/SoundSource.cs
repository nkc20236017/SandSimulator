using System.Collections;
using UnityEngine;

public class SoundSource : MonoBehaviour, ISoundSourceable
{
    [Header("Sound Config")]
    [SerializeField] private GameObject _soundSourcePrefab;
    [SerializeField] private float _instantiationTime = 3f;

    private bool _canInstantiate = true;

    public void InstantiateSound(Vector3 position, float despawnTime = 3f)
    {
        if (_canInstantiate)
        {
            StartCoroutine(InstantiateSoundCoroutine(position, despawnTime));
        }
    }

    private IEnumerator InstantiateSoundCoroutine(Vector3 position, float despawnTime)
    {
        _canInstantiate = false;
        var soundSource = Instantiate(_soundSourcePrefab, position, Quaternion.identity);
        Destroy(soundSource, despawnTime);
        yield return new WaitForSeconds(_instantiationTime);
        
        _canInstantiate = true;
    }
}
