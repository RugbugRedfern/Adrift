using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	public Vector3 rot;
	public float speed;
	
	void Update() {
		transform.Rotate(rot * speed * Time.deltaTime);
	}
}
