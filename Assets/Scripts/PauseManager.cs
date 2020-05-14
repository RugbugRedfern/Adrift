using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {

	PlayerController player;

	public GameObject pauseMenu;

	float originalTimeScale;

	[HideInInspector] public bool paused = false;

	[SerializeField] Animator canvasAnim;

	void Start() {
		originalTimeScale = Time.timeScale;
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			if(paused) {
				Resume();
			} else {
				Pause();
			}
		}
	}

	void Pause() {
		canvasAnim.SetTrigger("PauseMenuEnter");
		Time.timeScale = 0f;
		paused = true;
		player.LockLook(true);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void Resume() {
		if(canvasAnim.GetCurrentAnimatorStateInfo(0).IsName("PauseMenuEnter")) {
			canvasAnim.SetTrigger("PauseMenuExit");
		}
		if(canvasAnim.GetCurrentAnimatorStateInfo(1).IsName("SettingsMenuEnter")) {
			canvasAnim.SetTrigger("SettingsMenuExit");
		}
		if(canvasAnim.GetCurrentAnimatorStateInfo(2).IsName("AchievementMenuEnter")) {
			canvasAnim.SetTrigger("AchievementMenuExit");
		}
		if(canvasAnim.GetCurrentAnimatorStateInfo(3).IsName("HelpMenuEnter")) {
			canvasAnim.SetTrigger("HelpMenuExit");
		}
		Time.timeScale = originalTimeScale;
		paused = false;
		player.LockLook(false);
		Cursor.lockState = CursorLockMode.Locked; // NEEDS TO CHECK IF INVENTORY IS OPEN
		Cursor.visible = false;
	}

	public void Menu() {
		paused = false;
		Time.timeScale = originalTimeScale;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Destroy(FindObjectOfType<PersistentData>().gameObject); // Destroy the persistent data object before we return to the menu, otherwise the game will save with a blank save name.
		SceneManager.LoadScene("Menu");
	}
}
