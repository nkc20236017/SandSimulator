using System;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class PlayerAnimation : MonoBehaviour
{
	[Header("Animation Config")]
	[SerializeField] private Animator _playerModelAnimator;
	[SerializeField] private Rigidbody2D _rigidbody2D;
	[SerializeField] private PlayerMovement _playerMovement;
	[SerializeField] private Transform _playerPoint;
	[SerializeField] private Transform _playerCenterPoint;
	[SerializeField] private Transform _vacuumPoint;
	
	[Header("Arm Config")]
	[SerializeField] private Transform _leftArmPoint;
	[SerializeField] private Transform _rightArmPoint;
	
	[Header("Head Config")]
	[SerializeField] private SpriteRenderer _playerHeadSpriteRenderer;
	[SerializeField] private PlayerHead[] _playerHeads;
	
	[Header("Tank Config")]
	[SerializeField] private Transform _tankPoint;
	
	[Header("Tank Settings")]
	[Tooltip("タンクの回転範囲")] [MinMaxSlider(-180, -0f)]
	[SerializeField] private Vector2 _tankRotationRange = new(-110, 0);
	[Tooltip("タンクの回転速度")] [Min(0f)]
	[SerializeField] private float _tankRotationSpeed = 2f;
	
	private static readonly int IsJump = Animator.StringToHash("isJump");
	private static readonly int XVelocity = Animator.StringToHash("xVelocity");
	private Vector3 _playerDirection;
	private PlayerHeadType _currentPlayerHeadType = PlayerHeadType.Normal;

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

	private void Update()
	{
		Animation();
		SetPlayerToVacuumDirection();
		Flip();
		HeadDirection();
		LeftArmRotation();
		RightArmRotation();
		TankAnimation();
	}
	
	private void Animation()
	{
		_playerModelAnimator.SetBool(IsJump, !_playerMovement.IsGround());
		_playerModelAnimator.SetFloat(XVelocity, Mathf.Abs(_rigidbody2D.velocity.x));
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
	
	private void TankAnimation()
	{
	    // プレイヤーの速度に基づいてタンクの位置を更新
	    Vector3 playerVelocity = _rigidbody2D.velocity;
	    float angle = Mathf.Atan2(playerVelocity.y, playerVelocity.x) * Mathf.Rad2Deg;

	    // タンクの回転範囲を制限
	    angle = Mathf.Clamp(angle, _tankRotationRange.x, _tankRotationRange.y);

	    // タンクの回転を設定
	    var euler = new Vector3(0, 0, angle);
	    Quaternion rotation = Quaternion.Euler(euler);

	    // プレイヤーの向きが反転している場合、タンクの回転も反転
	    if (_playerPoint.localScale.x < 0)
	    {
	        rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
	    }

	    _tankPoint.rotation = Quaternion.Lerp(_tankPoint.rotation, rotation, _tankRotationSpeed * Time.deltaTime);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Vector3 from = Quaternion.Euler(0, 0, _tankRotationRange.x - 90) * Vector3.right;
		Vector3 to = Quaternion.Euler(0, 0, _tankRotationRange.y - 90) * Vector3.right;
		Gizmos.DrawLine(_tankPoint.position, _tankPoint.position + from);
		Gizmos.DrawLine(_tankPoint.position, _tankPoint.position + to);
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