using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
	[SerializeField] private Transform container;
	[SerializeField] private Transform recipeTemplate;

	private void Awake()
	{
		recipeTemplate.gameObject.SetActive(false);
	}

	private void Start()
	{
		DeliveryManager.Instance.OnWaitingRecipesChanged += Instance_OnWaitingRecipesChanged;
		UpdateVisual();
	}

	private void Instance_OnWaitingRecipesChanged(object sender, System.EventArgs e)
	{
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		foreach (Transform child in container.transform)
		{
			if (child == recipeTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (RecipeSO recipe in DeliveryManager.Instance.GetWaitingRecipes())
		{
			Transform recipeTranform = Instantiate(recipeTemplate, container);
			recipeTranform.gameObject.SetActive(true);
			recipeTranform.GetComponent<DeliveryManagerRecipeUI>().SetRecipeSO(recipe);
		}
	}
}
