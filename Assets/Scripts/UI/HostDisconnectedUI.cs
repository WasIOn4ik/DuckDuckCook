using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectedUI : MonoBehaviour
{
	[SerializeField] private Button playAgainButton;

	private void Start()
	{
		playAgainButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.SceneName.MainMenuScene);
		});
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

		Hide();
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
	{
		if (clientID == NetworkManager.ServerClientId)
		{
			Show();
		}
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		if (NetworkManager.Singleton)
			NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
	}
}
