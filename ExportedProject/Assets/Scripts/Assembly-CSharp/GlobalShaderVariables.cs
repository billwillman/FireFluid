using UnityEngine;

public class GlobalShaderVariables : MonoBehaviour
{
	private void Update()
	{
		Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
	}
}
