using UnityEngine;

public class CameraShakeDemo : MonoBehaviour
{
    [SerializeField]
    private float shakeTime;

    private ICameraEffect cameraShake;

    private void Start()
    {
        cameraShake = GetComponent<ICameraEffect>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraShake.CameraShake(shakeTime);
        }
    }
}