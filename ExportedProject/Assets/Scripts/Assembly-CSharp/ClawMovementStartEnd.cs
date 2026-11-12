using UnityEngine;

[RequireComponent(typeof(Claw))]
public class ClawMovementStartEnd : MonoBehaviour
{
	private Claw claw;

	public Vector3 targetPosition;

	public float speed = 64f;

	public void Initialize(Vector3 targetPosition, float speed)
	{
		this.targetPosition = targetPosition;
		this.speed = speed;
	}

	private void Awake()
	{
		claw = GetComponent<Claw>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (claw.IsTerminated())
		{
			return;
		}
		Vector3 vector = targetPosition - base.transform.position;
		float magnitude = vector.magnitude;
		if (magnitude > 0.01f)
		{
			float num = speed * Time.deltaTime;
			if (num > magnitude)
			{
				num = magnitude;
			}
			base.transform.position += vector / magnitude * num;
		}
		else
		{
			claw.Terminate();
		}
	}
}
