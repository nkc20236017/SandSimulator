using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	[Header("Animation Config")]
	[SerializeField] private Transform _playerPoint;
	[SerializeField] private Transform _playerCenterPoint;
	[SerializeField] private Transform _vacuumPoint;
	
	[Header("Arm Config")]
	[SerializeField] private Transform _leftArmPoint;
	[SerializeField] private Transform _rightArmPoint;
	
	[Header("Head Config")]
	[SerializeField] private SpriteRenderer _playerHeadSpriteRenderer;
	[SerializeField] private PlayerHead[] _playerHeads;
	
	[Header("Head Settings")]
	[SerializeField] private PlayerHeadType _defaultPlayerHeadType;
	
	private Vector3 _playerDirection;
	private PlayerHeadType _currentPlayerHeadType;

	/// <summary>
	/// プレイヤーの頭の種類を設定する
	/// </summary>
	public PlayerHeadType CurrentPlayerHeadType
	{
		get => _currentPlayerHeadType;
		set
		{
			_currentPlayerHeadType = value;
			_playerHeadSpriteRenderer.sprite = GetPlayerHead().upHead;
		}
	}
	
	private void Start()
	{
		CurrentPlayerHeadType = _defaultPlayerHeadType;
	}

	private void Update()
	{
		SetPlayerToVacuumDirection();
		Flip();
		HeadDirection();
		LeftArmRotation();
		RightArmRotation();
	}
	
	private void SetPlayerToVacuumDirection()
	{
		_playerDirection = _vacuumPoint.position.x > _playerCenterPoint.position.x ? Vector3.right : Vector3.left;
	}

	/// <summary>
	/// プレイヤーの向きをバキュームの向きに合わせる
	/// </summary>
	private void Flip()
	{
		Vector3 scale = _playerPoint.localScale;
		scale.x = Mathf.Abs(scale.x) * _playerDirection.x;
		_playerPoint.localScale = scale;
	}
	
	/// <summary>
	/// プレイヤーの頭の向きをバキュームの向きに合わせる
	/// </summary>
	private void HeadDirection()
	{
		if (_vacuumPoint.position.y > _playerCenterPoint.position.y)
		{
			_playerHeadSpriteRenderer.sprite = GetPlayerHead().upHead;
		}
		else
		{
			_playerHeadSpriteRenderer.sprite = GetPlayerHead().downHead;
		}
	}
	
	private PlayerHead GetPlayerHead()
	{
		return _playerHeads.FirstOrDefault(playerHead => playerHead.playerHeadType == CurrentPlayerHeadType);
	}
	
	/// <summary>
	/// 左腕の向きをバキュームの向きに合わせる
	/// </summary>
	private void LeftArmRotation()
	{
		Vector3 direction = (_vacuumPoint.position - _leftArmPoint.position).normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		int directionalityAmendment = _playerDirection == Vector3.right ? 0 : 180;
		_leftArmPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle + directionalityAmendment));
	}
	
	/// <summary>
	///	右腕の向きをバキュームの向きに合わせる
	/// </summary>
	private void RightArmRotation()
	{
		Vector3 direction = (_vacuumPoint.position - _rightArmPoint.position).normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		int directionalityAmendment = _playerDirection == Vector3.right ? 0 : 180;
		_rightArmPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle + directionalityAmendment));
	}
}

public enum PlayerHeadType
{
	Normal,
	Sad,
	Nihility,
	Detection
}

[Serializable]
public class PlayerHead
{
	public string name;
	public PlayerHeadType playerHeadType;
	[ShowAssetPreview] public Sprite downHead;
	[ShowAssetPreview] public Sprite upHead;
}