using UnityEngine;

public class InteractableShowHint : MonoBehaviour, IInteractableHint
{
	public float animationTime = 0.5f;

	private float currentAnimationTime;

	public InteractableHintStyle style;

	public bool billboard;

	public Vector3 positionStart = Vector3.down;

	public Vector3 positionEnd = Vector3.zero;

	public Vector3 rotationStart = Vector3.zero;

	public Vector3 rotationEnd = Vector3.zero;

	public Vector3 scaleStart = new Vector3(1f, 0f, 1f);

	public Vector3 scaleEnd = Vector3.one;

	protected bool shown;

	public Transform hintObject;

	private Vector3 hintObjectPosition;

	private Quaternion hintObjectRotation;

	private Vector3 hintObjectScale;

	public virtual void OnStartShowHint(InteractionController c)
	{
		shown = true;
		if (style != InteractableHintStyle.MoveRotateScale)
		{
			_ = 1;
			return;
		}
		hintObject.localPosition = hintObjectPosition + positionStart;
		hintObject.localRotation = Quaternion.Euler(rotationStart) * hintObjectRotation;
		Vector3 localScale = hintObjectScale;
		localScale.x *= scaleStart.x;
		localScale.y *= scaleStart.y;
		localScale.z *= scaleStart.z;
		hintObject.localScale = localScale;
	}

	public virtual void OnEndShowHint()
	{
		shown = false;
	}

	public bool IsSown()
	{
		return shown;
	}

	public virtual void Awake()
	{
		if (hintObject == null && base.transform.childCount > 0)
		{
			hintObject = base.transform.GetChild(0);
		}
		hintObjectPosition = hintObject.localPosition;
		hintObjectRotation = hintObject.localRotation;
		hintObjectScale = hintObject.localScale;
	}

	public virtual void Update()
	{
		if (shown)
		{
			currentAnimationTime = animationTime;
		}
		else
		{
			currentAnimationTime -= Time.unscaledDeltaTime;
		}
		hintObject.gameObject.SetActive(currentAnimationTime > 0f);
		switch (style)
		{
		case InteractableHintStyle.OnOff:
			if (shown)
			{
				hintObject.gameObject.SetActive(value: true);
			}
			else
			{
				hintObject.gameObject.SetActive(value: false);
			}
			break;
		case InteractableHintStyle.MoveRotateScale:
		{
			Vector3 b = hintObjectScale;
			Vector3 b2;
			Quaternion b3;
			if (shown)
			{
				b2 = hintObjectPosition + positionEnd;
				b3 = Quaternion.Euler(rotationEnd) * hintObjectRotation;
				b.x *= scaleEnd.x;
				b.y *= scaleEnd.y;
				b.z *= scaleEnd.z;
			}
			else
			{
				b2 = hintObjectPosition + positionStart;
				b3 = Quaternion.Euler(rotationStart) * hintObjectRotation;
				b.x *= scaleStart.x;
				b.y *= scaleStart.y;
				b.z *= scaleStart.z;
			}
			hintObject.localPosition = Vector3.Lerp(hintObject.localPosition, b2, Time.unscaledDeltaTime * 4f);
			hintObject.localRotation = Quaternion.Slerp(hintObject.localRotation, b3, Time.unscaledDeltaTime * 8f);
			hintObject.localScale = Vector3.Lerp(hintObject.localScale, b, Time.unscaledDeltaTime * 16f);
			break;
		}
		}
	}

	private void LateUpdate()
	{
		if (billboard)
		{
			Vector3 eulerAngles = Camera.main.transform.rotation.eulerAngles;
			eulerAngles.z = 0f;
			hintObject.rotation = Quaternion.Euler(eulerAngles);
		}
	}
}
