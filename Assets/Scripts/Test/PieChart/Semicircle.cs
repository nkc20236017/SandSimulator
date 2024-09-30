using UnityEngine;
using UnityEngine.UI;

public class Semicircle : MonoBehaviour
{
	private BlockType _blockType;
	private int _value;
	private Image _image;
	
	public BlockType BlockType => _blockType;
	public int Value => _value;

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
		_blockType = BlockType.None;
		_image.fillAmount = 0;
		_image.sprite = null;
		_value = 0;
	}

	public void SetBlockType(BlockType blockType, Sprite sprite)
	{
		gameObject.SetActive(true);
		_blockType = blockType;
		_image.sprite = sprite;
		_image.fillAmount = 0;
	}
	
	public void SetValue(int value)
	{
		_value = value;
	}
	
	public void SetRotation(float value)
	{
		transform.localRotation = Quaternion.Euler(0, 0, -value);
	}
	
	public void SetFillAmount(float value)
	{
		_image.fillAmount = value;
	}
}
