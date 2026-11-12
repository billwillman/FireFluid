using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLabel : MonoBehaviour
{
	public TextMeshProUGUI text;

	public Transform target;

	public Vector3 offset = Vector3.zero;

	public Image background;

	public void UpdatePosition()
	{
		base.transform.position = target.position + offset;
	}

	private void Update()
	{
		if (target == null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			UpdatePosition();
		}
	}

	public void SetText(string text)
	{
		this.text.text = text;
	}
}
