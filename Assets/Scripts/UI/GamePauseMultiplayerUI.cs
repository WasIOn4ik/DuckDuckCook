using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseMultiplayerUI : MonoBehaviour
{
	private void Start()
	{
		GameInstance.Instance.OnMultiplayerGamePaused += GameInstance_OnMultiplayerGamePaused;
		GameInstance.Instance.OnMultiplayerGameUnpaused += GameInstance_OnMultiplayerGameUnpaused;
		Hide();
	}

	private void GameInstance_OnMultiplayerGameUnpaused(object sender, System.EventArgs e)
	{
		Hide();
	}

	private void GameInstance_OnMultiplayerGamePaused(object sender, System.EventArgs e)
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
}
