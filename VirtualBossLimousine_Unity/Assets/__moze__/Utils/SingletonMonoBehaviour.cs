using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	protected abstract bool dontDestroyOnLoad { get; }

	static T instance;

	public static bool HasInstance => instance != null;

	public static T Instance
	{
		get
		{
			if (!instance)
			{
				Type t = typeof(T);
				instance = (T) FindObjectOfType(t);
				if (!instance)
				{
					// Debug.LogError(t + " is nothing.");
				}
			}

			return instance;
		}
	}

	public static T GetOrCreateInstance(GameObject attachTo = null)
	{
		//すでにあればreturn
		if (instance)
		{
			return instance;
		}

		//なければ作成してreturn
		GameObject target;
		if (attachTo == null)
		{
			target = new GameObject();
			target.name = typeof(T).Name;
		}
		else
		{
			target = attachTo;
		}

		return target.AddComponent<T>();
	}

	protected virtual void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		if (dontDestroyOnLoad)
		{
			this.transform.parent = null;
			DontDestroyOnLoad(this.gameObject);
		}
	}
}