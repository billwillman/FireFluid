using System;
using UnityEngine;

public class SplitOnDeath : Ability, IGameEventListener<OnCharacterDeathParams>
{
	public int numberOfSplits = 2;

	public int numberOfParts = 2;

	public float healthFactor = 0.5f;

	public float scaleFactor = 0.7f;

	public int GetOrder()
	{
		return 100;
	}

	public IGameEventResult OnGameEvent(OnCharacterDeathParams eventParameters)
	{
		if (eventParameters.victim == owner)
		{
			GameObject original = owner.gameObject;
			if (numberOfSplits > 0)
			{
				float num = UnityEngine.Random.Range(-MathF.PI, MathF.PI);
				for (int i = 0; i < numberOfParts; i++)
				{
					float f = MathF.PI * 2f / (float)numberOfParts * (float)i + num;
					Vector3 vector = base.transform.position + new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
					GameObject obj = UnityEngine.Object.Instantiate(original, vector, Quaternion.identity);
					obj.transform.localScale *= scaleFactor;
					CharacterAttributes component = obj.GetComponent<CharacterAttributes>();
					component.health.value.Push(identifier, healthFactor, ValueModifierType.Multiply);
					component.health.Fill();
					obj.GetComponentInChildren<SplitOnDeath>().numberOfSplits = numberOfSplits - 1;
					obj.GetComponent<Rigidbody>().AddForce((vector - base.transform.position).normalized * 32f, ForceMode.VelocityChange);
				}
			}
		}
		return IGameEventResult.SUCCESS;
	}

	public override void Start()
	{
		base.Start();
		GameEventManager.Get().OnCharacterDeath.AddListener(this);
	}

	private void OnDestroy()
	{
		GameEventManager.Get().OnCharacterDeath.RemoveListener(this);
	}
}
