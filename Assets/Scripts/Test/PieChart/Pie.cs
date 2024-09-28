using UnityEngine;
using UnityEngine.UI;

public class Pie : MonoBehaviour
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

	private void Update()
	{
		_image.fillAmount = _value / 180f;
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
	}
	
	public void SetValue(int value)
	{
		_value = value;
	}
	
	public void SetRotation(int value)
	{
		transform.localRotation = Quaternion.Euler(0, 0, -value);
	}
}
