using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject {
	public Item[] inputs;
	public int[] amounts;
	public Item[] replacementItems;
	public int[] replacementItemAmounts;
	public Item output;
	public int outputAmount;
	public RecipeManager.Categories[] categories;
}
