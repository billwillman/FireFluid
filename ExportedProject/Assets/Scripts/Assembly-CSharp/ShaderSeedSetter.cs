using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShaderSeedSetter : MonoBehaviour
{
	private void Awake()
	{
		Renderer component = GetComponent<Renderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		component.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat("_Seed", Random.value);
		component.SetPropertyBlock(materialPropertyBlock);
	}
}
