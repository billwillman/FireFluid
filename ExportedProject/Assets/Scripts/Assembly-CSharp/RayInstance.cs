using System.Collections.Generic;
using UnityEngine;

public class RayInstance : MonoBehaviour
{
	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	private int numberOfPositions = 16;

	private Vector3[] positions;

	private Vector2[] offsets;

	private Vector3[] smoothedPositions;

	private RaycastHit[] hits = new RaycastHit[128];

	private List<CharacterAttributes> targets = new List<CharacterAttributes>();

	public CooldownTracker<CharacterAttributes> cooldownTracker = new CooldownTracker<CharacterAttributes>(0.5f);

	private static Color hitHighlightColor = new Color(16f, 0.5f, 0f, 1f);

	private Vector3 start;

	private Vector3 end;

	private int endPointIndex;

	private Vector3 dir;

	private float travelLength;

	public ParticleSystem startParticles;

	public ParticleSystem endParticles;

	public FluidEmitter endEmitter;

	public bool currentlyActive;

	public RayAttack rayAttack;

	private void Start()
	{
		positions = new Vector3[numberOfPositions];
		offsets = new Vector2[numberOfPositions];
		smoothedPositions = new Vector3[numberOfPositions];
		GetComponentsInChildren(lineRenderers);
		UpdateActive();
	}

	private void RecalculateTravel()
	{
		dir = end - start;
		dir.y = 0f;
		travelLength = dir.magnitude;
		dir /= travelLength;
	}

	private void GenerateRayPoints(Vector3 start, Vector3 dir)
	{
		Vector3 vector = start;
		Vector3 vector2 = dir.normalized;
		float num = rayAttack.range.GetCurrentValue() / (float)(positions.Length - 1);
		for (int i = 0; i < positions.Length; i++)
		{
			positions[i] = vector;
			Collider[] array = Physics.OverlapSphere(vector, rayAttack.bendRadius.GetCurrentValue(), LayerHelper.LAYERMASK_ENEMY);
			Vector3 zero = Vector3.zero;
			float num2 = 0f;
			Collider[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				Vector3 vector3 = array2[j].bounds.center - vector;
				vector3.y = 0f;
				if (!(Vector3.Dot(vector2, vector3.normalized) < 0f))
				{
					float magnitude = vector3.magnitude;
					if (!(magnitude < 0.001f))
					{
						float num3 = 1f - magnitude / rayAttack.bendRadius.GetCurrentValue();
						zero += vector3.normalized * num3;
						num2 += num3;
					}
				}
			}
			if (num2 > 0f)
			{
				vector2 = Vector3.Slerp(vector2, (zero / num2).normalized, rayAttack.bendStrength);
			}
			vector += vector2 * num;
		}
	}

	private void UpdateSmoothedPositions(float hardness, Vector3 start, Vector3 dir)
	{
		Vector3 rhs = dir;
		rhs.y = 0f;
		rhs.Normalize();
		Vector3 rhs2 = new Vector3(0f - dir.z, 0f, dir.x);
		for (int i = 0; i < numberOfPositions; i++)
		{
			offsets[i].x = Mathf.Lerp(offsets[i].x, Vector3.Dot(positions[i] - start, rhs), hardness);
			offsets[i].y = Mathf.Lerp(offsets[i].y, Vector3.Dot(positions[i] - start, rhs2), hardness);
			smoothedPositions[i].x = start.x + rhs.x * offsets[i].x + rhs2.x * offsets[i].y;
			smoothedPositions[i].y = start.y;
			smoothedPositions[i].z = start.z + rhs.z * offsets[i].x + rhs2.z * offsets[i].y;
		}
	}

