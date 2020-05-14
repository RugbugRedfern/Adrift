using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour {

	public Resource resource;

	public int health;

	HiveMind hive;

	void Start() {
		if(health == 0) {
			health = resource.maxGathers;
		}
		hive = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<HiveMind>();
	}

	public void Gather(int amount) {
		health -= amount;
		if(health <= 0 && !resource.infiniteGathers) {
			hive.RemoveResource(this);
			if(resource.id == 5) { // This resource is a tree
				GetComponent<TreeResource>().DropFruits();
			}
			Destroy(gameObject);
		}
	}
}
