using System.Collections.Generic;
using UnityEngine;

public class FluidSurface : MonoBehaviour
{
	private List<FluidWave> waves = new List<FluidWave>();

	private Material material;

	private List<FluidDrop> drops = new List<FluidDrop>();

	public GameObject dropPrefab;

	public Transform testTransform;

	private const int maxWaves = 4;

	private void Start()
	{
		if (material == null)
		{
			Renderer component = GetComponent<Renderer>();
			material = component.material;
			component.material = material;
		}
	}

	private void Update()
	{
		List<FluidDrop> list = new List<FluidDrop>();
		foreach (FluidDrop drop in drops)
		{
			drop.v += -base.transform.forward * Time.deltaTime;
			drop.obj.transform.position += 4f * drop.v * Time.deltaTime;
			if (Vector3.Dot(drop.obj.transform.position - base.transform.position, base.transform.forward) < 0f)
			{
				list.Add(drop);
				SpawnWave(base.transform.position + Vector3.ProjectOnPlane(drop.obj.transform.position - base.transform.position, base.transform.forward));
				SoundEffectManager.Play("sacrifice.droplet");
			}
		}
		foreach (FluidDrop item in list)
		{
			drops.Remove(item);
			Object.Destroy(item.obj);
		}
		List<FluidWave> list2 = new List<FluidWave>();
		foreach (FluidWave wave in waves)
		{
			wave.radius += Time.deltaTime;
			if ((double)wave.radius > 2.0)
			{
				list2.Add(wave);
			}
		}
		foreach (FluidWave item2 in list2)
		{
			waves.Remove(item2);
		}
		for (int i = 0; i < 4; i++)
		{
			if (i < waves.Count)
			{
				material.SetVector("_WavePosition" + i, waves[i].position);
				material.SetFloat("_WaveRadius" + i, waves[i].radius);
			}
			else
			{
				material.SetVector("_WavePosition" + i, Vector4.zero);
				material.SetFloat("_WaveRadius" + i, 1024f);
			}
		}
		if ((bool)testTransform)
		{
			material.SetVector("_WavePosition3", testTransform.position);
			material.SetFloat("_WaveRadius3", testTransform.localScale.x);
		}
	}

	public void SpawnDrop()
	{
		GameObject gameObject = Object.Instantiate(dropPrefab, base.transform);
		gameObject.transform.position = base.transform.position + Vector3.ProjectOnPlane(Random.insideUnitSphere, base.transform.forward) + base.transform.forward * 4f;
		FluidDrop fluidDrop = new FluidDrop();
		fluidDrop.obj = gameObject;
		fluidDrop.v = Vector3.zero;
		drops.Add(fluidDrop);
	}

	public void SpawnWave(Vector3 pos)
	{
		if (waves.Count >= 4)
		{
			waves.RemoveAt(0);
		}
		FluidWave fluidWave = new FluidWave();
		fluidWave.position = pos;
		fluidWave.radius = 0f;
		waves.Add(fluidWave);
	}
}
