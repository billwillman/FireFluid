using System.Collections.Generic;
using UnityEngine;

public class FluidSimSoundEmitterSpawner : MonoBehaviour
{
	private class FluidProbe
	{
		public Vector3 position;

		public Vector3 reaction;

		public float reactionValue;

		public float velocityValue;
	}

	public int probesPerAxis = 16;

	public int maxClusters = 8;

	public int clusteringIterations = 3;

	private FluidSimCPUMapperAsync fluid;

	private List<FluidProbe> probes = new List<FluidProbe>();

	private List<FluidProbe> clusteredProbes = new List<FluidProbe>();

	private List<FluidProbe> smoothedCluster = new List<FluidProbe>();

	public GameObject fluidSoundPrefab;

	private List<FluidSimSoundEmitter> emitters = new List<FluidSimSoundEmitter>();

	public float reactionVolume = 1f;

	public float velocityVolume = 1f;

	private void Start()
	{
		fluid = GetComponent<FluidSimCPUMapperAsync>();
	}

	private void UpdateProbes()
	{
		float num = 1f / (float)probesPerAxis;
		probes.Clear();
		Vector2 b = base.transform.position;
		if ((bool)Player.Get())
		{
			fluid.GetUvPos(Player.Get().transform.position);
		}
		float num2 = 0.6f;
		float num3 = 0.1f;
		float num4 = 0.2f;
		Vector2 vector = default(Vector2);
		for (float num5 = 0f; (double)num5 < 1.01; num5 += num)
		{
			for (float num6 = 0f; (double)num6 < 1.01; num6 += num)
			{
				vector.x = num6 + Random.value * num;
				vector.y = num5 + Random.value * num;
				float num7 = Vector2.Distance(vector, b);
				float num8 = 0.5f;
				float num9 = Mathf.Clamp01(1f - num7 / num8);
				vector = Vector2.Lerp(vector, b, num9 * num2);
				Vector3 reaction = fluid.GetReactionValueLocal(vector);
				float num10 = reaction.x + reaction.y + reaction.z;
				float magnitude = ((Vector2)fluid.GetVelocityValueLocal(vector)).magnitude;
				if (num10 * reactionVolume > num3 || magnitude * velocityVolume > num4)
				{
					FluidProbe fluidProbe = new FluidProbe();
					fluidProbe.position = fluid.GetWorldPos(vector);
					fluidProbe.reaction = reaction;
					fluidProbe.reactionValue = num10;
					fluidProbe.velocityValue = magnitude;
					probes.Add(fluidProbe);
				}
			}
		}
	}

	private void UpdateClusterProbes()
	{
		clusteredProbes.Clear();
		if (probes.Count == 0)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		HashSet<int> hashSet = new HashSet<int>();
		while (list.Count < Mathf.Min(maxClusters, probes.Count))
		{
			int num = Random.Range(0, probes.Count);
			if (hashSet.Add(num))
			{
				list.Add(probes[num].position);
			}
		}
		for (int i = 0; i < clusteringIterations; i++)
		{
			List<List<FluidProbe>> list2 = new List<List<FluidProbe>>();
			for (int j = 0; j < list.Count; j++)
			{
				list2.Add(new List<FluidProbe>());
			}
			foreach (FluidProbe probe in probes)
			{
				float num2 = float.MaxValue;
				int index = 0;
				for (int k = 0; k < list.Count; k++)
				{
					float sqrMagnitude = (probe.position - list[k]).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
						index = k;
					}
				}
				list2[index].Add(probe);
			}
			for (int l = 0; l < list.Count; l++)
			{
				if (list2[l].Count == 0)
				{
					continue;
				}
				Vector3 zero = Vector3.zero;
				foreach (FluidProbe item2 in list2[l])
				{
					zero += item2.position;
				}
				list[l] = zero / list2[l].Count;
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			if (list[m] == Vector3.zero)
			{
				continue;
			}
			float num3 = 0f;
			float num4 = 0f;
			Vector3 zero2 = Vector3.zero;
			int num5 = 0;
			foreach (FluidProbe probe2 in probes)
			{
				if ((probe2.position - list[m]).sqrMagnitude < 0.1f)
				{
					num3 += probe2.reactionValue;
					num4 += probe2.velocityValue;
					zero2 += probe2.reaction;
					num5++;
				}
			}
			if (num5 > 0)
			{
				FluidProbe item = new FluidProbe
				{
					position = list[m],
					reactionValue = num3 / (float)num5,
					velocityValue = num4 / (float)num5,
					reaction = zero2 / num5
				};
				clusteredProbes.Add(item);
			}
		}
	}

