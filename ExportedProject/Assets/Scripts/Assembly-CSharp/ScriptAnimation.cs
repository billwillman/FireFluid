using UnityEngine;

public class ScriptAnimation : MonoBehaviour
{
	public AnimationUpdateMode updateMode;

	public AnimationType type;

	public float duration = 1f;

	public float playbackSpeed = 1f;

	[HideInInspector]
	protected float time;

	protected float remainingTime;

	protected GameObject target;

	protected Vector3 deltaPosition;

	protected Vector3 deltaScale;

	protected Vector3 deltaRotation;

	private Vector3 dP0 = Vector3.zero;

	private Vector3 dS0 = Vector3.zero;

	private Vector3 dR0 = Vector3.zero;

	public void Reset()
	{
		if (!(target == null))
		{
			deltaPosition = Vector3.zero;
			deltaScale = Vector3.zero;
			deltaRotation = Vector3.zero;
			ApplyDeltas();
			SetTime(0f);
		}
	}

	public void SetTime(float time)
	{
		this.time = time;
	}

	public static ScriptAnimation InstantiateTargetAnimation(ScriptAnimation targetAnimation, GameObject target, bool instantiateOnCenter = true, bool setParent = true, bool useTargetRotation = true)
	{
		if (targetAnimation != null)
		{
			GameObject gameObject = Object.Instantiate(targetAnimation.gameObject);
			if (setParent)
			{
				gameObject.transform.localScale = target.transform.lossyScale;
			}
			else
			{
				gameObject.transform.position = target.transform.position;
			}
			if (useTargetRotation)
			{
				gameObject.transform.rotation = target.transform.rotation;
			}
			ScriptAnimation component = gameObject.GetComponent<ScriptAnimation>();
			component.StartAnimation(target, setParent);
			return component;
		}
		return null;
	}

	public virtual void StartAnimation(GameObject target, bool setParent = true)
	{
		this.target = target;
		remainingTime = duration;
		if (setParent && (type == AnimationType.Endless || type == AnimationType.Loop))
		{
			base.gameObject.transform.SetParent(target.transform);
		}
	}

	public virtual void UpdateDeltas()
	{
	}

	private void Start()
	{
		if (type == AnimationType.Endless || type == AnimationType.Loop)
		{
			StartAnimation(base.gameObject);
		}
	}

	private void ApplyDeltas()
	{
		Vector3 localPosition = target.transform.localPosition;
		Vector3 localScale = target.transform.localScale;
		Vector3 eulerAngles = target.transform.localRotation.eulerAngles;
		localPosition -= dP0;
		localScale.x /= 1f + dS0.x;
		localScale.y /= 1f + dS0.y;
		localScale.z /= 1f + dS0.z;
		eulerAngles -= dR0;
		localPosition += deltaPosition;
		localScale.x *= 1f + deltaScale.x;
		localScale.y *= 1f + deltaScale.y;
		localScale.z *= 1f + deltaScale.z;
		eulerAngles += deltaRotation;
		target.transform.localPosition = localPosition;
		target.transform.localScale = localScale;
		target.transform.localRotation = Quaternion.Euler(eulerAngles);
		dP0 = deltaPosition;
		dS0 = deltaScale;
		dR0 = deltaRotation;
		if (type != AnimationType.Endless && remainingTime < 0f)
		{
			localPosition = target.transform.localPosition;
			localScale = target.transform.localScale;
			eulerAngles = target.transform.localRotation.eulerAngles;
			localPosition -= dP0;
			localScale.x /= 1f + dS0.x;
			localScale.y /= 1f + dS0.y;
			localScale.z /= 1f + dS0.z;
			eulerAngles -= dR0;
			target.transform.localPosition = localPosition;
			target.transform.localScale = localScale;
			target.transform.localRotation = Quaternion.Euler(eulerAngles);
			if (type == AnimationType.Once)
			{
				Object.Destroy(base.gameObject);
			}
			else if (type == AnimationType.Loop)
			{
				remainingTime = duration;
			}
		}
	}

	public virtual void Update()
	{
		if (!(target == null))
		{
			UpdateDeltas();
			ApplyDeltas();
			float num = Time.deltaTime;
			if (updateMode == AnimationUpdateMode.UnscaledDeltaTime)
			{
				num = Time.unscaledDeltaTime;
			}
			time += num * playbackSpeed;
			remainingTime -= num * playbackSpeed;
		}
	}
}
