using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestSpawner : MonoBehaviour
{
	public List<GameObject> pointOfInterestPrefabs = new List<GameObject>();

	public GameObject instance;

	public float minCoolDown = 10f;

	public float maxCoolDown = 60f;

	private float currentCoolDown;

	private void Start()
	{
		currentCoolDown = Random.Range(minCoolDown, maxCoolDown);
	}

	private void Update()
	{
		if (instance == null && Player.Get().GetComponentInChildren<Burden>() == null)
		{
			currentCoolDown -= Time.deltaTime;
		}
		if (currentCoolDown < 0f && TimeManager.Get().GetSecondsOfRunLeft() > 0)
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		Vector3 validPositionOnMap = PositionProvider.Get().GetValidPositionOnMap();
		GameObject original = pointOfInterestPrefabs[Random.Range(0, pointOfInterestPrefabs.Count)];
		instance = Object.Instantiate(original, validPositionOnMap, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
		currentCoolDown = Random.Range(minCoolDown, maxCoolDown);
	}
}
