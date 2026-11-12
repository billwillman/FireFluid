using UnityEngine;

public class ExplosionHelper
{
	public static void Spawn(GameObject prefab, Vector3 position, float impactRadius, Vector4 element, CharacterAttributes attacker, int attackLayer, float aoeDamage)
	{
		if (attacker != null && aoeDamage > 0.01f)
		{
			Damage.DoAoEDamage(position, impactRadius, aoeDamage, attacker, attackLayer);
		}
		Spawn(prefab, position, impactRadius, element);
	}

	public static void Spawn(GameObject prefab, Vector3 position, float radius, Vector4 element)
	{
		if (prefab != null)
		{
			GameObject gameObject = Object.Instantiate(prefab, position, Quaternion.Euler(90f, 0f, 0f));
			Vector3 localScale = gameObject.transform.localScale * radius;
			gameObject.transform.localScale = localScale;
			ElementHelper.SetElement(gameObject, element);
		}
	}
}
