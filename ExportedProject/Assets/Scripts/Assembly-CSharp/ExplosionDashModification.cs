using UnityEngine;

public class ExplosionDashModification : MonoBehaviour, IDashModification
{
	public GameObject explosionPrefab;

	public ValueStackFloat damageMultiplier = new ValueStackFloat(2f);

	public ValueStackFloat impactRadius = new ValueStackFloat(4f);

	private CharacterAttributes owner;

	private void Start()
	{
		owner = GetComponentInParent<CharacterAttributes>();
	}

	public void OnEndDashing()
	{
	}

	public void OnStartDashing()
	{
	}

	public void OnStartRunning()
	{
	}

	public void OnEndRunning()
	{
		float num = damageMultiplier.GetCurrentValue();
		if ((bool)owner)
		{
			num *= owner.damage.GetCurrentValue();
		}
		ExplosionHelper.Spawn(explosionPrefab, base.transform.position, impactRadius.GetCurrentValue() * Mathf.Sqrt(owner.area.GetCurrentValue()), Player.Get().GetAttributes().GetElement(), owner, LayerMask.NameToLayer("PlayerAttack"), num);
	}
}
