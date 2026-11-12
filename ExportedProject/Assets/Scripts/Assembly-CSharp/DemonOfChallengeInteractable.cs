using UnityEngine;

public class DemonOfChallengeInteractable : InstantiatePrefabInteractable
{
	public GameObject despawnEffectPrefab;

	public override void OnStartInteract(InteractionController c)
	{
		base.OnStartInteract(c);
		instance.GetComponent<DemonOfChallengeUI>().demonOfChallengeInteractable = this;
	}

	public void Despawn()
	{
		Object.Destroy(base.gameObject);
		Object.Instantiate(despawnEffectPrefab, base.transform.position, Quaternion.identity);
	}
}
