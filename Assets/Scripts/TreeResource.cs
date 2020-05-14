using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour {

	public GameObject applePrefab;
	public Transform[] appleSpawnLocations;

	public float appleSpawnChance = 0.8f;

	[HideInInspector] public List<GameObject> apples = new List<GameObject>();

	bool isQuitting = false;

	[HideInInspector] public bool spawnApples = true;

	void Start() {
		if(spawnApples) {
			foreach(Transform spawn in appleSpawnLocations) {
				if(Random.Range(0f, 1f) < appleSpawnChance) {
					GameObject appleObj = Instantiate(applePrefab, spawn.position, spawn.rotation) as GameObject;
					Rigidbody appleRB = appleObj.GetComponent<Rigidbody>();
					apples.Add(appleObj);
					if(appleRB) {
						Destroy(appleRB);
					}
				}
			}
		}
	}

	public void DropFruits() {
		foreach(GameObject apple in apples) {
			if(apple) {
				Instantiate(applePrefab, apple.transform.position, apple.transform.rotation);
				Destroy(apple);
			}
		}
	}
}
