using UnityEngine;

public class SetShaderUniformLightPos : MonoBehaviour
{
	private void Update()
	{
		Shader.SetGlobalVector("_LightWorldPos", base.transform.position);
	}
}
