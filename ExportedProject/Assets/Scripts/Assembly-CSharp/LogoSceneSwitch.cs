using UnityEngine;
using UnityEngine.UI;

public class LogoSceneSwitch : MonoBehaviour
{
	public float minWaitTime = 5f;

	private bool transition;

	private bool sceneSwitched;

	public Image shutter;

	public int blackFrames = 2;

	private void Update()
	{
		blackFrames--;
		if (blackFrames > 0)
		{
			Color color = shutter.color;
			color.a = 1f;
			shutter.color = color;
			return;
		}
		minWaitTime -= Time.deltaTime;
		if (!transition && minWaitTime < 0f && StringLocalization.IsLoaded())
		{
			transition = true;
		}
		if (transition)
		{
			Color color2 = shutter.color;
			color2.a = Mathf.Clamp01(color2.a + Time.deltaTime);
			shutter.color = color2;
			if (!sceneSwitched && (double)color2.a > 0.99)
			{
				SceneManager.Get().LoadScene(SceneIndex.STARTSCREEN);
				sceneSwitched = true;
			}
		}
		else
		{
			Color color3 = shutter.color;
			color3.a = Mathf.Clamp01(color3.a - Time.deltaTime * 0.5f);
			shutter.color = color3;
		}
	}
}
