using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
	[SerializeField] private PlateKitchenObject plateKitchenObject;
	[SerializeField] private List<KitchenObjectSO_GameObject> visuals;
	[Serializable]
	public struct KitchenObjectSO_GameObject
	{
		public KitchenObjectSO kitchenObjectSO;
		public GameObject gameObject;
	}

	private void Start()
	{
		plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
	}

	private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
	{
		foreach (var kitchenObjectVisual in visuals)
		{
			if (kitchenObjectVisual.kitchenObjectSO == e.kitchenObjectSO)
			{
				kitchenObjectVisual.gameObject.SetActive(true);
				return;
			}
		}
	}
}
