using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DDC/RecipesList")]
public class RecipesListSO : ScriptableObject
{
	public List<RecipeSO> recipeSOList;
}
