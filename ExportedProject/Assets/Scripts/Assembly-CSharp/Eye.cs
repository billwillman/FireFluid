using UnityEngine;

public class Eye : MonoBehaviour
{
	public Transform lookBone;

	private void Start()
	{
		Animator componentInChildren = GetComponentInChildren<Animator>();
		if ((bool)componentInChildren)
		{
			componentInChildren.Play("Idle", 0, Random.value);
		}
	}

	private void Update()
	{
		if (Time.frameCount % 4 == 0)
		{
			if ((double)Vector3.Dot(base.transform.up, Player.Get().transform.position - base.transform.position) > 0.1)
			{
				float num = 2f;
				lookBone.rotation = Quaternion.Euler(Random.Range(0f - num, num), Random.Range(0f - num, num), Random.Range(0f - num, num)) * Quaternion.LookRotation(Player.Get().transform.position - base.transform.position) * Quaternion.Euler(78f, 0f, 0f);
			}
			else
			{
				float num2 = 12f;
				lookBone.localRotation = Quaternion.Euler(Random.Range(0f - num2, num2), Random.Range(0f - num2, num2), Random.Range(0f - num2, num2)) * Quaternion.Euler(180f, 0f, 0f);
			}
		}
	}
}
