using UnityEngine;

public class ShowDialogOnStart : MonoBehaviour
{
	public bool showOnlyOnce;

	public string styleId;

	[TextArea(3, 32)]
	public string text;

	private void Start()
	{
		if (!showOnlyOnce && PersistentDataComponent.GetDataInstance().GetData(GetKey()) == null)
		{
			DialogManager.Get().ShowDialog(text, styleId);
		}
		else
		{
			PersistentDataComponent.GetDataInstance().SetData(GetKey(), true);
		}
	}

	public string GetKey()
	{
		return "dialog_" + base.gameObject.name;
	}
}
