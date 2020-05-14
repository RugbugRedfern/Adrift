using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Resource : ScriptableObject {
	public GameObject prefab;
	public int id;
	public Item[] resourceItems;
	public float[] chances;
	public int maxGathers;
	public float gatherTime;
	public bool infiniteGathers = false;
}
