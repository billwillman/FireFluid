using UnityEngine;

public class IdleRotate : ScriptAnimation
{
	public Vector3 speed;

	public override void UpdateDeltas()
	{
		deltaRotation = speed * time;
	}
}
