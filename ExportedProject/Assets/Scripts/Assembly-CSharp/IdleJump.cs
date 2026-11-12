using UnityEngine;

public class IdleJump : ScriptAnimation
{
	public Vector3 posDiff = new Vector3(0f, 0.3f, 0f);

	public float speed;

	public float bouncyness = 1f;

	public override void UpdateDeltas()
	{
		float num = Mathf.Pow(1f - Mathf.Abs(Mathf.Sin(time * speed)), bouncyness);
		deltaPosition = posDiff * num;
	}
}
