using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LayerManager : MonoBehaviour, IWorldGenerateWaitable
{
    [Header("Layer Text Config")]
    [SerializeField] private Layer _layer;
    [SerializeField] private string _riskLevelMark;
    
    [Header("Big Layer Text Config")]
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _bigLayer;
    [SerializeField] private Text _bigLayerText;
    [SerializeField] private Text _bigLayerRiskLevelText;
    
    private int _oldLayer;
    private Vector3 _defaultBigLayerPosition;
    private Transform _playerTransform;
    private Coroutine _coroutine;
    private List<int> _layerList = new();
    private IChunkInformation _chunkInformation;
    
    private void Start()
    {
        _defaultBigLayerPosition = _bigLayer.transform.position;
    }

    private void Update()
    {
        var layer = _chunkInformation.GetLayer(_playerTransform.position);
        if (layer == _oldLayer) { return; }
        if (_layerList.Contains(layer)) { return; }
        
        _oldLayer = layer;
        _layerList.Add(layer);
        var layerData = _layer.layerData[layer - 1];
        _bigLayerText.text = layerData.layerName;
        _bigLayerRiskLevelText.text = "危険度：";
        for (var i = 0; i < layerData.degreeOfRisk; i++)
        {
            _bigLayerRiskLevelText.text += _riskLevelMark;
        }
        
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(LayerTextFadeOut());
    }
    
    private IEnumerator LayerTextFadeOut()
    {
        _bigLayer.transform.position = _defaultBigLayerPosition;
        _bigLayer.transform.DOMoveX(960, _fadeTime);
        yield return new WaitForSeconds(_fadeTime);
        
        _bigLayer.transform.DOMoveX(-1000, _fadeTime);
    }

    public void OnGenerated(IChunkInformation worldMapManager)
    {
        _chunkInformation = worldMapManager;
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }
}
