using UnityEngine;

[DefaultExecutionOrder(100)]
public class ApplyUpgradesOnStart : MonoBehaviour
{
	private void Start()
	{
		Player component = GetComponent<Player>();
		Encyclopedia.Get().GetMask(PersistentDataComponent.GetDataInstance().currentMask).Apply(component);
		foreach (AttributeModification treeModification in PersistentDataComponent.GetDataInstance().treeModifications)
		{
			treeModification.ApplyModificationOnContext(component.gameObject, "tree");
		}
	}
}
