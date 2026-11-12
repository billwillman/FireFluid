using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldButton : MonoBehaviour
{
	public static List<WorldButton> all = new List<WorldButton>();

	public UnityEvent OnPressed;

	public UnityEvent OnEnter;

	public UnityEvent OnExit;

	private void Start()
	{
		all.Add(this);
	}

	private void OnDestroy()
	{
		all.Remove(this);
	}
}
