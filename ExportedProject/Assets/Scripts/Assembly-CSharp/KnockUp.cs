using UnityEngine;

public class KnockUp : MonoBehaviour
{
	public float height = 4f;

	private float power = 2f;

	public float speed = 2f;

	private float currentHeight;

	private float currentHeight0;

	private float time;

	public Vector3 offset;

	private float originalHeight;

	private Animator animator;

	private CharacterController player;

	private const string VALUE_MODIFIER_ID = "knockup";

	public float DropCurve(float t)
	{
		return (1f - Mathf.Abs(Mathf.Pow(t * 2f - 1f, power))) * height;
	}

	private void Start()
	{
		currentHeight = DropCurve(0f);
		currentHeight0 = currentHeight;
		animator = GetComponentInChildren<Animator>();
		player = GetComponent<CharacterController>();
		if ((bool)player)
		{
			player.enableAttacks.Push("knockup", value: false);
		}
	}

	private void Update()
	{
		time += Time.deltaTime * speed;
		currentHeight = DropCurve(Mathf.Clamp01(time));
		animator.transform.position += Vector3.up * (currentHeight - currentHeight0);
		if ((bool)player)
		{
			player.SetAnimationTakeOver(AnimationTakeOver.AnimationAndTransform);
			player.OffsetPosition(offset * Time.deltaTime);
		}
		else
		{
			base.transform.position += offset * Time.deltaTime;
		}
		currentHeight0 = currentHeight;
		if (time > 1f)
		{
			if ((bool)player)
			{
				player.enableAttacks.PopAll("knockup");
				Object.Destroy(this);
				return;
			}
			Vector3 position = base.transform.position;
			position.y = 0f;
			base.transform.position = position;
			Object.Destroy(this);
		}
	}
}
