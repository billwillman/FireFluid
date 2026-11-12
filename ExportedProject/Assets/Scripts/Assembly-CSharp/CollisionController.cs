using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
	public List<Collider> mainColliders = new List<Collider>();

	public List<FluidEmitter> mainEmitters = new List<FluidEmitter>();

	public ValueStackBool enableCollision = new ValueStackBool(defaultValue: true);

	private bool enableCollisionPreviousFrame = true;

	private void EnableCollision(bool enable, bool modifyAll = false)
	{
		if (modifyAll)
		{
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = enable;
			}
			FluidEmitter[] componentsInChildren2 = GetComponentsInChildren<FluidEmitter>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].gameObject.SetActive(enable);
			}
			return;
		}
		foreach (Collider mainCollider in mainColliders)
		{
			mainCollider.enabled = enable;
		}
		foreach (FluidEmitter mainEmitter in mainEmitters)
		{
			mainEmitter.gameObject.SetActive(enable);
		}
	}

	public void AutoFetchReferences()
	{
		GetComponentsInChildren(mainColliders);
		GetComponentsInChildren(mainEmitters);
	}

	private void Update()
	{
		if (enableCollisionPreviousFrame && !enableCollision.GetCurrentValue())
		{
			EnableCollision(enable: false);
		}
		if (!enableCollisionPreviousFrame && enableCollision.GetCurrentValue())
		{
			EnableCollision(enable: true);
		}
		enableCollisionPreviousFrame = enableCollision.GetCurrentValue();
	}
}
