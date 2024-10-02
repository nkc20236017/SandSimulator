using UnityEngine;
using UnityEngine.UI;

public class SemicircleGraph : MonoBehaviour , IOutResultUI
{
	[Header("Data Config")]
	[SerializeField] private BlockDatas _blockDatas;
	[SerializeField] private Semicircle _semicirclePrefab;
	
	[Header("Semicircle Chart Config")]
	[SerializeField] private int maxSemicircleCount = 9;

	[SerializeField]
	private Image selectImage;
	
	private Semicircle[] _semicircles;

	private void Start()
	{
		SemicircleGenerate();
	}
	
	private void SemicircleGenerate()
	{
		_semicircles = new Semicircle[maxSemicircleCount];
		for (var i = 0; i < maxSemicircleCount; i++)
		{
			_semicircles[i] = Instantiate(_semicirclePrefab, transform);
		}
	}

	/// <summary>
	/// ブロックの設定
	/// </summary>
	/// <param name="sprite">ブロックの画像</param>
	/// <param name="ratio">ブロックの割合</param>
	private void SemicircleGraphConfig(Sprite sprite, float ratio, BlockType blockType)
	{
		var hasSemicircle = false;
		var angle = 0f;
		
		// すでに設定されている場合は更新
		for (var i = 0; i < maxSemicircleCount; i++)
		{
			if (_semicircles[i].Sprite == sprite)
			{
				_semicircles[i].SemicircleConfig(sprite, ratio, blockType);
				hasSemicircle = true;
			}
			
			_semicircles[i].SetRotation(angle);
			var rad = _semicircles[i].FillAmount * 2f * Mathf.PI;
			var deg = rad * Mathf.Rad2Deg;
			angle += deg;
		}
		
		if (hasSemicircle) { return; }
		
		// 設定されていない場合は新規追加
		for (var i = 0; i < maxSemicircleCount; i++)
		{
			if (_semicircles[i].Sprite != null) { continue; }

			_semicircles[i].SemicircleConfig(sprite, ratio, blockType);
			return;
		}
	}

    public void OutputTank(OutPutTankData outPutData)
    {
	    var ratio = outPutData.itemRatio * outPutData.totalRatio / 2;
        SemicircleGraphConfig(outPutData.Sprite, ratio, outPutData.itemType);
    }

    public void OutputSelectTank(OutPutSelectData outPutSelectData)
    {
		DefaltScale();
        for(var i = 0; i < _semicircles.Length; i++)
		{
			if(outPutSelectData.type == _semicircles[i].blockType)
			{
				Debug.Log(outPutSelectData.type);
				_semicircles[i].SelectScaleUp();
				SetImage(outPutSelectData.SelectSprite);
			}
		}
    }

	private void SetImage(Sprite sprite)
	{
		selectImage.sprite = sprite;
	}

	private void DefaltScale()
	{
        for (int i = 0; i < _semicircles.Length; i++)
        {
			_semicircles[i].DefaltScale();
        }
    }

}
