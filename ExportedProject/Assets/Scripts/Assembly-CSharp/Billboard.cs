using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[DefaultExecutionOrder(100)]
public class Billboard : MonoBehaviour
{
	public bool billboardX;

	public bool billboardY = true;

	private void UpdateBillboard()
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		Vector3 eulerAngles2 = Camera.main.transform.rotation.eulerAngles;
		if (billboardX)
		{
			eulerAngles.x = eulerAngles2.x;
		}
		if (billboardY)
		{
			eulerAngles.y = eulerAngles2.y;
		}
		eulerAngles.z = 0f;
		base.transform.rotation = Quaternion.Euler(eulerAngles);
	}

	private void Start()
	{
		UpdateBillboard();
		Text componentInChildren = GetComponentInChildren<Text>();
		if ((bool)componentInChildren)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(componentInChildren.rectTransform);
		}
		TextMeshProUGUI componentInChildren2 = GetComponentInChildren<TextMeshProUGUI>();
		if ((bool)componentInChildren2)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(componentInChildren2.rectTransform);
		}
	}

	private void OnEnable()
	{
		UpdateBillboard();
	}

	private void LateUpdate()
	{
		UpdateBillboard();
	}
}
