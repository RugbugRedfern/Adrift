using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour {

	PersistentData persistentData;

	[SerializeField] Animator canvasAnim;

	public Text saveText;
	public Item[] allItems;
	public Resource[] allResources;
	[SerializeField] GameObject smallIslandPrefab;

	Inventory inventory;
	PlayerController player;

	AchievementManager achievementManager;

	WorldManager worldManager;

	HiveMind hive;

	public int difficulty;
	public int mode;

	FileInfo[] info;

	bool autoSave = true;
	float autoSaveInterval = 120f;
	float autoSaveTimer = 0f;

	void Awake() {
		hive = FindObjectOfType<HiveMind>();
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		worldManager = FindObjectOfType<WorldManager>();

		achievementManager = FindObjectOfType<AchievementManager>();
		inventory = playerObj.GetComponent<Inventory>();
		player = playerObj.GetComponent<PlayerController>();
		persistentData = FindObjectOfType<PersistentData>();
	}

	void Start() {
		CheckSaveDirectory();
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/saves");
		info = dir.GetFiles("*.*");
		if(persistentData) {
			if(persistentData.loadSave) {
				LoadGame(persistentData.saveToLoad);
			} else {
				difficulty = persistentData.difficulty;
				mode = persistentData.mode;
				SaveGame();
			}
			if(mode == 1) { // Creative mode
				inventory.LoadCreativeMode();
				player.LoadCreativeMode();
			}
		}
		if(autoSave) {
			autoSaveTimer = autoSaveInterval;
		}
	}

	void Update() {
		if(autoSave) {
			autoSaveTimer -= Time.deltaTime;
			if(autoSaveTimer <= 0f) {
				SaveGame();
				autoSaveTimer = autoSaveInterval;
			}
		}
	}

	Item FindItem(int id) {
		foreach(Item item in allItems) {
			if(item.id == id) {
				return item;
			}
		}
		Debug.LogError("Item with id " + id + " not found.");
		return null;
	}

	List<Item> IDsToItems(List<int> IDs) {
		List<Item> items = new List<Item>();
		foreach(int itemID in IDs) {
			items.Add(FindItem(itemID));
		}
		return items;
	}

	List<int> ItemsToIDs(List<Item> items) {
		List<int> IDs = new List<int>();
		foreach(Item item in items) {
			IDs.Add(item.id);
		}
		return IDs;
	}

	void CheckSaveDirectory() {
		if(!Directory.Exists(Application.persistentDataPath + "/saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}
	}

	public void LoadGame(int saveNum) {
		CheckSaveDirectory();
		if(File.Exists(Application.persistentDataPath + "/saves/" + info[saveNum].Name)) {
			ClearWorld();

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/saves/" + info[saveNum].Name, FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			for(int i = 0; i < save.worldItemIDs.Count; i++) {
				foreach(Item item in allItems) {
					if(item.id == save.worldItemIDs[i]) {
						GameObject itemObj = Instantiate(item.prefab, save.worldItemPositions[i], save.worldItemRotations[i]) as GameObject;
						if(!save.worldItemHasRigidbodies[i]) {
							Rigidbody itemRB = itemObj.GetComponentInParent<Rigidbody>();
							if(itemRB) {
								Destroy(itemRB);
							}
						}
						Furnace furnace = itemObj.GetComponent<Furnace>();
						AutoMiner autoMiner = itemObj.GetComponent<AutoMiner>();
						AutoSorter autoSorter = itemObj.GetComponent<AutoSorter>();
						ConveyorBelt conveyerBelt = itemObj.GetComponent<ConveyorBelt>();
						Radio radio = itemObj.GetComponent<Radio>();
						LightItem li = itemObj.GetComponent<LightItem>();
						if(furnace) {
							furnace.fuel = save.itemSaveData[i].fuel;
							if(save.itemSaveData[i].itemID != -1) {
								furnace.currentSmeltingItem = FindItem(save.itemSaveData[i].itemID);
								furnace.finishTime = furnace.smeltTime + Time.time;
							}
						}
						if(autoMiner) {
							autoMiner.items = IDsToItems(save.itemSaveData[i].itemIDs);
							autoMiner.itemAmounts = save.itemSaveData[i].itemAmounts;
							if(save.itemSaveData[i].itemID != -1) {
								autoMiner.SetTool(FindItem(save.itemSaveData[i].itemID));
							}
						}
						if(autoSorter) {
							if(save.itemSaveData[i].itemID != -1) {
								autoSorter.SetItem(FindItem(save.itemSaveData[i].itemID));
							}
						}
						if(conveyerBelt) {
							conveyerBelt.SetSpeed(save.itemSaveData[i].num);
						}
						if(radio) {
							radio.SetSong(save.itemSaveData[i].num);
						}
						if(li) {
							li.SetIntensity(save.itemSaveData[i].num);
						}
					}
				}
			}

			for(int i = 0; i < save.worldResourceIDs.Count; i++) {
				foreach(Resource resource in allResources) {
					if(resource.id == save.worldResourceIDs[i]) {
						if(resource.prefab) {
							GameObject resourceObj = Instantiate(resource.prefab, save.worldResourcePositions[i], save.worldResourceRotations[i]) as GameObject;
							ResourceHandler handler = resourceObj.GetComponent<ResourceHandler>();
							if(handler) {
								hive.AddResource(handler);
							}
							resourceObj.GetComponent<ResourceHandler>().health = save.worldResourceHealths[i];
							TreeResource tree = resourceObj.GetComponent<TreeResource>();
							if(tree) {
								tree.spawnApples = false;
							}
						}
					}
				}
			}

			for(int i = 0; i < save.inventoryItemIDs.Count; i++) {
				foreach(Item item in allItems) {
					if(item.id == save.inventoryItemIDs[i]) {
						inventory.SetItem(item, save.inventoryItemAmounts[i], i);
					}
				}
			}

			foreach(Vector3 pos in save.smallIslandPositions) {
				Instantiate(smallIslandPrefab, pos, smallIslandPrefab.transform.rotation);
			}

			player.transform.position = save.playerPosition;
			player.transform.rotation = save.playerRotation;
			player.hunger = save.playerHunger;
			player.health = save.playerHealth;

			if(save.playerDead) {
				player.Die();
			} else {
				if(save.worldType == 0) {
					player.LoadLightWorld();
				} else {
					player.LoadDarkWorld();
				}
			}

			player.rb.velocity = save.playerVelocity;

			difficulty = save.difficulty;
			mode = save.mode;

			achievementManager.SetAchievements(save.achievementIDs);

			inventory.InventoryUpdate();

			saveText.text = "Game loaded from " + save.saveTime.ToString("HH:mm MMMM dd, yyyy");
		} else {
			saveText.text = "No game save found";
		}
	}

	public void SaveGame() {
		canvasAnim.SetTrigger("Save");
		CheckSaveDirectory();
		Save save = CreateSave();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(persistentData.loadSave) {
			file = File.Create(Application.persistentDataPath + "/saves/" + info[persistentData.saveToLoad].Name);
		} else {
			file = File.Create(Application.persistentDataPath + "/saves/" + persistentData.newSaveName + ".save");
		}
		bf.Serialize(file, save);
		file.Close();

		saveText.text = "Game saved at " + DateTime.Now.ToString("HH:mm MMMM dd, yyyy");
	}

	private Save CreateSave() {
		Save save = new Save();

		save.playerPosition = player.transform.position;
		save.playerRotation = player.transform.rotation;
		save.playerHealth = player.health;
		save.playerHunger = player.hunger;
		save.saveTime = DateTime.Now;

		if(player.currentWorld == WorldManager.WorldType.Light) {
			save.worldType = 0;
		} else {
			save.worldType = 1;
		}

		foreach(InventorySlot slot in inventory.slots) {
			if(slot.currentItem) {
				save.inventoryItemIDs.Add(slot.currentItem.id);
				save.inventoryItemAmounts.Add(slot.stackCount);
			} else {
				save.inventoryItemIDs.Add(-1);
				save.inventoryItemAmounts.Add(-1);
			}
		}

		List<ItemHandler> usedItemHandlers = new List<ItemHandler>();

		foreach(GameObject itemObj in GameObject.FindGameObjectsWithTag("Item")) {
			ItemHandler handler = itemObj.GetComponentInParent<ItemHandler>();
			if(!usedItemHandlers.Contains(handler)) {
				save.worldItemIDs.Add(handler.item.id);
				// Saving Item Data
				ItemSaveData itemSaveData = new ItemSaveData();
				Furnace furnace = handler.GetComponent<Furnace>(); // Better way to do this would be to check item ids.
				AutoMiner autoMiner = handler.GetComponent<AutoMiner>();
				AutoSorter autoSorter = handler.GetComponent<AutoSorter>();
				ConveyorBelt conveyerBelt = handler.GetComponent<ConveyorBelt>();
				Radio radio = handler.GetComponent<Radio>();
				LightItem li = handler.GetComponent<LightItem>();
				if(furnace) {
					itemSaveData.fuel = furnace.fuel;
					if(furnace.currentSmeltingItem) {
						itemSaveData.itemID = furnace.currentSmeltingItem.id;
					} else {
						itemSaveData.itemID = -1;
					}
				}
				if(autoMiner) {
					itemSaveData.itemIDs = ItemsToIDs(autoMiner.items);
					itemSaveData.itemAmounts = autoMiner.itemAmounts;
					if(autoMiner.currentToolItem) {
						itemSaveData.itemID = autoMiner.currentToolItem.id;
					} else {
						itemSaveData.itemID = -1;
					}
				}
				if(autoSorter) {
					if(autoSorter.sortingItem) {
						itemSaveData.itemID = autoSorter.sortingItem.id;
					} else {
						itemSaveData.itemID = -1;
					}
				}
				if(conveyerBelt) {
					itemSaveData.num = conveyerBelt.speedNum;
				}
				if(radio) {
					itemSaveData.num = radio.songNum;
				}
				if(li) {
					itemSaveData.num = li.intensityNum;
				}
				/*
				if(handler.item.id == 26) { // Apple
					apples.Add(handler.gameObject);
					save.treeAssociation.Add(-1);
				} else {
					save.treeAssociation.Add(-2);
				}
				*/
				
				save.itemSaveData.Add(itemSaveData);
				// Done saving item data
				save.worldItemPositions.Add(itemObj.transform.position);
				save.worldItemRotations.Add(itemObj.transform.rotation);
				Rigidbody itemRB = itemObj.GetComponentInParent<Rigidbody>();
				save.worldItemHasRigidbodies.Add(itemRB);
				usedItemHandlers.Add(handler);
			}
		}

		List<ResourceHandler> usedResourceHandlers = new List<ResourceHandler>();

		foreach(GameObject resourceObj in GameObject.FindGameObjectsWithTag("Resource")) {
			ResourceHandler handler = resourceObj.GetComponentInParent<ResourceHandler>();
			if(!usedResourceHandlers.Contains(handler)) {
				save.worldResourceIDs.Add(handler.resource.id);
				save.worldResourcePositions.Add(resourceObj.transform.position);
				save.worldResourceRotations.Add(resourceObj.transform.rotation);
				save.worldResourceHealths.Add(handler.health);
				/*
				if(handler.resource.id == 5) { // Tree
					TreeResource tr = handler.GetComponent<TreeResource>();
					if(tr.apples.Count >= 1) {
						// AHH DO NOT LET THE APPLES FALL OUT OF THE TREE
					}
				}
				*/
				usedResourceHandlers.Add(handler);
			}
		}

		save.achievementIDs = achievementManager.ObtainedAchievements();

		foreach(SmallIsland si in FindObjectsOfType<SmallIsland>()) {
			save.smallIslandPositions.Add(si.transform.position);
		}

		save.difficulty = difficulty;
		save.mode = mode;

		save.playerDead = player.dead;

		save.playerVelocity = player.rb.velocity;

		return save;
	}

	void ClearWorld() {
		foreach(GameObject itemObj in GameObject.FindGameObjectsWithTag("Item")) {
			Destroy(itemObj);
		}

		foreach(GameObject resourceObj in GameObject.FindGameObjectsWithTag("Resource")) {
			if(resourceObj.GetComponent<ResourceHandler>().resource.prefab) {
				Destroy(resourceObj);
			}
		}

		foreach(InventorySlot slot in inventory.slots) {
			slot.ClearItem();
		}
	}

}

[System.Serializable]
public struct SerializableVector3 {

	public float x;
	public float y;
	public float z;

	public SerializableVector3(float rX, float rY, float rZ) {
		x = rX;
		y = rY;
		z = rZ;
	}

	public override string ToString() {
		return String.Format("[{0}, {1}, {2}]", x, y, z);
	}

	public static implicit operator Vector3(SerializableVector3 rValue) {
		return new Vector3(rValue.x, rValue.y, rValue.z);
	}

	public static implicit operator SerializableVector3(Vector3 rValue) {
		return new SerializableVector3(rValue.x, rValue.y, rValue.z);
	}
}

[System.Serializable]
public struct SerializableQuaternion {

	public float x;
	public float y;
	public float z;
	public float w;

	public SerializableQuaternion(float rX, float rY, float rZ, float rW) {
		x = rX;
		y = rY;
		z = rZ;
		w = rW;
	}

	public override string ToString() {
		return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
	}

	public static implicit operator Quaternion(SerializableQuaternion rValue) {
		return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
	}

	public static implicit operator SerializableQuaternion(Quaternion rValue) {
		return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
	}
}

// https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html