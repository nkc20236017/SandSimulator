using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    [SerializeField]
    [Header("")]
    private float time;

    void Start()
    {
       StartCoroutine(Count());
    }

    IEnumerator Count()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
