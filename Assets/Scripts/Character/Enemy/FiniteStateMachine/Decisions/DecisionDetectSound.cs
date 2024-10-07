using System.Collections;
using UnityEngine;

public class DecisionDetectSound : FsmDecision
{
	[Header("Detect Sound Config")]
	[SerializeField] private Transform pivot;
	[SerializeField] private float radius;
	[SerializeField] private LayerMask targetLayerMask;
	
	[Header("Mark Config")]
	[SerializeField] private SpriteRenderer markSpriteRenderer;
	[SerializeField] private Sprite findMarkSprite;
	[SerializeField] private Sprite lostMarkSprite;
	[SerializeField] private float showMarkTime;
	
	[Header("Debug Config")]
	[SerializeField] private bool debugMode;
	
	private bool _isTargetDetected;
	private Coroutine _markCoroutine;
	private EnemyBrain _enemyBrain;

	private void OnEnable()
	{
		_enemyBrain = GetComponent<EnemyBrain>();
	}

	public override bool Decide()
	{
		if (markSpriteRenderer.gameObject.activeSelf)
		{
			markSpriteRenderer.flipX = transform.localScale.x < 0;
		}
		DetectPlayer();
		return _isTargetDetected;
	}
	
	private void DetectPlayer()
	{
		// TODO: デバッグモードの時はプレイヤーを検知しないようにする
		
		var targetCollider = Physics2D.OverlapCircleAll(pivot.position, radius, targetLayerMask);
		if (targetCollider.Length > 0)
		{
			foreach (var target in targetCollider)
			{
				if (!_isTargetDetected)
				{
					_isTargetDetected = true;
					_enemyBrain.Target = target.transform;
					_enemyBrain.TargetPosition = target.transform.position;
					ShowMark(findMarkSprite);
				}
			}
		}
		else
		{
			if (_isTargetDetected)
			{
				_enemyBrain.Target = null;
				_isTargetDetected = false;
				ShowMark(lostMarkSprite);
			}
		}
	}
	
	private void ShowMark(Sprite mark)
	{
		markSpriteRenderer.gameObject.SetActive(true);
		markSpriteRenderer.sprite = mark;
		if (_markCoroutine != null)
		{
			StopCoroutine(_markCoroutine);
		}
		
		_markCoroutine = StartCoroutine(HideMark());
	}
        
	private IEnumerator HideMark()
	{
		yield return new WaitForSeconds(showMarkTime);
        
		markSpriteRenderer.sprite = null;
		markSpriteRenderer.gameObject.SetActive(false);
	}

	private void OnDrawGizmos()
	{
		if (!debugMode) { return; }
		if (pivot == null) { return; }
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(pivot.position, radius);
		
		if (_enemyBrain == null) { return; }
		if (_enemyBrain.Target == null) { return; }
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(pivot.position, _enemyBrain.Target.position);
	}
}

