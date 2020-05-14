using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour {

	public ParticleSystem lightingParticles;
	public Item lightningStoneItem;
	public GameObject island;

	List<ParticleCollisionEvent> collisionEvents;

	float nextStoneSpawnTime = 0f;
	float stoneSpawnTime = 5f;

	void Start() {
		collisionEvents = new List<ParticleCollisionEvent>();
	}

	void OnParticleCollision(GameObject other) {
		if(Time.time >= nextStoneSpawnTime) {
			nextStoneSpawnTime = stoneSpawnTime + Time.time;
			ParticlePhysicsExtensions.GetCollisionEvents(lightingParticles, other, collisionEvents);

			for(int i = 0; i < collisionEvents.Count; i++) {
				if(collisionEvents[i].colliderComponent.gameObject == island) {
					GameObject stoneObj = Instantiate(lightningStoneItem.prefab, collisionEvents[i].intersection + Vector3.up * 0.3f, lightningStoneItem.prefab.transform.rotation) as GameObject;

					Rigidbody objRB = stoneObj.GetComponent<Rigidbody>();
					if(objRB) {
						objRB.AddExplosionForce(1f, collisionEvents[0].intersection - Vector3.up * 0.5f, 2f);
					}
					break;
				}
			}
		}
	}
}
