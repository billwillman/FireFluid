using UnityEngine;

public class IdleLevitate : ScriptAnimation
{
	public float timeOffset;

	public Vector3 positionDiff = new Vector3(0.1f, -0.2f, 0f);

	public float speed = 1f;

	public override void UpdateDeltas()
	{
		float num = Mathf.Sin((timeOffset + time) * speed);
		deltaPosition = positionDiff * num;
	}
}
