using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
	private CharacterAttributes attributes1;

	private CharacterAttributes attributes2;

	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	private int numberOfPositions = 8;

	private Vector3[] positions;

	private float wiggle;

	public Transform chainEnd1;

	public Transform chainEnd2;

	public float movementSpeedMultiplier = 0.7f;

	public const string VALUE_MODIFIER_ID = "chain";

	public bool destroy;

	private const float MAX_LENGTH = 48f;

	public void Initialize(CharacterAttributes attributes1, CharacterAttributes attributes2)
	{
		base.transform.position = (attributes1.transform.position + attributes2.transform.position) * 0.5f;
		this.attributes1 = attributes1;
		this.attributes2 = attributes2;
		positions = new Vector3[numberOfPositions];
		GetComponentsInChildren(lineRenderers);
		foreach (LineRenderer lineRenderer in lineRenderers)
		{
			lineRenderer.positionCount = positions.Length;
			lineRenderer.textureScale = new Vector2((attributes1.transform.position - attributes2.transform.position).magnitude, 1f);
		}
		wiggle = 1f;
		SetPositions(999f);
		attributes1.movementSpeed.value.Push("chain", movementSpeedMultiplier, ValueModifierType.Multiply);
		attributes2.movementSpeed.value.Push("chain", movementSpeedMultiplier, ValueModifierType.Multiply);
		ElementHelper.SetElement(base.gameObject, Player.Get().GetAttributes().GetElement() * 0.3f);
	}

	private void Start()
	{
	}

	private void SetPositions(float dt)
	{
		for (int i = 0; i < positions.Length; i++)
		{
			float num = 1f * (float)i / (float)(positions.Length - 1);
			float num2 = (num - 0.5f) * (num - 0.5f) * 4f;
			Vector3 vector = Vector3.Lerp(attributes1.transform.position, attributes2.transform.position, num) + Vector3.up * num2;
			positions[i] = Vector3.Lerp(positions[i], vector, Mathf.Lerp(Mathf.Clamp01(dt * 2f * (1f + wiggle)), 1f, num2 * num2 * num2));
			positions[i] = Vector3.Lerp(positions[i], vector + Vector3.up * (1f - num2) * (Mathf.Sin(Time.time * 24f) + 1f), wiggle);
		}
		chainEnd1.position = attributes1.transform.position + Vector3.up;
		chainEnd2.position = attributes2.transform.position + Vector3.up;
	}

	private void Update()
	{
		if (destroy)
		{
			foreach (LineRenderer lineRenderer in lineRenderers)
			{
				Color startColor = lineRenderer.startColor;
				startColor.a -= Time.deltaTime * 2f;
				if ((double)startColor.a < 0.01)
				{
					Object.Destroy(base.gameObject);
					break;
				}
				lineRenderer.startColor = startColor;
				lineRenderer.endColor = startColor;
			}
			return;
		}
		if (!attributes1 || attributes1.health.IsExhausted() || !attributes2 || attributes2.health.IsExhausted())
		{
			destroy = true;
			return;
		}
		if ((attributes1.transform.position - attributes2.transform.position).sqrMagnitude > 2304f)
		{
			destroy = true;
			return;
		}
		SetPositions(Time.deltaTime);
		foreach (LineRenderer lineRenderer2 in lineRenderers)
		{
			lineRenderer2.SetPositions(positions);
		}
		wiggle = Mathf.Lerp(wiggle, 0f, Time.deltaTime * 2f);
	}

	private void OnDestroy()
	{
		if ((bool)attributes1)
		{
			attributes1.movementSpeed.value.PopAll("chain");
		}
		if ((bool)attributes2)
		{
			attributes2.movementSpeed.value.PopAll("chain");
		}
	}
}
