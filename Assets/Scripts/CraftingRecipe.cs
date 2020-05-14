using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipe : MonoBehaviour {

	public GameObject craftingSlotPrefab;
	public GameObject outputSlot;
	public Text outputAmount;

	RecipeHandler recipeHandler;
	Recipe recipe;

	Inventory inventory;

	public Color posColorTint;
	public Color negColorTint;

	ColorBlock originalColors = new ColorBlock();

	Button recipeButton;

	bool initialized;

	void Initialize() {
		inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		recipeButton = GetComponent<Button>();
		recipeHandler = GetComponent<RecipeHandler>();
		originalColors = recipeButton.colors;
		recipe = recipeHandler.recipe;
		initialized = true;
	}

	void Start() {
		if(!initialized) {
			Initialize();
		}
		if(recipe.outputAmount > 1) {
			outputAmount.gameObject.SetActive(true);
		} else {
			outputAmount.gameObject.SetActive(false);
		}

		outputAmount.text = recipe.outputAmount.ToString();

		int i = 0;
		foreach(Item item in recipe.inputs) {
			GameObject craftingSlotObj = Instantiate(craftingSlotPrefab, transform.GetChild(0)) as GameObject;
			CraftingSlot craftingSlot = craftingSlotObj.GetComponent<CraftingSlot>();
			craftingSlot.currentItem = recipe.inputs[i];
			craftingSlot.icon.sprite = item.icon;
			craftingSlot.amountText.text = recipe.amounts[i].ToString();
			i++;
		}

		outputSlot.transform.GetChild(0).GetComponent<Image>().sprite = recipe.output.icon;

		InventoryUpdate();
	}

	public void OnRecipeClick() {
		if(inventory.CheckRecipe(recipe)) {
			inventory.ConstructRecipe(recipe);
		}
	}

	public void InventoryUpdate() {
		if(!initialized) {
			Initialize();
		}
		if(inventory.CheckRecipe(recipe)) {
			ColorBlock colors = originalColors;
			colors.normalColor += posColorTint;
			colors.highlightedColor += posColorTint;
			colors.pressedColor += posColorTint;
			recipeButton.colors = colors;
		} else {
			ColorBlock colors = originalColors;
			colors.normalColor += negColorTint;
			colors.highlightedColor += negColorTint;
			colors.pressedColor += negColorTint;
			recipeButton.colors = colors;
		}
	}

	public void OnItemPointerEnter() {
		inventory.SetHoveredItem(recipe.output);
	}

	public void OnItemPointerExit() {
		inventory.LeaveHoveredItem();
	}
}
