using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour {
	public Item currentItem;
	public Image icon;
	public Text amountText;

	Inventory inventory;

	void Start() {
		inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
	}

	public void OnItemPointerEnter() {
		if(currentItem) {
			inventory.SetHoveredItem(currentItem);
		}
	}

	public void OnItemPointerExit() {
		inventory.LeaveHoveredItem();
	}
}
