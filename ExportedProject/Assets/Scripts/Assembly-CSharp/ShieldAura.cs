using UnityEngine;

public class ShieldAura : MonoBehaviour
{
	public ValueStackFloat pushIntensity = new ValueStackFloat(8f);

	public ValueStackFloat radius = new ValueStackFloat(8f);

	private Shield shield;

	private FluidEmitter emitter;

	private void Start()
	{
		emitter = GetComponentInChildren<FluidEmitter>(includeInactive: true);
	}

	private float GetRadius()
	{
		return base.transform.localScale.x * radius.GetCurrentValue();
	}

	private void Update()
	{
		if (shield == null)
		{
			shield = GetComponentInParent<Shield>(includeInactive: true);
		}
		if (emitter == null)
		{
			return;
		}
		if (shield == null || !shield.CurrentlyTriggering())
		{
			emitter.gameObject.SetActive(value: false);
			return;
		}
		emitter.gameObject.SetActive(value: true);
		Player player = Player.Get();
		Vector3 element = shield.GetElement();
		foreach (Enemy item in Enemy.all)
		{
			Vector3 vector = item.transform.position - base.transform.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			float num = GetRadius();
			if (magnitude < num)
			{
				Vector3 vector2 = new Vector3(vector.z, 0f, 0f - vector.x);
				item.transform.position -= vector2 / magnitude * Mathf.Clamp01(1f - magnitude / num) * Time.deltaTime * pushIntensity.GetCurrentValue();
				if (element.y > 0.01f)
				{
					StatusEffectManager.Apply(StatusEffectManager.poison, item.GetAttributes(), player.GetAttributes());
				}
				if (element.z > 0.01f)
				{
					StatusEffectManager.Apply(StatusEffectManager.frost, item.GetAttributes(), player.GetAttributes());
				}
			}
		}
	}
}
