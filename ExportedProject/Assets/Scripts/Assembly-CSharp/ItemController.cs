using UnityEngine;

public class ItemController : MonoBehaviour
{
	public GameObject pickUpEffect;

	private GameObject itemParent;

	public ItemUI itemUI;

	private CharacterAttributes attributes;

	private void Start()
	{
		itemParent = new GameObject("Items");
		itemParent.transform.parent = base.transform;
		itemParent.transform.localPosition = Vector3.zero;
		itemParent.transform.localRotation = Quaternion.identity;
		attributes = GetComponent<CharacterAttributes>();
	}

	public void RequestEquip(Item item)
	{
		ItemUI component = Object.Instantiate(itemUI.gameObject).GetComponent<ItemUI>();
		component.item = item;
		component.itemController = this;
	}

	public void Equip(Item item)
	{
		Object.Instantiate(pickUpEffect).transform.position = item.transform.position;
		item.transform.parent = itemParent.transform;
		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
		item.worldModel.SetActive(value: false);
		item.GetComponent<Collider>().enabled = false;
		item.OnEquip(attributes);
	}

	public void Drop(Item item)
	{
		item.transform.parent = null;
		item.worldModel.SetActive(value: true);
		item.gameObject.AddComponent<DropAnimation>();
		item.GetComponent<Collider>().enabled = true;
		item.OnUnequip();
	}
}
