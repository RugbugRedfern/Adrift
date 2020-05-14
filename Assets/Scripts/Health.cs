using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float maxHealth;
	[HideInInspector] public float health;

	void Start () {
		health = maxHealth;
	}
	
	public void TakeDamage(float amount) {
		health -= amount;
		if(health <= 0f) {
			Die();
		}
	}

	void Die() {
		Destroy(gameObject);
	}
}
