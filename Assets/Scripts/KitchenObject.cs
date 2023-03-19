using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	private IKitchenObjectParent kitchenObjectParent;

	public static KitchenObject SpawnKitchenObject(KitchenObjectSO objectSO, IKitchenObjectParent parent)
	{
		if (parent.HasKitchenObject())
			return parent.KitchenObject;

		KitchenObject kitchenObject = Instantiate(objectSO.prefab).GetComponent<KitchenObject>();
		kitchenObject.KitchenObjectParent = parent;
		return kitchenObject;
	}

	public IKitchenObjectParent KitchenObjectParent
	{
		get
		{
			return kitchenObjectParent;
		}
		set
		{
			if (kitchenObjectParent != null)
			{
				kitchenObjectParent.ClearKitchenObject();
			}

			kitchenObjectParent = value;

			if (kitchenObjectParent.HasKitchenObject())
			{
				Debug.LogError("Counter already has a kitchenObject");
				return;
			}
			kitchenObjectParent.KitchenObject = this;

			transform.parent = kitchenObjectParent.GetSnapPoint();
			transform.localPosition = Vector3.zero;
		}
	}

	public KitchenObjectSO GetKitchenObjectSO()
	{
		return kitchenObjectSO;
	}

	public void DestroySelf()
	{
		KitchenObjectParent?.ClearKitchenObject();
		Destroy(gameObject);
	}

	public bool TryGetPlate(out PlateKitchenObject plate)
	{
		if (this is PlateKitchenObject)
		{
			plate = this as PlateKitchenObject;
			return true;
		}

		plate = null;
		return false;
	}
}
