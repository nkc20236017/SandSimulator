using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement Config")]
	[SerializeField] private float speed;
	[SerializeField] private float jumpForce;
	
	[Header("Ground Check")]
	[SerializeField] private float radius;
	[SerializeField] private LayerMask groundLayerMask;
	
	private CapsuleCollider2D _capsuleCollider2D;
	private Rigidbody2D _rigidbody2D;
	private Camera _camera;

	private void Awake()
	{
		_capsuleCollider2D = GetComponent<CapsuleCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		_camera = Camera.main;
	}

	private void Update()
	{
		Movement();
		Flip();
		Jump();
		IsGround();
	}

	private void Movement()
	{
		var horizontal = Input.GetAxisRaw("Horizontal");
		transform.position += new Vector3(horizontal, 0, 0) * (speed * Time.deltaTime);
	}

	private void Flip()
	{
		var direction = Input.mousePosition - _camera.WorldToScreenPoint(transform.position);
		const float scale = 7.5f;
		if (direction.x > 0)
		{
			transform.localScale = new Vector3(scale, scale, scale);
		}
		else
		{
			transform.localScale = new Vector3(-scale, scale, scale);
		}
	}

	private void Jump()
	{
		if (!IsGround()) { return; }
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			_rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		}
	}
	
	private bool IsGround()
	{
		var x = _capsuleCollider2D.bounds.center.x;
		var y = _capsuleCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		var hit = Physics2D.CircleCast(position, radius, Vector2.down, 0.1f, groundLayerMask);
		return hit.collider != null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		var capsuleCollider2D = GetComponent<CapsuleCollider2D>();
		var x = capsuleCollider2D.bounds.center.x;
		var y = capsuleCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		Gizmos.DrawWireSphere(position, radius);
	}
}
