using UnityEngine;

public class DestroyOnEnemyLimit : MonoBehaviour
{
	private float limitStart = 50f;

	private void Start()
	{
		_ = limitStart;
		if (Settings.performanceMode)
		{
			limitStart /= 2f;
		}
		if (Mathf.Clamp01(1f * ((float)Enemy.all.Count - limitStart) / (float)Enemy.all.Count) > Random.value)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
