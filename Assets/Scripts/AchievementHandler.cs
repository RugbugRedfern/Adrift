using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementHandler : MonoBehaviour {

	float timeToExit;

	public Achievement achievement;

	public Image backgroundImage;
	public Text achievementNameText;
	public Text achievementDescText;
	public Image achievementIconImage;

	void Start() {
		timeToExit = Time.time + 5f;
	}

	void Update() {
		if(Time.time >= timeToExit) {
			GetComponent<Animator>().SetTrigger("Exit");
			Destroy(gameObject, 1f);
			timeToExit = Mathf.Infinity;
		}
	}
}
