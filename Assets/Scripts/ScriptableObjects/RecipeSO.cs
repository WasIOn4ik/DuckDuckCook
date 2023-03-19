using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DDC/Recipe")]
public class RecipeSO : ScriptableObject
{
	public string recipeName;
	public List<KitchenObjectSO> ingredients;
}
