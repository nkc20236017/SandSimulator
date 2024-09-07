using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    private Camera _camera;
    private float _width = 48f;
    private float _height = 85f;

    private void Start()
    {
        _camera = Camera.main;
        if (_camera == null) { return; }
        
        _camera.orthographic = true;
        _camera.orthographicSize = _height / 2;

        var targetAspect = 16f / 9f;
        var windowAspect = (float)Screen.width / (float)Screen.height;
        var scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            var rect = _camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            _camera.rect = rect;
        }

        _camera.transform.position = new Vector3(_width / 2, _height / 2, -10f);
    }
}
