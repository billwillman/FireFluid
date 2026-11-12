using UnityEngine;

public class DemonOfChanceInteractable : InstantiatePrefabInteractable
{
	public GameObject despawnEffectPrefab;

	public override void OnStartInteract(InteractionController c)
	{
		base.OnStartInteract(c);
		instance.GetComponent<DemonOfChanceUI>().demonOfChanceInteractable = this;
	}

	public void Despawn()
	{
		Object.Destroy(base.gameObject);
		Object.Instantiate(despawnEffectPrefab, base.transform.position, Quaternion.identity);
	}
}
