using UnityEngine;

public class Sacrifice : MonoBehaviour
{
	public string identifier = "sacrifice";

	public string GetIdentifier()
	{
		return identifier;
	}

	public string GetName()
	{
		return StringLocalization.Get("sacrifice_name_" + GetIdentifier());
	}

	public virtual string GetDescription()
	{
		return StringLocalization.Get("sacrifice_description_" + GetIdentifier());
	}

	public static string GetDescriptionAll(GameObject sacrifices)
	{
		string text = "";
		Sacrifice[] components = sacrifices.GetComponents<Sacrifice>();
		foreach (Sacrifice sacrifice in components)
		{
			text += sacrifice.GetDescription();
		}
		return text;
	}

	public int GetMaxLevel()
	{
		return 5;
	}

	public bool CanLevelUp()
	{
		SacrificeEntry sacrificeEntry = PersistentDataComponent.GetDataInstance().GetSacrificeEntry(GetIdentifier());
		if (PersistentDataComponent.GetDataInstance().UnspentSacrificeAvailable() && GetMaxLevel() > sacrificeEntry.level)
		{
			return true;
		}
		return false;
	}

	public bool CanLevelDown()
	{
		if (PersistentDataComponent.GetDataInstance().GetSacrificeEntry(GetIdentifier()).level > 0)
		{
			return true;
		}
		return false;
	}

	public void LevelUp()
	{
		if (CanLevelUp())
		{
			PersistentDataComponent.GetDataInstance().GetSacrificeEntry(GetIdentifier()).level++;
			PersistentDataComponent.GetDataInstance().spendSacrifices++;
		}
	}

	public void LevelDown()
	{
		if (CanLevelDown())
		{
			PersistentDataComponent.GetDataInstance().GetSacrificeEntry(GetIdentifier()).level--;
			PersistentDataComponent.GetDataInstance().spendSacrifices--;
		}
	}

	public static float GetExtraCurrencyPerMinute()
	{
		return (float)PersistentDataComponent.GetDataInstance().spendSacrifices * 0.25f;
	}
}
