using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private GameObject eyes;
	
	private Rigidbody2D _rigidbody2D;
	private Camera _camera;

	private void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		_camera = Camera.main;
	}

	private void Update()
	{
		var horizontal = Input.GetAxisRaw("Horizontal");
		transform.position += new Vector3(horizontal, 0, 0) * (40 * Time.deltaTime);
		var direction = Input.mousePosition - _camera.WorldToScreenPoint(transform.position);
		const float scale = 7.5f;
		var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		if (direction.x > 0)
		{
			transform.localScale = new Vector3(scale, scale, scale);
		}
		else
		{
			transform.localScale = new Vector3(-scale, scale, scale);
			angle += 180;
		}
		eyes.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			_rigidbody2D.AddForce(Vector2.up * 30, ForceMode2D.Impulse);
		}
	}
}
