using System.Collections.Generic;
using UnityEngine;

public class CurrencyObject : MonoBehaviour
{
	public static List<CurrencyObject> all = new List<CurrencyObject>();

	public GameObject pickUpEffectPrefab;

	public int currency;

	private Transform model;

	public string lootSound = "loot.coin";

	private void Start()
	{
		base.transform.rotation = Quaternion.Euler(-90f, Random.Range(0f, Random.Range(0f, 360f)), 0f);
		all.Add(this);
	}

	private void Update()
	{
		Player nearestPlayer = Player.GetNearestPlayer(base.transform.position);
		if (!(nearestPlayer == null) && !nearestPlayer.GetAttributes().health.IsExhausted())
		{
			float sqrMagnitude = (base.transform.position - (nearestPlayer.transform.position + Vector3.up)).sqrMagnitude;
			float currentValue = nearestPlayer.GetAttributes().pickUpRadius.GetCurrentValue();
			if (sqrMagnitude < currentValue * currentValue)
			{
				PersistentDataComponent.GetDataInstance().CollectCurrency(currency);
				SoundEffectManager.Play(lootSound, base.transform.position);
				Object.Destroy(base.gameObject);
				Object.Instantiate(pickUpEffectPrefab, base.transform.position + Vector3.up * 0.1f - Camera.main.transform.forward, Quaternion.identity);
			}
		}
	}

	private void OnDestroy()
	{
		all.Remove(this);
	}
}
