using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Save {
	public SerializableVector3 playerPosition;
	public SerializableQuaternion playerRotation;
	public float playerHealth;
	public float playerHunger;

	public List<SerializableVector3> worldItemPositions = new List<SerializableVector3>();
	public List<SerializableQuaternion> worldItemRotations = new List<SerializableQuaternion>();
	public List<int> worldItemIDs = new List<int>();
	public List<bool> worldItemHasRigidbodies = new List<bool>();

	public List<SerializableVector3> worldResourcePositions = new List<SerializableVector3>();
	public List<SerializableQuaternion> worldResourceRotations = new List<SerializableQuaternion>();
	public List<int> worldResourceIDs = new List<int>();
	public List<int> worldResourceHealths = new List<int>();

	public List<int> inventoryItemIDs = new List<int>();
	public List<int> inventoryItemAmounts = new List<int>();

	public List<int> achievementIDs = new List<int>();

	public List<ItemSaveData> itemSaveData = new List<ItemSaveData>();

	public List<SerializableVector3> smallIslandPositions = new List<SerializableVector3>();

	//public List<int> treeAssociation = new List<int>();

	public DateTime saveTime;

	public int worldType;

	public int difficulty;

	public int mode;

	public bool playerDead;

	public SerializableVector3 playerVelocity;
}

[Serializable]
public class ItemSaveData {

	public float fuel;
	public int itemID;
	public List<int> itemIDs;
	public List<int> itemAmounts;
	public int num;

}