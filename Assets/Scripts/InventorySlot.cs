using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

	public Text amountText;
	public Sprite slotSprite;
	public Sprite emptySlotSprite;

	public Image slotImage;
	public Image icon;

	public GameObject selector;

	public Animation anim;

	Inventory inventory;
	PlayerController player;

	int mode;

	[HideInInspector] public int slotID;
	[HideInInspector] public Item currentItem;
	[HideInInspector] public int stackCount;

	bool wasActiveInHierarchy;

	void Awake() {
		anim = GetComponent<Animation>();
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		inventory = playerObj.GetComponent<Inventory>();
		player = playerObj.GetComponent<PlayerController>();
	}

	public void SetItem(Item item, int _stackCount) {
		if(!item) {
			ClearItem();
			return;
		}
		icon.gameObject.SetActive(true);
		icon.sprite = item.icon;
		currentItem = item;
		slotImage.sprite = slotSprite;
		stackCount = _stackCount;
		if(mode != 1) {
			amountText.gameObject.SetActive(true);
		}
		amountText.text = stackCount.ToString();
		if(anim == null) {
			anim = GetComponent<Animation>();
		}
		anim.Play();
	}

	public void IncreaseItem(int count) {
		if(mode != 1) {
			stackCount += count;
			amountText.text = stackCount.ToString();
		}
		anim.Play();
	}

	public void DecreaseItem(int count) {
		if(mode != 1) {
			stackCount -= count;
			amountText.text = stackCount.ToString();
			if(stackCount <= 0) {
				ClearItem();
			}
		}
		anim.Play();
	}

	public void ClearItem() {
		icon.sprite = null;
		icon.gameObject.SetActive(false);
		currentItem = null;
		slotImage.sprite = emptySlotSprite;
		stackCount = 0;
		amountText.gameObject.SetActive(false);
	}

	public void OnItemClick() {
		if(currentItem && !player.ActiveSystemMenu() && !player.dead) {
			if(currentItem.type == Item.ItemType.Structure) {
				PlaceItem(currentItem);
			} else {
				DropItem();
			}
		}
	}
	/*
	public void OnInventoryClose() {
		if(transform.parent.name != "Hotbar") {
			anim.Play();
		}
	}
	*/
	public void OnItemPointerEnter() {
		inventory.SetHoveredItem(currentItem, this);
	}

	public void OnItemPointerExit() {
		inventory.LeaveHoveredItem();
	}

	public void OnItemBeginDrag() {
		inventory.BeginDrag(this);
	}

	public void OnItemEndDrag() {
		inventory.EndDrag();
	}

	void DropItem() {
		inventory.DropItem(currentItem, 1);
		if(mode != 1) {
			stackCount--;
			amountText.text = stackCount.ToString();
			if(stackCount <= 0) {
				ClearItem();
			}
			inventory.InventoryUpdate();
		}
	}

	public void PlaceItem(Item item) {
		inventory.Place(item);
		if(mode != 1) {
			stackCount--;
			amountText.text = stackCount.ToString();
			if(stackCount <= 0) {
				ClearItem();
			}
		}
	}

	public void LoadCreativeMode() {
		mode = 1;
		amountText.gameObject.SetActive(false);
	}
}
