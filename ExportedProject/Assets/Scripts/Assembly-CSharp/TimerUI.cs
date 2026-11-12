using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
	public TextMeshProUGUI text;

	private void Update()
	{
		float num = TimeManager.Get().GetSecondsOfRunLeft();
		int num2 = (int)(num % 60f);
		int num3 = (int)(num / 60f);
		text.text = num3 + ":" + num2.ToString("D2");
	}
}
