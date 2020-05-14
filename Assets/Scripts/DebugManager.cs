using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour {

	[SerializeField] GameObject debugMenu;
	[SerializeField] Text FPSText;

	int FPSHigh;
	int FPSLow = int.MaxValue;
	float FPSAvg;
	int FPS;

	bool debugActive;

	float nextTimeToUpdate;

	void Awake() {
		debugMenu.SetActive(debugActive);
	}

	void Update() {
		FPS = (int)(1f / Time.deltaTime);
		if(FPS > FPSHigh) {
			FPSHigh = FPS;
		}
		if(FPS < FPSLow && FPS >= 0) {
			FPSLow = FPS;
		}
		FPSAvg = FPS >= 0 ? (FPSAvg + FPS) / 2f : FPSAvg;

		if(Input.GetKeyDown(KeyCode.F1)) {
			debugActive = !debugActive;
			debugMenu.SetActive(debugActive);
		}
		if(debugActive && Time.time >= nextTimeToUpdate) {
			FPSText.text = string.Format("FPS: {0}\nHigh: {1}\nLow: {2}\nAvg: {3}\nTime: {4}", FPS, FPSHigh, FPSLow, FPSAvg.ToString("0.0"), Mathf.Floor(Time.time / 60f).ToString("00") + ":" + Mathf.Floor(Time.time % 60f).ToString("00"));
			nextTimeToUpdate = Time.time + 0.5f;
		}
	}
}
