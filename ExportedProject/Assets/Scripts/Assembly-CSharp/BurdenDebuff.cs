using UnityEngine;

public abstract class BurdenDebuff : MonoBehaviour
{
	private Burden burden;

	public virtual void Start()
	{
		burden = GetComponent<Burden>();
	}

	public abstract void Activate();

	public abstract string GetDescription();
}
