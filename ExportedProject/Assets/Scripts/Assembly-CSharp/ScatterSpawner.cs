using System.Collections.Generic;
using UnityEngine;

public class ScatterSpawner : Projectile
{
	public GameObject spawnPrefab;

	public int numberOfSpawns = 20;

	public float spawnDelay = 0.5f;

	public float scatterRadius = 32f;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	private void Start()
	{
		Vector3 position = base.transform.position;
		for (int i = 0; i < numberOfSpawns; i++)
		{
			cinematicMachine.PushAction(new CinematicFunction(delegate
			{
				Vector2 vector = Random.insideUnitCircle * scatterRadius;
				Projectile component = Object.Instantiate(spawnPrefab, position + new Vector3(vector.x, 0f, vector.y), Quaternion.identity).GetComponent<Projectile>();
				if ((bool)component)
				{
					component.Initialize(owner, Vector3.zero);
					List<IProjectileModification> list = new List<IProjectileModification>();
					GetComponentsInChildren(list);
					foreach (IProjectileModification item in list)
					{
						item.OnInstantiateProjectile(component);
					}
				}
				return true;
			}));
			cinematicMachine.PushAction(new CinematicWait(spawnDelay));
		}
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			Object.Destroy(base.gameObject);
			return true;
		}));
	}

	private new void Update()
	{
		cinematicMachine.UpdateActions(Time.deltaTime);
	}
}