	private void UpdateSmoothedClusters()
	{
		float num = 16f;
		float t = Mathf.Clamp01(Time.deltaTime * 8f);
		float t2 = Mathf.Clamp01(Time.deltaTime * 32f);
		float t3 = Mathf.Clamp01(Time.deltaTime * 8f);
		float t4 = Mathf.Clamp01(Time.deltaTime * 8f);
		foreach (FluidProbe item in smoothedCluster)
		{
			item.velocityValue = Mathf.Lerp(item.velocityValue, 0f, t4);
		}
		foreach (FluidProbe item2 in smoothedCluster)
		{
			item2.reactionValue = Mathf.Lerp(item2.reactionValue, 0f, t4);
		}
		foreach (FluidProbe clusteredProbe in clusteredProbes)
		{
			FluidProbe fluidProbe = null;
			float num2 = float.MaxValue;
			foreach (FluidProbe item3 in smoothedCluster)
			{
				float sqrMagnitude = (clusteredProbe.position - item3.position).sqrMagnitude;
				if (sqrMagnitude < num2 && sqrMagnitude < num * num)
				{
					num2 = sqrMagnitude;
					fluidProbe = item3;
				}
			}
			if (fluidProbe != null)
			{
				fluidProbe.position = Vector3.Lerp(fluidProbe.position, clusteredProbe.position, t);
				if (clusteredProbe.reactionValue > fluidProbe.reactionValue)
				{
					fluidProbe.reactionValue = Mathf.Lerp(fluidProbe.reactionValue, clusteredProbe.reactionValue, t2);
					fluidProbe.reaction = Vector3.Lerp(fluidProbe.reaction, clusteredProbe.reaction, t2);
				}
				else
				{
					fluidProbe.reactionValue = Mathf.Lerp(fluidProbe.reactionValue, clusteredProbe.reactionValue, t3);
					fluidProbe.reaction = Vector3.Lerp(fluidProbe.reaction, clusteredProbe.reaction, t3);
				}
				if (clusteredProbe.velocityValue > fluidProbe.velocityValue)
				{
					fluidProbe.velocityValue = Mathf.Lerp(fluidProbe.velocityValue, clusteredProbe.velocityValue, t2);
				}
				else
				{
					fluidProbe.velocityValue = Mathf.Lerp(fluidProbe.velocityValue, clusteredProbe.velocityValue, t3);
				}
			}
			else
			{
				smoothedCluster.Add(new FluidProbe
				{
					position = clusteredProbe.position,
					reaction = clusteredProbe.reaction,
					reactionValue = Mathf.Lerp(0f, clusteredProbe.reactionValue, t),
					velocityValue = Mathf.Lerp(0f, clusteredProbe.velocityValue, t)
				});
			}
		}
		smoothedCluster.RemoveAll((FluidProbe c) => c.reactionValue < 0.01f && c.velocityValue < 0.01f);
	}

	private void Update()
	{
		if ((double)Time.timeScale < 0.1)
		{
			foreach (FluidSimSoundEmitter emitter in emitters)
			{
				emitter.reactionSourceR.volume *= 0.9f;
				emitter.reactionSourceG.volume *= 0.9f;
				emitter.reactionSourceB.volume *= 0.9f;
				emitter.velocitySource.volume *= 0.9f;
			}
			return;
		}
		UpdateProbes();
		UpdateClusterProbes();
		UpdateSmoothedClusters();
		while (emitters.Count < smoothedCluster.Count)
		{
			FluidSimSoundEmitter component = Object.Instantiate(fluidSoundPrefab).GetComponent<FluidSimSoundEmitter>();
			component.reactionSourceR.time = Random.Range(0f, component.reactionSourceR.clip.length);
			component.reactionSourceG.time = Random.Range(0f, component.reactionSourceG.clip.length);
			component.reactionSourceB.time = Random.Range(0f, component.reactionSourceB.clip.length);
			component.velocitySource.time = Random.Range(0f, component.velocitySource.clip.length);
			emitters.Add(component);
		}
		for (int i = 0; i < smoothedCluster.Count; i++)
		{
			FluidProbe fluidProbe = smoothedCluster[i];
			FluidSimSoundEmitter fluidSimSoundEmitter = emitters[i];
			fluidSimSoundEmitter.gameObject.SetActive(value: true);
			fluidSimSoundEmitter.transform.position = fluidProbe.position;
			Vector3 normalized = fluidProbe.reaction.normalized;
			fluidSimSoundEmitter.reactionSourceR.volume = Mathf.Clamp01(fluidProbe.reactionValue * normalized.x * reactionVolume * Settings.fxVolume);
			fluidSimSoundEmitter.reactionSourceG.volume = Mathf.Clamp01(fluidProbe.reactionValue * normalized.y * reactionVolume * Settings.fxVolume);
			fluidSimSoundEmitter.reactionSourceB.volume = Mathf.Clamp01(fluidProbe.reactionValue * normalized.z * reactionVolume * Settings.fxVolume);
			fluidSimSoundEmitter.velocitySource.volume = Mathf.Clamp01(fluidProbe.velocityValue * velocityVolume * Settings.fxVolume);
			fluidSimSoundEmitter.velocitySource.pitch = 0.9f + fluidProbe.velocityValue * 1f;
		}
		for (int j = smoothedCluster.Count; j < emitters.Count; j++)
		{
			emitters[j].gameObject.SetActive(value: false);
		}
	}

	private float GetGizmoRadius(FluidProbe probe)
	{
		return (probe.reactionValue * reactionVolume + probe.velocityValue * velocityVolume) * 2f;
	}

	private Color GetGizmoColor(FluidProbe probe)
	{
		return Color.Lerp(Color.blue, Color.red, probe.reactionValue * reactionVolume / (probe.reactionValue * reactionVolume + probe.velocityValue * velocityVolume + 0.001f));
	}

	private void OnDrawGizmos()
	{
		foreach (FluidProbe probe in probes)
		{
			Gizmos.color = GetGizmoColor(probe);
			Gizmos.DrawWireSphere(probe.position, GetGizmoRadius(probe));
		}
		if (clusteredProbes != null)
		{
			foreach (FluidProbe clusteredProbe in clusteredProbes)
			{
				Gizmos.color = GetGizmoColor(clusteredProbe);
				Gizmos.DrawWireSphere(clusteredProbe.position, GetGizmoRadius(clusteredProbe) * 2f);
			}
		}
		if (smoothedCluster == null)
		{
			return;
		}
		foreach (FluidProbe item in smoothedCluster)
		{
			Gizmos.color = GetGizmoColor(item);
			Gizmos.DrawSphere(item.position, GetGizmoRadius(item) * 2f);
		}
	}
}
