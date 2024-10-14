using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeMoPlayer : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            audioManager.PlaySFX("test");
        }
    }

}
