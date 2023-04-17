using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DDC/KitchenObjectsList")]
public class KitchenObjectsListSO : ScriptableObject
{
	public List<KitchenObjectSO> kitchenObjectSOList;
}
