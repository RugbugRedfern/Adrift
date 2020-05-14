using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject {
	public enum ItemType {Resource, Tool, Structure, Food, Weapon}
	public string itemName;
	public string itemDescription;
	public ItemType type;
	public int id;
	public Sprite icon;
	public GameObject prefab;
	public float timeToGather = 0.25f;
	public Vector3 handRotation;
	public Vector3 handScale = Vector3.one;
	public int maxStackCount;
	public GameObject previewPrefab;
	public float speed;
	public int gatherAmount;
	public float power;
	public float fuel;
	public Item smeltItem;
	public bool alignToNormal;
	public Vector3 gridSize;
	public Vector3[] rots;
	public int achievementNumber = -1;
	public float calories;
}