	public void UpdateState(Vector3 start, Vector3 dir)
	{
		GenerateRayPoints(start, dir);
		UpdateSmoothedPositions(Time.deltaTime * 8f, start, dir);
		this.start = start;
		end = smoothedPositions[positions.Length - 1];
		endPointIndex = positions.Length - 2;
		targets.Clear();
		HashSet<CharacterAttributes> hashSet = new HashSet<CharacterAttributes>();
		bool flag = false;
		for (int i = 0; i < smoothedPositions.Length - 1; i++)
		{
			if (flag)
			{
				break;
			}
			Vector3 vector = smoothedPositions[i];
			Vector3 direction = smoothedPositions[i + 1] - vector;
			float magnitude = direction.magnitude;
			if (magnitude < 0.001f)
			{
				continue;
			}
			direction /= magnitude;
			int num = Physics.RaycastNonAlloc(vector, direction, hits, magnitude, LayerHelper.LAYERMASK_ENVIRONMENT | LayerHelper.LAYERMASK_ENEMY | LayerHelper.LAYERMASK_ENVIRONMENTDESTRUCTABLE);
			for (int j = 0; j < num; j++)
			{
				RaycastHit raycastHit = hits[j];
				CharacterAttributes componentInParent = raycastHit.collider.GetComponentInParent<CharacterAttributes>();
				if ((bool)componentInParent)
				{
					if (hashSet.Add(componentInParent))
					{
						targets.Add(componentInParent);
					}
					if (!rayAttack.piercing.GetCurrentValue())
					{
						end = raycastHit.point;
						flag = true;
						endPointIndex = i;
						break;
					}
					continue;
				}
				end = raycastHit.point;
				flag = true;
				endPointIndex = i;
				break;
			}
		}
		RecalculateTravel();
		cooldownTracker.cooldown = rayAttack.secondsPerTick / rayAttack.GetOwner().attackSpeed.GetCurrentValue();
		float dmg = rayAttack.GetOwner().damage.GetCurrentValue() * rayAttack.damageMultiplier.GetCurrentValue();
		foreach (CharacterAttributes target in targets)
		{
			if (!cooldownTracker.IsOnCooldown(target))
			{
				cooldownTracker.ExhaustCooldown(target);
				if (target.ReceiveDamage(dmg, rayAttack.GetOwner(), rayAttack.GetIdentifier()) > 0.01f)
				{
					RendererEffects.Highlight(target.gameObject, rayAttack.GetIdentifier(), hitHighlightColor, 0.075f);
				}
			}
		}
	}

	public void UpdateActive()
	{
		if (currentlyActive)
		{
			foreach (LineRenderer lineRenderer in lineRenderers)
			{
				lineRenderer.positionCount = endPointIndex + 1;
				lineRenderer.gameObject.SetActive(value: true);
				lineRenderer.SetPositions(smoothedPositions);
				lineRenderer.textureScale = new Vector2((start - end).magnitude, 1f);
			}
			startParticles.transform.position = start;
			startParticles.transform.forward = dir;
			endParticles.transform.position = smoothedPositions[endPointIndex];
			ParticleSystem.EmissionModule emission = startParticles.emission;
			emission.enabled = true;
			emission = endParticles.emission;
			emission.enabled = true;
			ElementHelper.SetElement(base.gameObject, Player.Get().GetAttributes().GetElement() * 0.25f);
			endEmitter.transform.position = end;
			endEmitter.reaction = Player.Get().GetAttributes().GetElement() * 0.25f;
			endEmitter.noiseForce = 0.5f;
			endEmitter.temperature = 0.3f;
			return;
		}
		foreach (LineRenderer lineRenderer2 in lineRenderers)
		{
			lineRenderer2.gameObject.SetActive(value: false);
		}
		ParticleSystem.EmissionModule emission2 = startParticles.emission;
		emission2.enabled = false;
		emission2 = endParticles.emission;
		emission2.enabled = false;
		endEmitter.reaction = Vector3.zero;
		endEmitter.noiseForce = 0f;
		endEmitter.temperature = 0f;
	}

	public void Update()
	{
		if (positions.Length != numberOfPositions)
		{
			positions = new Vector3[numberOfPositions];
			offsets = new Vector2[numberOfPositions];
			smoothedPositions = new Vector3[numberOfPositions];
		}
		cooldownTracker.Update(Time.deltaTime);
		UpdateActive();
	}
}
