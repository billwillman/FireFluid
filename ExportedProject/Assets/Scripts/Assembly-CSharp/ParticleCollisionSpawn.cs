using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionSpawn : MonoBehaviour
{
	private ParticleSystem particleSystem;

	private List<ParticleCollisionEvent> collisionEvents;

	public GameObject prefab;

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
	}

	private void OnParticleCollision(GameObject other)
	{
		int num = particleSystem.GetCollisionEvents(other, collisionEvents);
		for (int i = 0; i < num; i++)
		{
			Vector3 intersection = collisionEvents[i].intersection;
			if (other.layer == LayerMask.NameToLayer("Ground"))
			{
				Object.Instantiate(prefab, intersection, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
			}
		}
	}
}
