using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DDC/FryRecipe")]
public class FryingRecipeSO : ScriptableObject
{
	public KitchenObjectSO input;
	public KitchenObjectSO output;

	public BurningRecipeSO burningRecipe;

	public float fryingTimerMax = 3.0f;
}
