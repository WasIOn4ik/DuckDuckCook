using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerRecipeUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI recipeNameText;
	[SerializeField] private Transform iconContainer;
	[SerializeField] private Transform iconTemplate;

	private void Awake()
	{
		iconTemplate.gameObject.SetActive(false);
	}

	public void SetRecipeSO(RecipeSO recipeSO)
	{
		recipeNameText.text = recipeSO.recipeName;

		foreach (Transform child in iconContainer.transform)
		{
			if (child == iconTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (KitchenObjectSO kitchenObjectSO in recipeSO.ingredients)
		{
			var icon = Instantiate(iconTemplate, iconContainer);
			icon.gameObject.SetActive(true);
			icon.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
		}
	}
}
