using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
	[SerializeField] private BaseCounter clearCounter;
	[SerializeField] private GameObject[] visualGameObjects;

	private void Start()
	{
		if(Player.LocalInstance != null)
		{
			Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		}
		else
		{
			Player.OnLocalPlayerSpawned += Player_onLocalPlayerSpawned;
		}
	}

	private void Player_onLocalPlayerSpawned(object sender, System.EventArgs e)
	{
		if (Player.LocalInstance != null)
		{
			Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
			Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		}
	}

	private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
	{
		if (e.selectedCounter == clearCounter)
			Show();
		else
			Hide();
	}

	private void Show()
	{
		foreach(var visualObject in visualGameObjects)
		{
			visualObject.SetActive(true);
		}
	}

	private void Hide()
	{
		foreach (var visualObject in visualGameObjects)
		{
			visualObject.SetActive(false);
		}
	}
}
