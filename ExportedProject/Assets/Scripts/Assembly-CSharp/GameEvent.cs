using System.Collections.Generic;
using UnityEngine;

public class GameEvent<T>
{
	public const int ORDER_READONSUCCESS = 1000;

	private List<IGameEventListener<T>> listeners = new List<IGameEventListener<T>>();

	public void AddListener(IGameEventListener<T> listener)
	{
		listeners.Add(listener);
		for (int num = listeners.Count - 1; num > 0; num--)
		{
			if (listeners[num].GetOrder() < listeners[num - 1].GetOrder())
			{
				IGameEventListener<T> value = listeners[num];
				listeners[num] = listeners[num - 1];
				listeners[num - 1] = value;
			}
		}
	}

	public void RemoveListener(IGameEventListener<T> listener)
	{
		listeners.Remove(listener);
	}

	public IGameEventResult Invoke(T eventParams)
	{
		if (!Application.isPlaying)
		{
			return IGameEventResult.PREVENTED;
		}
		foreach (IGameEventListener<T> listener in listeners)
		{
			if (listener.OnGameEvent(eventParams) == IGameEventResult.PREVENTED)
			{
				return IGameEventResult.PREVENTED;
			}
		}
		return IGameEventResult.SUCCESS;
	}
}
