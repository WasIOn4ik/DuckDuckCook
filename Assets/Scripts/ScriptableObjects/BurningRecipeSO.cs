using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DDC/BurnRecipe")]
public class BurningRecipeSO : ScriptableObject
{
	public KitchenObjectSO input;
	public KitchenObjectSO output;

	public float burningTimerMax = 3.0f;
}
