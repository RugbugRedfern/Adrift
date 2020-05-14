using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour {

	public TellParent tellParent;

	public GameObject fireLight;
	public ParticleSystem smoke;
	public ParticleSystem fire;

	[HideInInspector] public float fuel = 0;
	[HideInInspector] public Item currentSmeltingItem;

	[HideInInspector] public float finishTime;
	public float smeltTime = 10f;
	
	void Update() {
		if(tellParent.currentColliders.Count > 0) {
			foreach(Collider col in tellParent.currentColliders) {
				if(col && col.CompareTag("Item")) {
					ItemHandler itemHandler = col.GetComponent<ItemHandler>();
					if(itemHandler) {
						if(itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.fuel > 0) {
							Destroy(itemHandler.gameObject);
							fuel += itemHandler.item.fuel;
						} else if(itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.smeltItem && !currentSmeltingItem && fuel > 0) {
							StartSmelting(itemHandler.item);
							Destroy(itemHandler.gameObject);
						}
					}
				}
			}
		}
		if(fuel > 0 && currentSmeltingItem && Time.time >= finishTime) {
			DropItem(currentSmeltingItem.smeltItem);
			fuel--;
			StopSmelting();
		} else if(fuel < 0 && currentSmeltingItem) { // Not necessary unless fuel goes down on a timer
			DropItem(currentSmeltingItem);
			StopSmelting();
		}
	}

	void OnTriggerStay(Collider other) {
		if(other.CompareTag("Item")) {
			ItemHandler itemHandler = other.GetComponent<ItemHandler>();
			if(itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.fuel > 0) {
				Destroy(itemHandler.gameObject);
				fuel += itemHandler.item.fuel;
			} else if(itemHandler.item.type == Item.ItemType.Resource && itemHandler.item.smeltItem && !currentSmeltingItem && fuel > 0) {
				StartSmelting(itemHandler.item);
				Destroy(itemHandler.gameObject);
			}
		}
	}

	void StartSmelting(Item item) {
		finishTime = Time.time + smeltTime;
		currentSmeltingItem = item;
		smoke.Play();
		fire.Play();
		fireLight.SetActive(true);
	}

	void StopSmelting() {
		currentSmeltingItem = null;
		smoke.Stop();
		fire.Stop();
		fireLight.SetActive(false);
	}

	void DropItem(Item item) {
		GameObject smeltedItemObj = Instantiate(item.prefab, transform.position + transform.forward * 0.25f - transform.up * 0.75f, item.prefab.transform.rotation) as GameObject;
		Rigidbody objRB = smeltedItemObj.GetComponent<Rigidbody>();
		if(objRB) {
			objRB.velocity = transform.forward;
		}
	}
}