using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
	private void Start()
	{
		GameInstanceMultiplayer.Instance.OnTryingTOJoinGame += GameInstanceMultiplayer_OnTryingTOJoinGame;
		GameInstanceMultiplayer.Instance.OnFailedToJoinGame += GameInstanceMultiplayer_OnFailedToJoinGame;
		Hide();
	}

	private void GameInstanceMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
	{
		Hide();
	}

	private void GameInstanceMultiplayer_OnTryingTOJoinGame(object sender, System.EventArgs e)
	{
		Show();  
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
		GameInstanceMultiplayer.Instance.OnTryingTOJoinGame -= GameInstanceMultiplayer_OnTryingTOJoinGame;
		GameInstanceMultiplayer.Instance.OnFailedToJoinGame -= GameInstanceMultiplayer_OnFailedToJoinGame;
	}
}
