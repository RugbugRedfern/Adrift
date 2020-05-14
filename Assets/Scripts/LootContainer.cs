using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootContainer : MonoBehaviour {

	public LootItem[] loot;

	public GameObject lootParticles;
	
	public void Open() {
		int i = 0;
		foreach(LootItem lootItem in loot) {
			int amount = Random.Range(lootItem.minAmount, lootItem.maxAmount + 1);
			for(int a = 0; a < amount; a++) {
				if(Random.Range(0f, 1f) <= lootItem.chance) {
					GameObject itemObj = Instantiate(lootItem.item.prefab, transform.position + Vector3.up * 0.3f + Random.onUnitSphere * 0.2f, lootItem.item.prefab.transform.rotation) as GameObject;
					Rigidbody itemRB = itemObj.GetComponent<Rigidbody>();
					if(itemRB) {
						itemRB.AddExplosionForce(3f, transform.position, 2f);
					}
				}
			}
			i++;
		}
		GameObject obj = Instantiate(lootParticles, transform.position, Quaternion.identity) as GameObject;
		Destroy(obj, 1f);
		Destroy(gameObject);
	}
}

[System.Serializable]
public struct LootItem {
	public Item item;
	public int minAmount;
	public int maxAmount;
	public float chance;
}