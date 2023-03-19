using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
	[SerializeField] private Transform snapPoint;
	[SerializeField] private Transform plateVisualPrefab;
	[SerializeField] private PlatesCounter platesCounter;

	private Vector3 plateOffset = new Vector3(0f, 0.1f, 0f);

	private Stack<GameObject> plates = new();

	private void Start()
	{
		platesCounter.OnCountChanged += PlatesCounter_OnCountChanged;
	}

	private void PlatesCounter_OnCountChanged(object sender, PlatesCounter.OnCountChangedEventArgs e)
	{
		if (e.count > plates.Count)
		{
			Transform targetTransform;
			Transform newPlate;
			if (plates.Count == 0)
			{
				targetTransform = snapPoint;
				newPlate = Instantiate(plateVisualPrefab, targetTransform.position, Quaternion.identity, targetTransform);
			}
			else
			{
				targetTransform = plates.Peek().transform;
				newPlate = Instantiate(plateVisualPrefab, targetTransform.position + plateOffset, Quaternion.identity, targetTransform);
			}
			plates.Push(newPlate.gameObject);
		}
		else
		{
			var targetTransform = plates.Pop();
			Destroy(targetTransform);
		}
	}
}
