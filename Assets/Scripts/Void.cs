using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour {

	public Transform worldSpawn;

	//int difficulty;

	void Start() {
		//difficulty = PlayerPrefs.GetInt("Difficulty");
	}

	void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Player")) {
			return;
		}
		other.transform.position = worldSpawn.position;
		Rigidbody otherRB = other.GetComponent<Rigidbody>();
		if(otherRB) {
			otherRB.velocity = Vector3.zero;
			otherRB.angularVelocity = Vector3.zero;
		}
	}

	void OnTriggerStay(Collider other) {
		if(other.CompareTag("Player")) {
			return;
		}
		other.transform.position = worldSpawn.position;
		Rigidbody otherRB = other.GetComponent<Rigidbody>();
		if(otherRB) {
			otherRB.velocity = Vector3.zero;
			otherRB.angularVelocity = Vector3.zero;
		}
	}
}
