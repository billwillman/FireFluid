using System.Collections.Generic;
using UnityEngine;

public class DynamicScalingAttributeUpdater : MonoBehaviour
{
	public List<DynamicScalingValueStack> dynamicScalingValueStacks = new List<DynamicScalingValueStack>();

	private void Update()
	{
		foreach (DynamicScalingValueStack dynamicScalingValueStack in dynamicScalingValueStacks)
		{
			dynamicScalingValueStack.UpdateValue();
		}
	}
}
