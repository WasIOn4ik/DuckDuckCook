using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI recipesDeliveredText;
	[SerializeField] private Button playAgainButton;

	private void Start()
	{
		playAgainButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.SceneName.MainMenuScene);
		});
		GameInstance.Instance.OnStateChanged += GameInstance_OnStateChanged;

		Hide();
	}

	private void GameInstance_OnStateChanged(object sender, System.EventArgs e)
	{
		if (GameInstance.Instance.IsGameOver())
		{
			Show();
			recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessRecipesAmount().ToString();
		}
		else
		{
			Hide();
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
