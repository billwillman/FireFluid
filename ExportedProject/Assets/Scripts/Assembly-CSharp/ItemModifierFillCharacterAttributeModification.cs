using UnityEngine;

public class ItemModifierFillCharacterAttributeModification : ItemModifierTemporaryAttributeModification, IItemModifier
{
	public string attributePath = "CharacterAttributes.health";

	public float minFill;

	public float maxFill;

	private float fill;

	public override string GetDescription(int level)
	{
		return StringLocalization.Get("item_recoverattribute").Replace("{value}", ((int)fill).ToString()).Replace("{attribute}", ValueModification.AttributePathToName(attributePath)) + "<br><br>" + base.GetDescription(level);
	}

	public override void OnEquip(CharacterAttributes owner)
	{
		base.OnEquip(owner);
		object obj = ValueModification.Get(owner.gameObject, attributePath);
		if (obj != null && obj is DynamicCharacterAttribute)
		{
			((DynamicCharacterAttribute)obj).Fill(fill);
		}
		else
		{
			Debug.Log("couldn't find dynamic character attribute " + attributePath);
		}
	}

	public override void Evaluate(int level)
	{
		base.Evaluate(level);
		fill = Random.Range(minFill, maxFill) * (float)level;
	}
}
