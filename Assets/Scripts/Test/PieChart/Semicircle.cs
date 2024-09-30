using UnityEngine;
using UnityEngine.UI;

public class Semicircle : MonoBehaviour
{
	private Image _image;
	
	public Sprite Sprite => _image.sprite;
	public float FillAmount => _image.fillAmount;

	private void Awake()
	{
		_image = GetComponent<Image>();
	}

	private void Start()
	{
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
	public void SemicircleConfig(Sprite sprite, float value)
	{
		if (value <= 0)
		{
			ClearPie();
			return;
		}
		
		gameObject.SetActive(true);
		_image.sprite = sprite;
		_image.fillAmount = value;
	}
	
	/// <summary>
	/// 角度の設定
	/// </summary>
	/// <param name="angle">角度</param>
	public void SetRotation(float angle)
	{
		transform.localRotation = Quaternion.Euler(0, 0, -angle);
	}
}
