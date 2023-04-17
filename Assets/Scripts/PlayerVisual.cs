using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
	[SerializeField] private MeshRenderer headMeshRenderer;
	[SerializeField] private MeshRenderer bodyMeshRendrer;


	private Material material;

	private void Awake()
	{
		material = new Material(headMeshRenderer.material);
		headMeshRenderer.material = material;
		bodyMeshRendrer.material = material;
	}

	public void SetPlayerColor(Color color)
	{
		material.color = color;
	}
}
