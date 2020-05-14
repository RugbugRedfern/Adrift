using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuSaveManager : MonoBehaviour {

	PersistentData persistentData;

	[SerializeField] GameObject saveListItem;
	[SerializeField] GameObject saveList;

	[SerializeField] Text errorText;

	[SerializeField] Animator canvasAnim;

	[SerializeField] InputField saveNameInputField;
	[SerializeField] Dropdown saveDifficultyDropdown;
	[SerializeField] Dropdown saveModeDropdown;
	[SerializeField] Text difficultyBlurb;
	[SerializeField] Text modeBlurb;
	[SerializeField] Text saveErrorText;

	SaveManager saveManager;

	FileInfo[] info;

	MenuManager menuManager;

	List<GameObject> saveListItems = new List<GameObject>();

	string[] difficultyBlurbs = {"Easy: Keep your inventory items when you die and start with 2 crates.",
		"Normal: Keep your inventory items when you die.",
		"Hard: Lose your inventory items when you die."};

	string[] modeBlurbs = {"Survival: Survive on the island.",
		"Creative: Health and hunger disabled, with an infinite supply of all items and the ability to fly."};

	void Awake() {
		menuManager = FindObjectOfType<MenuManager>();
	}

	void Start() {
		persistentData = FindObjectOfType<PersistentData>();
		if(!Directory.Exists(Application.persistentDataPath + "/saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/saves");
		info = dir.GetFiles("*.*");
		RenderList();
		OnChangeDifficulty();
		OnChangeMode();
	}

	void RenderList() {
		saveNameInputField.text = "World " + (info.Length + 1);
		for(int i = 0; i < info.Length; i++) {
			int saveNum = i;
			GameObject go = Instantiate(saveListItem, saveList.transform);
			saveListItems.Add(go);
			string[] infoSplit = info[saveNum].Name.Split('.');
			go.GetComponentInChildren<Text>().text = infoSplit[infoSplit.Length - 2];
			go.GetComponent<Button>().onClick.AddListener(delegate { LoadSave(saveNum); });
			Button confirmDeleteButton = go.transform.Find("ConfirmDeleteButton").GetComponent<Button>();
			confirmDeleteButton.onClick.AddListener(delegate { DeleteSave(saveNum); });
		}

		CreateNewSaveButton();
	}

	void ClearList() {
		foreach(GameObject go in saveListItems) {
			Destroy(go);
		}
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/saves");
		info = dir.GetFiles("*.*");
		saveListItems.Clear();
	}

	void CreateNewSaveButton() {
		GameObject go = Instantiate(saveListItem, saveList.transform);
		saveListItems.Add(go);
		go.GetComponentInChildren<Text>().text = "New Save";
		go.GetComponent<Button>().onClick.AddListener(delegate { OpenNewSaveMenu(); });
		go.GetComponent<Image>().color = Color.Lerp(Color.green, Color.white, 0.5f);
		Destroy(go.transform.Find("DeleteButton").gameObject);
		RectTransform rt = go.transform.Find("SaveText").GetComponent<RectTransform>();
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
	}

	public void LoadSave(int saveNum) {
		persistentData.loadSave = true;
		persistentData.saveToLoad = saveNum;
		Debug.Log("Loading save " + saveNum);
		menuManager.LoadScene("World");
	}

	public void DeleteSave(int saveNum) {
		foreach(GameObject go in saveListItems) {
			if(go.GetComponentInChildren<Text>().text == info[saveNum].Name) {
				Destroy(go);
				break;
			}
		}
		File.Delete(Application.persistentDataPath + "/saves/" + info[saveNum].Name);
		ClearList();
		RenderList();
	}

	public void OpenNewSaveMenu() {
		canvasAnim.SetTrigger("NewSaveMenuEnter");
		canvasAnim.SetTrigger("PlayMenuExit");
		Invoke("SelectSaveNameInputField", 0.1f); // Kinda hacky, but gets the job done :)
	}

	public void SelectSaveNameInputField() {
		saveNameInputField.Select();
		saveNameInputField.ActivateInputField();
	}

	public void OnChangeDifficulty() {
		difficultyBlurb.text = difficultyBlurbs[saveDifficultyDropdown.value];
	}

	public void OnChangeMode() {
		modeBlurb.text = modeBlurbs[saveModeDropdown.value];
	}

	public void CreateNewSave() {
		if(string.IsNullOrEmpty(saveNameInputField.text)) {
			saveErrorText.text = "Please enter a valid name.";
			saveErrorText.gameObject.SetActive(true);
			return;
		} else {
			foreach(FileInfo f in info) {
				string[] infoSplit = f.Name.Split('.');
				if(infoSplit[infoSplit.Length - 2] == saveNameInputField.text) {
					saveErrorText.text = "Please enter a name that isn't taken.";
					saveErrorText.gameObject.SetActive(true);
					return;
				}
			}
		}
		persistentData.newSaveName = saveNameInputField.text;
		persistentData.difficulty = saveDifficultyDropdown.value;
		persistentData.mode = saveModeDropdown.value;
		persistentData.loadSave = false;
		persistentData.saveToLoad = info.Length;
		menuManager.LoadScene("World");
	}
}
