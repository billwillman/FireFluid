using UnityEngine;

public class InstantiateSacrifices : MonoBehaviour
{
	private void Start()
	{
		foreach (SacrificeEntry allActiveSacrificeEntry in PersistentDataComponent.GetDataInstance().GetAllActiveSacrificeEntries())
		{
			Object.Instantiate(Encyclopedia.Get().GetSacrifice(allActiveSacrificeEntry.id), base.transform);
		}
	}

	private void Update()
	{
	}
}
