using TMPro;
using UnityEngine;

public class SacrificesAvailableLabel : MonoBehaviour
{
	private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.text = "<sprite name=blood> " + PersistentDataComponent.GetDataInstance().spendSacrifices + "/" + PersistentDataComponent.GetDataInstance().maxSacrifices;
	}
}
