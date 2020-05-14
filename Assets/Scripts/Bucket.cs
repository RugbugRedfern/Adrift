using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour {

	[SerializeField] bool waterBucket;

	[SerializeField] Item bucketItem;
	[SerializeField] Item waterBucketItem;

	[SerializeField] Transform waterLevel;

	WorldManager.WorldType worldType;

	float fillTime;
	float timeToFill = 10f;

	float nextTimeToWorldCheck;
	float worldCheckInterval = 1f;

	void Start() {
		if(waterBucket) {
			fillTime = timeToFill;
		}
	}

	void Update() {
		if(worldType == WorldManager.WorldType.Dark) {
			fillTime += Time.deltaTime * Mathf.Max(0f, transform.up.y);
		}
		if(transform.up.y <= 0.6f) {
			fillTime += Time.deltaTime + Mathf.Min(0f, (transform.up.y - 0.65f) / 2f);
		}
		waterLevel.transform.localPosition = Vector3.Lerp(Vector3.up * -0.49f, Vector3.zero, fillTime / timeToFill);
		waterLevel.transform.localScale = Vector3.Lerp(Vector3.one * 0.82f, Vector3.one, fillTime / timeToFill);
		if(fillTime <= 0f) {
			if(waterBucket) {
				EmptyWater();
			} else {
				fillTime = 0f;
			}
		} else if(fillTime >= timeToFill) {
			if(waterBucket) {
				fillTime = timeToFill;
			} else {
				FillWithWater();
			}
		}

		/*
		if(waterBucket) {
			if(transform.up.y <= 0.6f) { // If the bucket is tipped
				fillTime += Time.deltaTime * transform.up.y - 0.2f; // Slowly empty it
				waterLevel.transform.localPosition = Vector3.Lerp(Vector3.up * -0.49f, Vector3.zero, fillTime / timeToFill);
				waterLevel.transform.localScale = Vector3.Lerp(Vector3.one * 0.82f, Vector3.one, fillTime / timeToFill);
				if(fillTime <= 0f) {
					EmptyWater();
				}
			} else if(worldType == WorldManager.WorldType.Dark) {
				fillTime += Time.deltaTime * Mathf.Max(0.6f, transform.up.y);
				if(fillTime > timeToFill) {
					fillTime = timeToFill;
				}
			}
		} else {
			if(worldType == WorldManager.WorldType.Dark) {
				fillTime += Time.deltaTime * transform.up.y;
				waterLevel.transform.localPosition = Vector3.Lerp(Vector3.up * -0.49f, Vector3.zero, fillTime / timeToFill);
				waterLevel.transform.localScale = Vector3.Lerp(Vector3.one * 0.82f, Vector3.one, fillTime / timeToFill);
				if(fillTime < 0f) {
					fillTime = 0f;
				}
				if(fillTime >= timeToFill) {
					FillWithWater();
				}
			}
		}
		*/

		if(fillTime <= 0f) {
			if(waterLevel.gameObject.activeSelf) {
				waterLevel.gameObject.SetActive(false);
			}
		} else {
			if(!waterLevel.gameObject.activeSelf) {
				waterLevel.gameObject.SetActive(true);
			}
		}

		if(Time.time >= nextTimeToWorldCheck) {
			CheckWorldType();
			nextTimeToWorldCheck = Time.time + worldCheckInterval;
		}
	}

	void CheckWorldType() {
		if(transform.position.x < 5000f) {
			worldType = WorldManager.WorldType.Light;
		} else {
			worldType = WorldManager.WorldType.Dark;
		}
	}

	void EmptyWater() {
		Instantiate(bucketItem.prefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}

	void FillWithWater() {
		Instantiate(waterBucketItem.prefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}
