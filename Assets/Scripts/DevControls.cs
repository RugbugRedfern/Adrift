using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevControls : MonoBehaviour {

	Inventory inventory;

	SaveManager saveManager;

	void Awake() {
		inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		saveManager = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<SaveManager>();
	}

	void Update() {
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I)) {
			AddAllItems();
		}
	}

	void AddAllItems() {
		foreach(Item item in saveManager.allItems) {
			inventory.AddItem(item, 1);
		}
	}
}
