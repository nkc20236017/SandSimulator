using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}

			var t = typeof(T);
			_instance = (T)FindObjectOfType(t);
			if (_instance == null)
			{
				Debug.LogError(t + " is no component anywhere.");
			}

			return _instance;
		}
	}

	protected virtual void Awake()
	{
		CheckInstance();
	}

	private void CheckInstance()
	{
		if (_instance == null)
		{
			_instance = this as T;
			return;
		}

		if (Instance == this)
		{
			return;
		}

		Destroy(this);
	}
}
