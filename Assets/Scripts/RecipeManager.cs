using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour {

	public enum Categories { All, Construction, Resources, Tools, Automation, Decoration };

	[SerializeField] Sprite[] categoryIcons;
	[SerializeField] GameObject categoriesContainer;
	[SerializeField] GameObject categoryButtonPrefab;

	public Transform craftingRecipeContainer;
	public RectTransform craftingScrollRectTransform;
	ScrollRect craftingScrollRect;

	public Recipe[] recipes;

	public GameObject craftingRecipePrefab;

	void Awake() {
		craftingScrollRect = craftingScrollRectTransform.GetComponent<ScrollRect>();
	}

	void Start() {
		InitializeCategories();
		InitializeRecipes(Categories.All);
	}

	public void InitializeRecipes(Categories c) {
		ClearRecipes();
		int i = 0;
		foreach(Recipe recipe in recipes) {
			if(recipe.categories.Contains(c) || c == 0) {
				GameObject recipeObj = Instantiate(craftingRecipePrefab, craftingRecipeContainer) as GameObject;
				RectTransform rectTransform = recipeObj.GetComponent<RectTransform>();
				Vector3 tempSize = rectTransform.sizeDelta;
				tempSize.y = Mathf.Ceil(recipe.inputs.Length / 3f) * 100f;
				rectTransform.sizeDelta = tempSize;
				recipeObj.GetComponent<RecipeHandler>().recipe = recipes[i];
				craftingScrollRect.verticalScrollbar.value = 1f;
			}
			i++;
		}
	}

	void ClearRecipes() {
		foreach(Transform recipeTrans in craftingRecipeContainer) {
			if(recipeTrans.GetComponent<RecipeHandler>()) {
				Destroy(recipeTrans.gameObject);
			}
		}
	}

	void InitializeCategories() {
		int numCategories = System.Enum.GetNames(typeof(Categories)).Length;
		string[] categoryNames = System.Enum.GetNames(typeof(Categories));
		for(int i = 0; i <= numCategories - 1; i++) {
			int recipeToInitialize = i;
			GameObject obj = Instantiate(categoryButtonPrefab, categoriesContainer.transform);
			obj.GetComponent<Button>().onClick.AddListener(delegate { InitializeRecipes((Categories)recipeToInitialize); });
			obj.GetComponentInChildren<Text>().text = categoryNames[i];
			obj.GetComponentsInChildren<Image>()[1].sprite = categoryIcons[i];
		}
	}
}
