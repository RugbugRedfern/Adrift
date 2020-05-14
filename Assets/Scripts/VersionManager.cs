using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionManager : MonoBehaviour {

	[SerializeField] string version;

	[SerializeField] Text versionText;
	[SerializeField] Text newVersionText;

	MenuManager menuManager;

	void Awake() {
		menuManager = FindObjectOfType<MenuManager>();
	}

	void Start() {
		versionText.text = "v" + version;
		//RemoteSettings.Completed += HandleRemoteSettings;
	}

	/*
	void HandleRemoteSettings(bool wasUpdatedFromServer, bool settingsChanged, int serverResponse) {
		string newestVersion = RemoteSettings.GetString("NewestVersion", "error");
		if(newestVersion == "error") {
			newVersionText.text = "Unable to check for newest version";
			newVersionText.gameObject.SetActive(true);
		} else if(newestVersion != version) {
			newVersionText.text = "New version of Adrift found: v" + newestVersion + ". Click to go to download page.";
			newVersionText.gameObject.SetActive(true);
		}
	}
	*/
}
