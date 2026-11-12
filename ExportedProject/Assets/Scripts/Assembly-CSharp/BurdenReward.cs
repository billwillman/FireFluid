using UnityEngine;

public abstract class BurdenReward : MonoBehaviour
{
	protected Burden burden;

	public virtual void Start()
	{
		burden = GetComponent<Burden>();
	}

	public abstract void Grant();

	public abstract string GetDescription();
}
