using MackySoft.Navigathena.SceneManagement;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
	[SerializeField] private float invincibilityTime;
	[SerializeField] private int maxHealth;
	[SerializeField] private int maxDefence;
	//PlayerのHealthのUI
	[SerializeField]
	private HealthUI healthUI;

	private float _timer;
	private bool _isInvincible;
	private int _currentHealth;
	private int _currentDefence;

	private void Start()
	{
		_currentHealth = maxHealth;
		_currentDefence = maxDefence;
		healthUI.StartHealth(maxHealth);
	}

	private void Update()
	{
		if (!_isInvincible) { return; }
		
		_timer -= Time.deltaTime;
		if (_timer <= 0f)
		{
			_isInvincible = false;
		}
	}

	public void TakeDamage(int damage)
	{
		if (_isInvincible) { return; }
		
		_timer = invincibilityTime;
		_isInvincible = true;
		_currentHealth -= damage - _currentDefence;
		// TODO: ［効果音］プレイヤーダメージ
		AudioManager.Instance.PlaySFX("DamegeSE");
		// TODO: ［エフェクト］プレイヤーダメージ

		healthUI.UpdateHealth(_currentHealth);
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
		GlobalSceneNavigator.Instance.Push(new BuiltInSceneIdentifier("TitleScene")
			, new LoadSceneDirector(new BuiltInSceneIdentifier("LoadScene")));
	}
}

