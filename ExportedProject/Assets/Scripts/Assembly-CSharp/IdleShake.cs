using UnityEngine;

public class IdleShake : ScriptAnimation
{
	public float intensity = 0.05f;

	public Vector3 speed;

	public override void UpdateDeltas()
	{
		deltaPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * intensity;
	}
}
