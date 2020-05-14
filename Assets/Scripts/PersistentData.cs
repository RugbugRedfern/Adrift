using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour {

	public bool loadSave;
	public int saveToLoad;
	public int difficulty;
	public int mode;
	public string newSaveName;

	void Awake() {
		PersistentData[] pds = FindObjectsOfType<PersistentData>();

		if(pds.Length > 1) {
			foreach(PersistentData pd in pds) {
				if(pd != this) {
					Destroy(pd.gameObject);
				}
			}
		}

		DontDestroyOnLoad(gameObject);
	}
}
