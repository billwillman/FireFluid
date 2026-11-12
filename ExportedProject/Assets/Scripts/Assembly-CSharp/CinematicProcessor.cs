using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class CinematicProcessor : MonoBehaviour
{
	private static CinematicProcessor instance;

	private List<CinematicMachine> cinematicMachines = new List<CinematicMachine>();

	private List<CinematicMachine> disposedMachines = new List<CinematicMachine>();

	private bool inCinematic;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Object.Destroy(instance);
			instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		inCinematic = false;
		foreach (CinematicMachine cinematicMachine in cinematicMachines)
		{
			inCinematic |= cinematicMachine.isCinematic;
			float deltaTime = Time.deltaTime;
			cinematicMachine.UpdateActions(deltaTime);
		}
		List<CinematicMachine> list = new List<CinematicMachine>();
		foreach (CinematicMachine disposedMachine in disposedMachines)
		{
			if (!disposedMachine.IsBusy())
			{
				list.Add(disposedMachine);
			}
		}
		foreach (CinematicMachine item in list)
		{
			disposedMachines.Remove(item);
			cinematicMachines.Remove(item);
		}
	}

	public static CinematicProcessor GetInstance()
	{
		return instance;
	}

	public CinematicMachine CreateCinematicMachine()
	{
		CinematicMachine cinematicMachine = new CinematicMachine();
		cinematicMachines.Add(cinematicMachine);
		return cinematicMachine;
	}

	public void DisposeCinematicMachine(CinematicMachine cm)
	{
		disposedMachines.Add(cm);
	}

	public bool IsInCinematic()
	{
		return inCinematic;
	}
}
