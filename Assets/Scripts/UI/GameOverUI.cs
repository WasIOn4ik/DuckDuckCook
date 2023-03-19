using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI recipesDeliveredText;

	private void Start()
	{
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
