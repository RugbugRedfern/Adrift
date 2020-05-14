using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class SettingsManager : MonoBehaviour {

	public GameObject settingsMenu;

	[SerializeField] Toggle postProcessingToggle;

	[SerializeField] Dropdown fullscreenDropdown;

	[SerializeField] Dropdown graphicsDropdown;

	[SerializeField] Dropdown resolutionDropdown;

	[SerializeField] Slider FOVSlider;
	[SerializeField] Text FOVText;

	[SerializeField] Slider volumeSlider;
	[SerializeField] Text volumeText;

	[SerializeField] Slider mouseSensitivitySlider;
	[SerializeField] Text mouseSensitivityText;

	[SerializeField] bool mainMenu;

	PlayerController player;

	Camera playerCamera;

	public AudioMixer audioMixer;

	Resolution[] resolutions;

	void Awake() {
		if(!mainMenu) {
			player = FindObjectOfType<PlayerController>();
			playerCamera = player.playerCamera.GetComponent<Camera>();
		}
		resolutions = Screen.resolutions;
		System.Array.Reverse(resolutions);
	}

	void DisplayResolutionOptions() {
		resolutionDropdown.ClearOptions();
		List<string> resolutionNames = new List<string>();

		int currentResolutionIndex = 0;
		for(int i = 0; i < resolutions.Length; i++) {
			string option = resolutions[i].width + "x" + resolutions[i].height;
			resolutionNames.Add(option);
			if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
				currentResolutionIndex = i;
			}
		}

		resolutionDropdown.AddOptions(resolutionNames);
		resolutionDropdown.value = currentResolutionIndex;
	}

	void Start() {
		DisplayResolutionOptions();

		if(!PlayerPrefs.HasKey("Settings.FOV")) {
			if(playerCamera != null)
			{
				PlayerPrefs.SetFloat("Settings.FOV", playerCamera.fieldOfView);
			}
		}
		if(!PlayerPrefs.HasKey("Settings.PostProcessingEnabled")) {
			PlayerPrefs.SetInt("Settings.PostProcessingEnabled", 1);
		}
		if(!PlayerPrefs.HasKey("Settings.Volume")) {
			PlayerPrefs.SetFloat("Settings.Volume", 0f);
		}
		if(!PlayerPrefs.HasKey("Settings.Graphics")) {
			PlayerPrefs.SetInt("Settings.Graphics", QualitySettings.GetQualityLevel());
		}
		if(!PlayerPrefs.HasKey("Settings.Fullscreen")) {
			PlayerPrefs.SetInt("Settings.Fullscreen", 0); // Set to fullscreen by default
		}
		if(!PlayerPrefs.HasKey("Settings.Resolution")) {
			PlayerPrefs.SetInt("Settings.Resolution", resolutions.Length - 1);
		}
		if(!PlayerPrefs.HasKey("Settings.MouseSensitivity")) {
			PlayerPrefs.SetFloat("Settings.MouseSensitivity", 3.5f);
		}

		UpdateUI();
		ApplySettings();
	}

	void UpdateUI() {
		postProcessingToggle.isOn = PlayerPrefs.GetInt("Settings.PostProcessingEnabled") == 1;
		FOVSlider.value = PlayerPrefs.GetFloat("Settings.FOV");
		FOVText.text = "FOV: " + FOVSlider.value.ToString();
		volumeSlider.value = PlayerPrefs.GetFloat("Settings.Volume");
		volumeText.text = "Volume: " + ((volumeSlider.value + 80f) / 80f * 100f).ToString("0") + "%";
		graphicsDropdown.value = PlayerPrefs.GetInt("Settings.Graphics");
		resolutionDropdown.value = PlayerPrefs.GetInt("Settings.Resolution");
		fullscreenDropdown.value = PlayerPrefs.GetInt("Settings.Fullscreen");
		mouseSensitivitySlider.value = PlayerPrefs.GetFloat("Settings.MouseSensitivity");
		mouseSensitivityText.text = "Sensitivity: " + mouseSensitivitySlider.value.ToString("0.0");
	}

	void ApplySettings() {
		SetFOV(PlayerPrefs.GetFloat("Settings.FOV"));
		SetPostProcessing(PlayerPrefs.GetInt("Settings.PostProcessingEnabled") == 1);
		SetVolume(PlayerPrefs.GetFloat("Settings.Volume"));
		SetGraphics(PlayerPrefs.GetInt("Settings.Graphics"));
		SetFullscreen(PlayerPrefs.GetInt("Settings.Fullscreen"));
		SetResolution(PlayerPrefs.GetInt("Settings.Resolution"));
		SetMouseSensitivity(PlayerPrefs.GetFloat("Settings.MouseSensitivity"));
	}

	public void SetPostProcessing(bool n) {
		if(!mainMenu) {
			player.playerCameraPostProcessingBehaviour.enabled = n;
		} else {
			Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = n;
		}
		PlayerPrefs.SetInt("Settings.PostProcessingEnabled", n ? 1 : 0);
	}

	public void SetFOV(float n) {
		if(!mainMenu) {
			playerCamera.fieldOfView = n;
		}
		FOVText.text = "FOV: " + n;
		PlayerPrefs.SetFloat("Settings.FOV", n);
	}

	public void SetVolume(float n) {
		volumeText.text = "Volume: " + ((n + 80f) / 80f * 100f).ToString("0") + "%";
		audioMixer.SetFloat("volume", n);
		PlayerPrefs.SetFloat("Settings.Volume", n);
	}

	public void SetResolution(int n) {
		Screen.SetResolution(resolutions[n].width, resolutions[n].height, Screen.fullScreen);
		PlayerPrefs.SetInt("Settings.Resolution", n);
	}

	public void SetGraphics(int n) {
		QualitySettings.SetQualityLevel(n, true);
		PlayerPrefs.SetInt("Settings.Graphics", n);
	}

	public void SetFullscreen(int n) {
		Screen.fullScreenMode = (FullScreenMode)n;
		PlayerPrefs.SetInt("Settings.Fullscreen", n);
	}

	public void SetMouseSensitivity(float n) {
		if(!mainMenu) {
			player.mouseSensitivityX = n;
			player.mouseSensitivityY = n;
		}
		mouseSensitivityText.text = "Sensitivity: " + n.ToString("0.0");
		PlayerPrefs.SetFloat("Settings.MouseSensitivity", n);
	}
}
