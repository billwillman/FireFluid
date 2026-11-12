using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineController : MonoBehaviour
{
	private static CoroutineController _singleton;

	private static Dictionary<string, IEnumerator> _routines = new Dictionary<string, IEnumerator>(100);

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeType()
	{
		_singleton = new GameObject("#CoroutineController").AddComponent<CoroutineController>();
		Object.DontDestroyOnLoad(_singleton);
	}

	public static bool Has(string id)
	{
		IEnumerator value;
		return _routines.TryGetValue(id, out value);
	}

	public static Coroutine Start(IEnumerator routine)
	{
		return _singleton.StartCoroutine(routine);
	}

	public static Coroutine Start(IEnumerator routine, string id)
	{
		Coroutine result = _singleton.StartCoroutine(routine);
		if (!_routines.ContainsKey(id))
		{
			_routines.Add(id, routine);
			return result;
		}
		_singleton.StopCoroutine(_routines[id]);
		_routines[id] = routine;
		return result;
	}

	public static void Stop(IEnumerator routine)
	{
		_singleton.StopCoroutine(routine);
	}

	public static void Stop(string id)
	{
		if (_routines.TryGetValue(id, out var value))
		{
			_singleton.StopCoroutine(value);
			_routines.Remove(id);
		}
		else
		{
			Debug.LogWarning("coroutine '" + id + "' not found");
		}
	}

	public static void StopAll()
	{
		_singleton.StopAllCoroutines();
	}
}
