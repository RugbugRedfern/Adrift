using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour {

	public PlayerController playerController;

	void OnTriggerEnter(Collider col) {
		playerController.grounded = true;
	}

	void OnTriggerExit(Collider col) {
		playerController.grounded = false;
	}

	void OnTriggerStay(Collider col) {
		playerController.grounded = true;
	}

	void OnCollisionEnter(Collision col) {
		playerController.grounded = true;
	}

	void OnCollisionExit(Collision col) {
		playerController.grounded = false;
	}

	void OnCollisionStay(Collision col) {
		playerController.grounded = true;
	}
}
