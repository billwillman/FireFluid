using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public ValueStackFloat timeScale = new ValueStackFloat(1f);

	public ValueStackBool paused = new ValueStackBool(defaultValue: false);

	public float runDuration = 1200f;

	public float time;

	public ValueStackFloat currencyPerMinute = new ValueStackFloat(1f);

	private static TimeManager instance;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	public static TimeManager Get()
	{
		return instance;
	}

	public int GetSecondsOfRunLeft()
	{
		return (int)Mathf.Clamp(runDuration - time, 0f, runDuration);
	}

	private IEnumerator FreezeCoroutine(float duration)
	{
		timeScale.Push("freezeframe", 0f);
		Time.timeScale = timeScale.GetCurrentValue();
		yield return new WaitForSecondsRealtime(duration);
		timeScale.Pop("freezeframe");
		Time.timeScale = timeScale.GetCurrentValue();
	}

	public void Freeze(float duration)
	{
		StartCoroutine(FreezeCoroutine(duration));
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (paused.GetCurrentValue())
		{
			Time.timeScale = 0f;
			return;
		}
		Time.timeScale = timeScale.GetCurrentValue();
		if (RunManager.Get().state != RunState.Ongoing)
		{
			return;
		}
		float num = time;
		time += Time.deltaTime;
		if ((int)(time / 60f) > (int)(num / 60f) && num < (float)(int)runDuration)
		{
			float num2 = currencyPerMinute.GetCurrentValue() + Sacrifice.GetExtraCurrencyPerMinute();
			float num3 = num2 - (float)(int)num2;
			int num4 = (int)num2;
			if (Random.value < num3)
			{
				num4++;
			}
			PersistentDataComponent.GetDataInstance().CollectCurrency(num4);
		}
	}
}
