using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuManager : MonoBehaviour {

	[SerializeField] GameObject loadingScreen;
	[SerializeField] Text loadingProgressText;
	[SerializeField] Text loadingProgressBlurb;
	[SerializeField] Image loadingProgressImage;


	public void OpenURL(string url) {
		Application.OpenURL(url);
	}

	public void LoadScene(string name) {
		StartCoroutine(LoadAsynchronously(name));
	}

	public void Quit() {
		Application.Quit();
	}

	public void OpenSavesFolder() {
		ShowExplorer(Application.persistentDataPath + "/saves/");
	}

	void ShowExplorer(string path) {
		path = path.Replace(@"/", @"\");
		System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
	}

	IEnumerator LoadAsynchronously(string name) {
		AsyncOperation operation = SceneManager.LoadSceneAsync(name);
		loadingScreen.SetActive(true);

		while(!operation.isDone) {
			float progress = Mathf.Clamp01(operation.progress / 0.9f);

			if(operation.progress >= 0.9f) {
				loadingProgressBlurb.text = "Activating world";
			} else {
				loadingProgressBlurb.text = "Loading assets";
			}

			loadingProgressText.text = (progress * 100f).ToString("0") + "%";
			loadingProgressImage.fillAmount = progress;

			yield return null;
		}
	}
}
