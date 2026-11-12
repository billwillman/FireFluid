using UnityEngine;

public abstract class BurdenRequirement : MonoBehaviour
{
	private Burden burden;

	public virtual void Start()
	{
		burden = GetComponent<Burden>();
	}

	public abstract bool IsMet();

	public abstract float GetProgress();

	public abstract string GetDescription();

	public abstract string GetLabelText();
}
