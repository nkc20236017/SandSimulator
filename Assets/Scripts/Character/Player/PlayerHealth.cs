using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamagable
{
	[Header("Health Config")]
	[SerializeField] private int maxHealth;
	[SerializeField] private int defensiveStrength;
	// [SerializeField] private float invincibilityTime;
	
	// private float _timer;
	// private bool _isInvincible;
	private int _currentHealth;

	private void Start()
	{
		_currentHealth = maxHealth;
	}

	// private void Update()
	// {
	// 	if (!_isInvincible) { return; }
	// 	
	// 	_timer -= Time.deltaTime;
	// 	if (_timer <= 0f)
	// 	{
	// 		_isInvincible = false;
	// 	}
	// }

	public void TakeDamage(int damage)
	{
		// if (_isInvincible) { return; }
		//
		// _timer = invincibilityTime;
		// _isInvincible = true;
		_currentHealth -= damage - defensiveStrength;
		if (_currentHealth > 0) { return; }

		_currentHealth = 0;
		Die();
	}
	
	public void RestoreHealth(int amount)
	{
		_currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
	}
	
	private void Die()
	{
		SceneManager.LoadScene("SampleScene");
	}
}

