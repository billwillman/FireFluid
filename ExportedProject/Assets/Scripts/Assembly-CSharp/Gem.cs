using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
	public static List<Gem> all = new List<Gem>();

	public float experience;

	public float collectionSpeed = 8f;

	private bool attracted;

	public float rotationSpeed = 90f;

	private Transform model;

	public string lootSound;

	public const float MERGE_DISTANCE_SQRT_PERF = 64f;

	public const float OUT_OF_SCREEN_DISTANCE_SQRT_PERF = 576f;

	public const float MERGE_DISTANCE_SQRT = 16f;

	public const float OUT_OF_SCREEN_DISTANCE_SQRT = 1024f;

	private void Start()
	{
		model = base.transform.GetChild(0);
		all.Add(this);
	}

	private void Update()
	{
		model.transform.rotation *= Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
		Player nearestPlayer = Player.GetNearestPlayer(base.transform.position);
		if (nearestPlayer == null || nearestPlayer.GetAttributes().health.IsExhausted())
		{
			return;
		}
		Vector3 vector = base.transform.position - (nearestPlayer.transform.position + Vector3.up);
		float sqrMagnitude = vector.sqrMagnitude;
		float currentValue = nearestPlayer.GetAttributes().pickUpRadius.GetCurrentValue();
		if (!attracted && sqrMagnitude < currentValue * currentValue)
		{
			attracted = true;
			OnGemAttractedParams onGemAttractedParams = new OnGemAttractedParams();
			onGemAttractedParams.collector = nearestPlayer.GetAttributes();
			onGemAttractedParams.gem = this;
			GameEventManager.Get().OnGemAttracted.Invoke(onGemAttractedParams);
		}
		if (attracted)
		{
			base.transform.position -= vector.normalized * collectionSpeed * Time.deltaTime;
			if (sqrMagnitude < 0.5f)
			{
				OnGemCollectedParams onGemCollectedParams = new OnGemCollectedParams();
				onGemCollectedParams.collector = nearestPlayer.GetAttributes();
				onGemCollectedParams.xp = experience;
				if (GameEventManager.Get().OnGemCollected.Invoke(onGemCollectedParams) == IGameEventResult.SUCCESS)
				{
					nearestPlayer.GetAttributes().GainXP(onGemCollectedParams.xp);
					SoundEffectManager.Play(lootSound, base.transform.position);
					Object.Destroy(base.gameObject);
				}
			}
		}
		float num = (Settings.performanceMode ? 576f : 1024f);
		float num2 = (Settings.performanceMode ? 64f : 16f);
		if (!((double)Random.value < 0.01) || !(sqrMagnitude > num))
		{
			return;
		}
		foreach (Gem item in all)
		{
			if (!(item == this))
			{
				sqrMagnitude = (item.transform.position - base.transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					base.transform.position = (item.transform.position + base.transform.position) * 0.5f;
					experience += item.experience;
					base.transform.localScale += item.transform.localScale * 0.1f;
					Object.Destroy(item.gameObject);
					break;
				}
			}
		}
	}

	private void OnDestroy()
	{
		all.Remove(this);
	}

	public void Collect()
	{
		attracted = true;
	}
}
