using UnityEngine;

[DefaultExecutionOrder(120)]
public class CameraShutter : MonoBehaviour
{
	[Range(0f, 1f)]
	public static float fade;

	[Range(0f, 1f)]
	public static float bars;

	public Material material;

	private void Update()
	{
		Vector2Int vector2Int = new Vector2Int(Screen.width, Screen.height);
		material.SetFloat("_AspectRatio", 1f * (float)vector2Int.x / (float)vector2Int.y);
		material.SetFloat("_Fade", fade);
		material.SetFloat("_Bars", bars);
	}
}
