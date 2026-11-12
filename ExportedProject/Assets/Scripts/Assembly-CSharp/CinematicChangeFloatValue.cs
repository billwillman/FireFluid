using System;
using UnityEngine;

public class CinematicChangeFloatValue : CinematicAction
{
	private Ref<float> valueRef;

	private float startValue;

	private float targetValue;

	private float time;

	private float passedTime;

	public override void SetActor(GameObject actor)
	{
	}

	public CinematicChangeFloatValue(Func<float> getter, Action<float> setter, float targetValue, float time, bool blocking = false)
	{
		base.blocking = blocking;
		valueRef = new Ref<float>(getter, setter);
		this.targetValue = targetValue;
		this.time = time;
	}

	public override void StartAction()
	{
		startValue = valueRef.Value;
	}

	public override bool UpdateAction(float dt)
	{
		passedTime += dt;
		float t = Mathf.Clamp01(passedTime / time);
		valueRef.Value = interpolator.interpolate(startValue, targetValue, t);
		if (passedTime >= time)
		{
			return true;
		}
		return false;
	}
}
