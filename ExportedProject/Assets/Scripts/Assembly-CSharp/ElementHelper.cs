using UnityEngine;

public class ElementHelper
{
	public static void SetElement(GameObject obj, Vector4 element)
	{
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.sharedMaterial.shader.name == "Custom/AnimeFire")
			{
				renderer.material.SetVector("_GradientWeights", element);
			}
			else if (renderer.sharedMaterial.HasColor("_ReactionScale"))
			{
				renderer.material.SetVector("_ReactionScale", element);
			}
		}
		FluidEmitter[] componentsInChildren2 = obj.GetComponentsInChildren<FluidEmitter>();
		foreach (FluidEmitter fluidEmitter in componentsInChildren2)
		{
			float num = fluidEmitter.reaction.x + fluidEmitter.reaction.y + fluidEmitter.reaction.z;
			fluidEmitter.reaction.x = num * element.x;
			fluidEmitter.reaction.y = num * element.y;
			fluidEmitter.reaction.z = num * element.z;
		}
	}

	public static void InfuseElement(Projectile projectile, Vector4 element)
	{
		Vector4 element2 = InfuseElement(projectile.element, element);
		SetElement(projectile.gameObject, element2);
		projectile.element = element2;
	}

	public static void InfuseElement(Claw claw, Vector4 element)
	{
		Vector4 element2 = InfuseElement(claw.element, element);
		SetElement(claw.gameObject, element2);
		claw.element = element2;
	}

	public static Vector4 InfuseElement(Vector4 target, Vector4 infuse, bool binaryRoll = true)
	{
		if (binaryRoll)
		{
			infuse.y = ((Random.value < infuse.y) ? 1f : 0f);
			infuse.z = ((Random.value < infuse.z) ? 1f : 0f);
		}
		target = Vector4.Max(target, infuse);
		target.x = Mathf.Clamp01(1f - target.y - target.z);
		target.Normalize();
		return target;
	}
}
