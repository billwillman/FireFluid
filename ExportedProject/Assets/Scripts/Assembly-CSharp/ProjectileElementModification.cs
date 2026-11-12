using UnityEngine;

public class ProjectileElementModification : MonoBehaviour, IProjectileModification
{
	public ValueStackFloat chanceToG = new ValueStackFloat(0f);

	public ValueStackFloat chanceToB = new ValueStackFloat(0f);

	public void OnInstantiateProjectile(Projectile p)
	{
		float currentValue = chanceToG.GetCurrentValue();
		float currentValue2 = chanceToB.GetCurrentValue();
		Vector4 element = new Vector4(1f - currentValue - currentValue2, currentValue, currentValue2, 0f);
		ElementHelper.InfuseElement(p, element);
	}
}
