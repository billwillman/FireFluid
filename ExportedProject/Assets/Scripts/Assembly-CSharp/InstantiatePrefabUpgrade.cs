using UnityEngine;

public class InstantiatePrefabUpgrade : Upgrade
{
	public string classPath = "CharacterAttributes";

	[Space]
	public GameObject prefab;

	public override bool RequirementsMet(Player player)
	{
		bool flag = ValueModification.Get(player.gameObject, classPath) != null;
		return base.RequirementsMet(player) && flag;
	}

	public override void Apply(Player player)
	{
		base.Apply(player);
		object obj = ValueModification.Get(player.gameObject, classPath);
		if (!(obj is MonoBehaviour))
		{
			Debug.Log("Class not found: [" + classPath + "]");
			return;
		}
		GameObject obj2 = Object.Instantiate(prefab, ((MonoBehaviour)obj).transform);
		obj2.transform.localPosition = Vector3.zero;
		obj2.transform.localRotation = Quaternion.identity;
	}
}
