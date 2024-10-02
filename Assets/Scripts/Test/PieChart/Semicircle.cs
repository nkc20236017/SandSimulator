using UnityEngine;
using UnityEngine.UI;

public class Semicircle : MonoBehaviour
{
	private Image _image;
	private Vector2 defaltScale;
	public Sprite Sprite => _image.sprite;
	public float FillAmount => _image.fillAmount;
	public BlockType blockType = BlockType.None;

	private void Awake()
	{
		_image = GetComponent<Image>();
	}

	private void Start()
	{
		defaltScale = transform.localScale;
		ClearPie();
	}

	public void ClearPie()
	{
		gameObject.SetActive(false);
		_image.sprite = null;
		_image.fillAmount = 0;
	}

	/// <summary>
	/// 個別のブロックの設定
	/// </summary>
	/// <param name="sprite">ブロックの画像</param>
	/// <param name="value">ブロックの割合</param>
	public void SemicircleConfig(Sprite sprite, float value ,BlockType blockType)
	{
		if (value <= 0)
		{
			ClearPie();
			return;
		}
		
		gameObject.SetActive(true);
		_image.sprite = sprite;
		_image.fillAmount = value;
		this.blockType = blockType;
	}
	
	/// <summary>
	/// 角度の設定
	/// </summary>
	/// <param name="angle">角度</param>
	public void SetRotation(float angle)
	{
		transform.localRotation = Quaternion.Euler(0, 0, -angle);
	}

	//Imageのスケールアップ
	public void SelectScaleUp()
	{
		_image.transform.localScale = new Vector2(1,1);
	}

	public void DefaltScale()
	{
		_image.transform.localScale = defaltScale;
	}

}
