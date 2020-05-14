using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

	public enum WorldType {Light, Dark}

	public WorldType worldType;

	public bool spawnSmallIslands = true;
	public GameObject smallIslandPrefab;

	public GameObject world;
	public GameObject starterCratePrefab;
	public GameObject cratePrefab;

	public float bounds;

	public Vector3[] smallIslandSpawnLocations;
	//public GameObject copperOreResourcePrefab;
	public GameObject[] spawns;
	public int[] amountsMin;
	public int[] amountsMax;
	public float[] spawnTimes;

	public HiveMind hive;

	float[] nextSpawnTime;

	public float smallIslandSpawnTime = 60f;
	float nextSmallIslandSpawnTime;

	int difficulty;

	bool gameStarted;

	PersistentData persistentData;

	void Start() {
		persistentData = FindObjectOfType<PersistentData>();
		if(persistentData) {
			difficulty = persistentData.difficulty;
			if(!persistentData.loadSave) {
				GenerateWorld();
			}
		} else {
			GenerateWorld();
		}
		SetUpWorld();
	}

	void SetUpWorld() {
		nextSpawnTime = new float[spawnTimes.Length];

		int c = 0;
		foreach(float spawnTime in spawnTimes) {
			nextSpawnTime[c] = Time.time + spawnTimes[c];
			c++;
		}

		if(spawnSmallIslands) {
			nextSmallIslandSpawnTime = Time.time + smallIslandSpawnTime;
			SpawnSmallIsland();
		}
	}

	void GenerateWorld() {
		int i = 0;
		int b = 0;
		foreach(GameObject spawn in spawns) {
			int amount = Random.Range(amountsMin[i], amountsMax[i] + 1);
			for(int a = 0; a < amount + 1; a++) {
				Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * bounds, 300f, Random.insideUnitCircle.y * bounds));
				RaycastHit hit;
				if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1)) {
					if(hit.collider.gameObject == world) {
						GameObject obj = Instantiate(spawns[i], hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
						ResourceHandler handler = obj.GetComponent<ResourceHandler>();
						if(handler) {
							hive.AddResource(handler);
						}
						obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
					} else {
						a--;
					}
				} else {
					a--;
				}
				b++;
				if(b > 100) {
					b = 0;
					break;
				}
			}

			i++;
		}

		if(difficulty == 0 && worldType == WorldType.Light) {
			for(int d = 0; d < 2; d++) {
				Instantiate(starterCratePrefab, Vector3.up * 4f + Vector3.up * d, starterCratePrefab.transform.rotation);
			}
		}
	}

	void Update() {
		int i = 0;
		foreach(float spawnTime in nextSpawnTime) {
			if(Time.time >= spawnTime) {
				Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * bounds, 300f, Random.insideUnitCircle.y * bounds));
				RaycastHit hit;
				if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1)) {
					if(hit.collider.gameObject == world) {
						GameObject obj = Instantiate(spawns[i], hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
						hive.AddResource(obj.GetComponent<ResourceHandler>());
						obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
					} else {
						continue;
					}
				}

				nextSpawnTime[i] = Time.time + spawnTimes[i];
			}
			i++;
		}

		if(spawnSmallIslands) {
			if(Time.time >= nextSmallIslandSpawnTime) {
				SpawnSmallIsland();
				nextSmallIslandSpawnTime = Time.time + smallIslandSpawnTime;
			}
		}
	}

	void SpawnSmallIsland() {
		GameObject islandObj = Instantiate(smallIslandPrefab, transform.TransformPoint(smallIslandSpawnLocations[Random.Range(0, smallIslandSpawnLocations.Length)]), smallIslandPrefab.transform.rotation) as GameObject;
		Vector3 pos = new Vector3(Random.Range(-1f, 1f), 300f, Random.Range(-1f, 1f)) + islandObj.transform.position;
		RaycastHit hit;
		if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1)) {
			GameObject crateObj = Instantiate(cratePrefab, hit.point + Vector3.up * 0.5f, Quaternion.LookRotation(hit.normal)) as GameObject;
			crateObj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
		}
		//if(Random.Range(0, 2) == 0) {
		//	if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1)) {
		//		GameObject oreObj = Instantiate(copperOreResourcePrefab, hit.point, Quaternion.LookRotation(hit.normal), islandObj.transform) as GameObject;
		//		oreObj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
		//	}
		//} else {
			
		//}
	}
}
