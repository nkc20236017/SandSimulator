using UnityEngine;

public class SemicircleGraph : MonoBehaviour , IOutResultUI
{
	[Header("Data Config")]
	[SerializeField] private BlockDatas _blockDatas;
	[SerializeField] private Semicircle _semicirclePrefab;
	
	[Header("Semicircle Chart Config")]
	[SerializeField] private int maxSemicircleCount = 9;
	
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
	private void SemicircleGraphConfig(Sprite sprite, float ratio)
	{
		var hasSemicircle = false;
		var angle = 0f;
		
		// すでに設定されている場合は更新
		for (var i = 0; i < maxSemicircleCount; i++)
		{
			if (_semicircles[i].Sprite == sprite)
			{
				_semicircles[i].SemicircleConfig(sprite, ratio);
				hasSemicircle = true;
			}
			
			_semicircles[i].SetRotation(angle);
			var rad = _semicircles[i].FillAmount * 2f * Mathf.PI;
			var deg = rad * Mathf.Rad2Deg;
			angle += deg / 2;
		}
		
		if (hasSemicircle) { return; }
		
		// 設定されていない場合は新規追加
		for (var i = 0; i < maxSemicircleCount; i++)
		{
			if (_semicircles[i].Sprite != null) { continue; }

			_semicircles[i].SemicircleConfig(sprite, ratio);
			return;
		}
	}

    public void OutputTank(OutPutResultData outPutData)
    {
	    var ratio = outPutData.itemRatio * outPutData.totalRatio / 2;
        SemicircleGraphConfig(outPutData.Sprite, ratio);
    }
}
