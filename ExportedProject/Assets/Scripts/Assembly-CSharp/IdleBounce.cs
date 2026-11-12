using UnityEngine;

public class IdleBounce : ScriptAnimation
{
	public Vector3 scaleDiff = new Vector3(0.1f, -0.2f, 0f);

	public float speed;

	public float bouncyness = 1f;

	public override void UpdateDeltas()
	{
		float num = Mathf.Pow(1f - Mathf.Abs(Mathf.Sin(time * speed)), bouncyness);
		deltaScale = scaleDiff * num;
	}
}
