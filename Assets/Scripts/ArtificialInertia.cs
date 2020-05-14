using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialInertia : MonoBehaviour {

	public Transform root; // Optional to set custom root in inspector

	void Awake() {
		if(!root) {
			root = transform.root;
		}
	}

	void OnCollisionEnter(Collision col) {
		//CustomTags customTags = col.gameObject.GetComponent<CustomTags>();
		if(root.parent != col.transform && col.gameObject.name == "SmallIsland(Clone)") {
			root.SetParent(col.transform);
		}
	}

	void OnCollisionStay(Collision col) {
		//CustomTags customTags = col.gameObject.GetComponent<CustomTags>();
		if(root.parent != col.transform && col.gameObject.name == "SmallIsland(Clone)") {
			root.SetParent(col.transform);
		}
	}

	void OnCollisionExit(Collision col) {
		root.SetParent(null);
	}
}