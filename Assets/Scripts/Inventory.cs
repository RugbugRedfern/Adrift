using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	[SerializeField] GameObject placementParticleSystem;
	[SerializeField] GameObject slotPrefab;

	public GameObject inventoryContainer;
	public Transform craftingRecipeContainer;
	[SerializeField] GameObject craftingContainer;

	public GameObject tooltip;
	public Vector3 tooltipOffset;
	public Text tooltipTitle;
	public Text tooltipDesc;
	RectTransform tooltipTransform;

	public Transform[] slotHolders;
	[SerializeField] Transform inventorySlotsContainer;
	public Transform hotbar;

	public List<InventorySlot> slots = new List<InventorySlot>();

	PlayerController player;

	AchievementManager achievementManager;
	AudioManager audioManager;

	[HideInInspector] public bool placingStructure;
	GameObject currentPreviewObj;
	Item currentPlacingItem;
	int currentPlacingRot;

	public int selectedHotbarSlot = 0;
	public Item currentSelectedItem;

	InventorySlot currentHoveredSlot;
	InventorySlot beginDragSlot;

	SaveManager saveManager;

	int mode;

	float gridX = 1f;
	float gridY = 0.25f;
	float gridZ = 1f;

	int slotID = 0;

	void Awake() {
		audioManager = FindObjectOfType<AudioManager>();
		achievementManager = FindObjectOfType<AchievementManager>();
		saveManager = FindObjectOfType<SaveManager>();

		tooltipTransform = tooltip.GetComponent<RectTransform>();

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		foreach(Transform slotHolder in slotHolders) {
			foreach(Transform slot in slotHolder) {
				slots.Add(slot.GetComponent<InventorySlot>());
				slot.GetComponent<InventorySlot>().slotID = slotID;
				slotID++;
			}
		}
	}
	
	public void LoadCreativeMode() {
		mode = 1;

		// Add extra slots to the inventory so it can contain every item
		int i = 0;
		foreach(Transform slotHolder in slotHolders) {
			foreach(Transform slot in slotHolder) {
				i++;
			}
		}
		if(i < saveManager.allItems.Length) {
			for(int q = 0; q < saveManager.allItems.Length - i; q++) {
				Instantiate(slotPrefab, inventorySlotsContainer);
			}
		}
		Instantiate(slotPrefab, inventorySlotsContainer); // One extra empty slot for screenshots

		foreach(Transform slotHolder in slotHolders) {
			foreach(Transform slot in slotHolder) {
				InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
				if(!slots.Contains(inventorySlot)) {
					slots.Add(inventorySlot);
					slot.GetComponent<InventorySlot>().slotID = slotID;
					slotID++;
				}
			}
		}
		AddAllItems();
		foreach(InventorySlot slot in slots) {
			slot.LoadCreativeMode();
		}
		craftingContainer.SetActive(false);
	}

	public void Pickup(ItemHandler itemHandler) {
		if(mode != 1) {
			AddItem(itemHandler.item, 1); //TODO: Check if inventory is full first!
		}
		Destroy(itemHandler.gameObject);
	}

	void Update() {
		if(tooltip.activeSelf) {
			tooltipTransform.position = Input.mousePosition + tooltipOffset;
		}

		if(Input.GetKeyDown(KeyCode.Tab)) {
			inventoryContainer.SetActive(!inventoryContainer.activeSelf);
			player.LockLook(inventoryContainer.activeSelf);
			if(inventoryContainer.activeSelf) {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			} else {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				LeaveHoveredItem();
				/*
				foreach(InventorySlot slot in slots) {
					slot.OnInventoryClose();
				}
				*/
			}
		}

		for(int i = 1; i < hotbar.childCount + 1; i++) {
			if(Input.GetKeyDown("" + i)) {
				if(selectedHotbarSlot != i - 1) {
					selectedHotbarSlot = i - 1;
					if(placingStructure) {
						CancelStructurePlacement();
					}
					HotbarUpdate();
				}
			}
		}

		if((Input.GetAxisRaw("Mouse ScrollWheel") != 0 || Input.GetButtonDown("CycleRight") || Input.GetButtonDown("CycleLeft")) && !player.ActiveMenu()) {
			if(placingStructure) {
				CancelStructurePlacement();
			}
			if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 || Input.GetButtonDown("CycleLeft")) {
				selectedHotbarSlot--;
				if(selectedHotbarSlot < 0) {
					selectedHotbarSlot = hotbar.childCount - 1;
				}
			} else {
				selectedHotbarSlot++;
				if(selectedHotbarSlot >= hotbar.childCount) {
					selectedHotbarSlot = 0;
				}
			}

			HotbarUpdate();

		}

		if(!player.dead) {
			if(placingStructure && !player.ActiveMenu()) {
				player.ShowNoticeText("[LMB] to place, [RMB] to cancel, [R] to rotate");
				if(!currentPreviewObj.activeSelf) {
					currentPreviewObj.SetActive(true);
				}

				if(!player.target || player.targetHit.distance > player.interactRange) {
					if(currentPlacingItem.alignToNormal) {
						currentPreviewObj.transform.position = player.playerCamera.transform.position + player.playerCamera.transform.forward * player.interactRange;
					} else {
						Vector3 targetPos = player.playerCamera.transform.position + player.playerCamera.transform.forward * player.interactRange;
						currentPreviewObj.transform.position = new Vector3(Mathf.Round(targetPos.x / gridX) * gridX,
						Mathf.Round(targetPos.y / gridY) * gridY,
						Mathf.Round(targetPos.z / gridZ) * gridZ);
					}
				} else {
					if(currentPlacingItem.alignToNormal) {
						currentPreviewObj.transform.position = player.targetHit.point;
					} else {
						currentPreviewObj.transform.position = new Vector3(Mathf.Round((player.targetHit.point.x / gridX) + player.targetHit.normal.normalized.x * 0.05f) * gridX,
						Mathf.Round((player.targetHit.point.y + player.targetHit.normal.normalized.y * 0.05f) / gridY) * gridY,
						Mathf.Round((player.targetHit.point.z / gridZ) + player.targetHit.normal.normalized.z * 0.05f) * gridZ);
					}
				}

				if(Input.GetKeyDown(KeyCode.R)) { //TODO: Make "Rotate" Button, not key
					currentPlacingRot++;
					if(currentPlacingRot >= currentPlacingItem.rots.Length) {
						currentPlacingRot = 0;
					}
					if(!currentPlacingItem.alignToNormal) {
						currentPreviewObj.transform.rotation = Quaternion.Euler(currentPlacingItem.rots[currentPlacingRot]);
					}
				}

				if(currentPlacingItem.alignToNormal) {
					if(player.target && player.distanceToTarget <= player.interactRange) { // TODO: WORKING ON CURRENTLY |||___|||---|||___|||===================
						currentPreviewObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, player.targetHit.normal) * Quaternion.Euler(currentPlacingItem.rots[currentPlacingRot]);
					} else {
						currentPreviewObj.transform.rotation = Quaternion.Euler(currentPlacingItem.rots[currentPlacingRot]);
					}
				}

				if(Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
					GameObject go = Instantiate(currentPlacingItem.prefab, currentPreviewObj.transform.position, currentPreviewObj.transform.rotation);
					GameObject psgo = Instantiate(placementParticleSystem, go.transform);
					MeshRenderer mr = go.GetComponent<MeshRenderer>();
					if(!mr) {
						mr = go.GetComponentInChildren<MeshRenderer>();
					}
					if(mr) {
						ParticleSystem ps = psgo.GetComponent<ParticleSystem>();
						ParticleSystem.ShapeModule shape = ps.shape;
						shape.meshRenderer = mr;
						ps.Play();
					}
					audioManager.Play("Build");
					currentPlacingRot = 0;
					RemovePlace();
					InventoryUpdate();
					player.HideNoticeText();
				} else if(Input.GetMouseButtonDown(1) || Input.GetAxisRaw("ControllerTriggers") >= 0.1f) {
					CancelStructurePlacement();
				}
			}
			if(currentSelectedItem && !player.ActiveMenu()) {
				if(Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
					if(currentSelectedItem.type == Item.ItemType.Structure && !placingStructure) {
						hotbar.GetChild(selectedHotbarSlot).GetComponent<InventorySlot>().PlaceItem(currentSelectedItem);
					} else if(currentSelectedItem.type == Item.ItemType.Food) {
						player.Consume(currentSelectedItem);
						hotbar.GetChild(selectedHotbarSlot).GetComponent<InventorySlot>().DecreaseItem(1);
					}

					InventoryUpdate();
				} else if(Input.GetButtonDown("Drop") && currentSelectedItem.type != Item.ItemType.Structure) {
					if(Input.GetButton("Supersize")) {
						InventorySlot slot = hotbar.GetChild(selectedHotbarSlot).GetComponent<InventorySlot>();
						if(mode != 1) {
							DropItem(currentSelectedItem, Ping(5, slot.stackCount));
							slot.DecreaseItem(Ping(5, slot.stackCount));
						} else {
							DropItem(currentSelectedItem, 5);
						}
					} else {
						DropItem(currentSelectedItem, 1);
						hotbar.GetChild(selectedHotbarSlot).GetComponent<InventorySlot>().DecreaseItem(1);
					}

					InventoryUpdate();
				}
			}
		}
	}

	void HotbarUpdate() {
		for(int i = 0; i < hotbar.childCount; i++) {
			GameObject selector = hotbar.GetChild(i).GetComponent<InventorySlot>().selector;
			if(i == selectedHotbarSlot) {
				selector.SetActive(true);
			} else {
				if(selector.activeSelf) {
					selector.SetActive(false);
				}
			}
		}
		currentSelectedItem = hotbar.GetChild(selectedHotbarSlot).GetComponent<InventorySlot>().currentItem;
		player.InventoryUpdate();
	}

	public void SetHoveredItem(Item item, InventorySlot slot = null) {
		audioManager.Play("UIClick");
		if(item) {
			if(!tooltip.activeSelf) {
				tooltip.SetActive(true);
			}
			tooltipTitle.text = item.itemName;
			if(string.IsNullOrEmpty(item.itemDescription)) {
				if(item.type == Item.ItemType.Tool) {
					tooltipDesc.text = "Speed: " + (item.speed).ToString();
				} else if(item.smeltItem) {
					tooltipDesc.text = "Smeltable " + item.type.ToString();
				} else if(item.fuel > 0) {
					tooltipDesc.text = "Burnable " + item.type.ToString();
				} else {
					tooltipDesc.text = item.type.ToString();
				}
			} else {
				tooltipDesc.text = item.itemDescription;
			}
		}

		currentHoveredSlot = slot;
	}

	public void LeaveHoveredItem() {
		if(tooltip.activeSelf) {
			tooltip.SetActive(false);
		}
		if(currentHoveredSlot) {
			currentHoveredSlot = null;
		}
	}

	public void BeginDrag(InventorySlot slot) {
		beginDragSlot = slot;
	}

	public void EndDrag() {
		if(currentHoveredSlot) {
			SwapItems(beginDragSlot, currentHoveredSlot);
			InventoryUpdate();
		}
	}

	void SwapItems(InventorySlot slotA, InventorySlot slotB) {
		Item itemA = slotA.currentItem;
		Item itemB = slotB.currentItem;
		int stackCountA = slotA.stackCount;
		int stackCountB = slotB.stackCount;

		slotA.SetItem(itemB, stackCountB);
		slotB.SetItem(itemA, stackCountA);
	}

	public void CancelStructurePlacement() {
		Destroy(currentPreviewObj);
		currentPlacingRot = 0;
		AddItem(currentPlacingItem, 1);
		placingStructure = false;
		currentPlacingItem = null;
		currentPreviewObj = null;
		player.HideNoticeText();
	}

	public void AddItem(Item item, int amount) {
		for(int q = 0; q < amount; q++) { // NOT SUSTAINABLE
			bool full = true;

			for(int i = 0; i < slots.Count; i++) {
				InventorySlot inventorySlot = slots[i].GetComponent<InventorySlot>();
				if(inventorySlot.currentItem) {
					if(inventorySlot.currentItem.id == item.id) {
						if(inventorySlot.stackCount < item.maxStackCount || mode == 1) {
							inventorySlot.IncreaseItem(1);
							full = false;
							break;
						}
					}
				}
			}

			if(full) {
				for(int i = 0; i < slots.Count; i++) {
					InventorySlot inventorySlot = slots[i].GetComponent<InventorySlot>();
					if(!inventorySlot.currentItem) {
						inventorySlot.SetItem(item, 1);
						full = false;
						break;
					}
				}
			}

			if(full) {
				Debug.Log("Inventory Full");
				if(mode != 1) {
					DropItem(item, 1);
				}
			}
		}

		if(item.achievementNumber != -1) {
			achievementManager.GetAchievement(item.achievementNumber);
		}

		InventoryUpdate();
	}

	public void SetItem(Item item, int amount, int _slotID) {
		foreach(InventorySlot slot in slots) {
			if(slot.slotID == _slotID) {
				if(amount == -1) {
					slot.ClearItem();
				} else {
					slot.SetItem(item, amount);
				}
			}
		}
	}

	/* Trying to make a better system
		 * public void AddItem(Item item, int amount) {

		bool full = true;

		int amountLeft = amount;

		for(int i = 0; i < slots.Count; i++) {
			InventorySlot inventorySlot = slots[i].GetComponent<InventorySlot>();
			if(inventorySlot.currentItem) {
				if(inventorySlot.currentItem.id == item.id) {
					if(inventorySlot.stackCount < item.maxStackCount) {
						Debug.Log((int)Mathf.PingPong(amount, item.maxStackCount - inventorySlot.stackCount));
						int increaseAmount = (int)Mathf.PingPong(item.maxStackCount - inventorySlot.stackCount, amount);
						amountLeft -= increaseAmount;
						inventorySlot.IncreaseItem(increaseAmount);
						full = false;
						break;
					}
				}
			}
		}
		*/

	public void DropItem(Item item, int amount) {
		for(int i = 0; i < amount; i++) {
			GameObject itemObj = Instantiate(item.prefab, player.playerCamera.transform.position + player.playerCamera.transform.forward * 1.25f + Vector3.up * i * (item.prefab.GetComponentInChildren<Renderer>().bounds.size.y + 0.1f), player.playerCamera.transform.rotation);
			Rigidbody itemRB = itemObj.GetComponent<Rigidbody>();
			if(itemRB) {
				itemRB.velocity = player.rb.velocity;
			}
		}

		//InventoryUpdate();
		// InventoryUpdate is called in inventory slot now, to prevent duplication glich
	}

	int Ping(int num, int max) {
		if(num > max) {
			return max;
		} else {
			return num;
		}
	}

	void RemovePlace() { // Remove the current placing obj, and stop placing
		Destroy(currentPreviewObj);
		placingStructure = false;
		currentPlacingItem = null;
		currentPreviewObj = null;
	}

	public void Place(Item item) {

		if(placingStructure) {
			CancelStructurePlacement();
			player.HideNoticeText();
		}

		GameObject previewObj = Instantiate(item.previewPrefab, Vector3.up * -10000f, item.previewPrefab.transform.rotation) as GameObject;
		gridX = item.gridSize.x;
		gridY = item.gridSize.y;
		gridZ = item.gridSize.z;
		currentPreviewObj = previewObj;
		currentPlacingItem = item;
		placingStructure = true;
	}

	void AddAllItems() {
		foreach(Item item in saveManager.allItems) {
			AddItem(item, 1);
		}
	}

	public bool CheckRecipe(Recipe recipe) {
		if(player.ActiveSystemMenu()) {
			return false;
		}
		int[] inputAmounts = new int[recipe.inputs.Length];

		foreach(InventorySlot slot in slots) {
			if(slot.currentItem) {
				int i = 0;
				foreach(Item item in recipe.inputs) {
					if(slot.currentItem == item) {
						inputAmounts[i] += slot.stackCount;
						break;
					}
					i++;
				}
			}
		}

		bool canCraft = true;
		for(int i = 0; i < inputAmounts.Length; i++) {
			if(!(inputAmounts[i] >= recipe.amounts[i])) {
				canCraft = false;
			}
		}

		return canCraft;
	}

	public void ClearInventory() {
		for(int i = 0; i < slots.Count; i++) {
			InventorySlot inventorySlot = slots[i].GetComponent<InventorySlot>();
			if(inventorySlot.currentItem) {
				inventorySlot.ClearItem();
				break;
			}
		}

		InventoryUpdate();
	}

	public void ConstructRecipe(Recipe recipe) {
		
		int[] inputAmounts = new int[recipe.inputs.Length];

		foreach(InventorySlot slot in slots) {
			if(slot.currentItem) { // If the inventory slot has an item in it
				int i = 0;
				foreach(Item item in recipe.inputs) { // Loop through all the ingredients in the recipe to see if the slot's item is the same as one of them
					if(slot.currentItem == item) { // Is the item the same?
						int amountToDecrease = Mathf.Max(0, recipe.amounts[i] - inputAmounts[i]);
						inputAmounts[i] += slot.stackCount; // Add the amount of that item to the inputAmounts
						slot.DecreaseItem(amountToDecrease);
						break;
					}
					i++;
				}
			}
		}

		AddItem(recipe.output, recipe.outputAmount);

		int r = 0;
		foreach(Item replacementItem in recipe.replacementItems) {
			AddItem(replacementItem, recipe.replacementItemAmounts[r]);
			r++;
		}
	}

	public void InventoryUpdate() {
		currentSelectedItem = hotbar.GetChild(selectedHotbarSlot).GetComponent<InventorySlot>().currentItem;
		player.InventoryUpdate();
		foreach(Transform craftingRecipeObj in craftingRecipeContainer) {
			craftingRecipeObj.GetComponent<CraftingRecipe>().InventoryUpdate();
		}
	}
}