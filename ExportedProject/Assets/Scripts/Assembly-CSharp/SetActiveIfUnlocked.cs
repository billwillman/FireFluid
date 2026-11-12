using UnityEngine;

public class SetActiveIfUnlocked : MonoBehaviour
{
	public string dataKey = "maskofchaosunlocked";

	public bool active = true;

	public GameObject target;

	private void Start()
	{
	}

	private void Update()
	{
		if (PersistentDataComponent.GetDataInstance().GetData(dataKey) != null)
		{
			target.SetActive(active);
		}
		else
		{
			target.SetActive(!active);
		}
	}
}
