using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallIsland : MonoBehaviour {

	public Vector3 moveAmount;

	public float speed = 0.5f;
	
	void Update () {
		transform.position += moveAmount * Time.deltaTime * speed;
		if(transform.localPosition.z >= 200f) {
			if(transform.childCount > 0) {
				Transform[] children = transform.GetComponentsInChildren<Transform>();

				foreach(Transform child in children) {
					if(!child.CompareTag("Player")) {
						Destroy(child.gameObject);
					}
				}
			}
			Destroy(gameObject);
		}
	}
}