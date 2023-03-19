using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DDC/CutRecipe")]
public class CuttingRecipesSO : ScriptableObject
{
	public KitchenObjectSO input;
	public KitchenObjectSO output;

	public int cuttingProgressMax = 3;
}
