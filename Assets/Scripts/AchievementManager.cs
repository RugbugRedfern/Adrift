using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour {

	public Transform achievementContainer;
	public Transform acievementList;
	public GameObject achievementPrefab;

	bool[] hasAchievements;
	public Achievement[] achievements;

	AchievementHandler[] achievementHandlers;

	PlayerController player;

	void Awake() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		hasAchievements = new bool[achievements.Length];
		achievementHandlers = new AchievementHandler[achievements.Length];

		for(int i = 0; i < achievements.Length; i++) {
			GameObject achievementObj = Instantiate(achievementPrefab, acievementList) as GameObject;
			AchievementHandler handler = achievementObj.GetComponent<AchievementHandler>();
			handler.achievementNameText.text = achievements[i].achievementName;
			handler.achievementDescText.text = achievements[i].achievementDesc;
			handler.achievementIconImage.sprite = achievements[i].achievementIcon;
			handler.achievement = achievements[i];
			achievementHandlers[i] = handler;
		}
	}

	public void ShowAchievement(int achievementID) {
		GameObject achievementObj = Instantiate(achievementPrefab, achievementContainer) as GameObject;
		AchievementHandler handler = achievementObj.GetComponent<AchievementHandler>();
		int i = 0;
		foreach(Achievement achievement in achievements) {
			if(achievements[i].achievementID == achievementID) {
				handler.achievementNameText.text = achievements[i].achievementName;
				handler.achievementDescText.text = achievements[i].achievementDesc;
				handler.achievementIconImage.sprite = achievements[i].achievementIcon;
				break;
			}
			i++;
		}
	}

	public void SetAchievements(List<int> achievementIDs) {
		for(int i = 0; i < achievementIDs.Count; i++) {
			foreach(Achievement achievement in achievements) {
				if(achievement.achievementID == achievementIDs[i]) {
					hasAchievements[achievement.achievementID] = true;
					break;
				}
			}

			foreach(AchievementHandler handler in achievementHandlers) {
				if(handler.achievement.achievementID == achievementIDs[i]) {
					handler.backgroundImage.color = Color.green;
				}
			}
		}
	}

	public void GetAchievement(int _achievementID) {
		if(!hasAchievements[_achievementID]) {
			ShowAchievement(_achievementID);

			int i = 0;
			foreach(Achievement achievement in achievements) {
				if(achievements[i].achievementID == _achievementID) {
					hasAchievements[_achievementID] = true;
					break;
				}
				i++;
			}

			foreach(AchievementHandler handler in achievementHandlers) { // Set the achievement background to green in the achievement UI
				if(handler.achievement.achievementID == _achievementID) {
					handler.backgroundImage.color = Color.green;
				}
			}
		}
	}

	public List<int> ObtainedAchievements() {
		List<int> a = new List<int>();
		for(int i = 0; i < hasAchievements.Length; i++) {
			if(hasAchievements[i]) {
				a.Add(i);
			}
		}
		return a;
	}
}