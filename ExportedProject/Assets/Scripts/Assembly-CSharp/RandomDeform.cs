using UnityEngine;

public class RandomDeform : MonoBehaviour
{
	public Vector3 scaleMin = new Vector3(0.8f, 0.8f, 0.8f);

	public Vector3 scaleMax = new Vector3(1.6f, 1.6f, 1.6f);

	private void Start()
	{
		Vector3 localScale = base.transform.localScale;
		localScale.x *= Random.Range(scaleMin.x, scaleMax.x);
		localScale.y *= Random.Range(scaleMin.y, scaleMax.y);
		localScale.z *= Random.Range(scaleMin.z, scaleMax.z);
		base.transform.localScale = localScale;
	}
}
