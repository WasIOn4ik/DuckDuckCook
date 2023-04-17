using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour
{
	private void Start()
	{
		GameInstance.Instance.OnLocalPlayerReadyChanged += GameInstance_OnLocalPlayerReadyChanged;
		GameInstance.Instance.OnStateChanged += GameInstance_OnStateChanged;
		Hide();
	}

	private void GameInstance_OnStateChanged(object sender, System.EventArgs e)
	{
		if(GameInstance.Instance.IsCountdownToStartActive())
		{
			Hide();
		}
	}

	private void GameInstance_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
	{
		if(GameInstance.Instance.IsLocalPlayerReady())
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
}
