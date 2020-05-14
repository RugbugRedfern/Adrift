using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStone : MonoBehaviour {

	Renderer rn;

	float destructionTimer;
	float destructionTime = 60f;

	Color originalColor;
	Color endColor;

	void Awake() {
		rn = GetComponent<Renderer>();
		originalColor = rn.material.color;
		endColor = Color.white;
		endColor.a = 0f;
	}
	
	void Update() {
		destructionTimer += Time.deltaTime;
		if(destructionTimer / destructionTime >= 0.9f) { // 90% Complete
			rn.material.color = Color.Lerp(originalColor, endColor, ((destructionTimer / destructionTime) - 0.9f) / 0.1f);
			transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, ((destructionTimer / destructionTime) - 0.9f) / 0.1f);
		}
		if(destructionTimer >= destructionTime) {
			Destroy(gameObject);
		}
	}
}
