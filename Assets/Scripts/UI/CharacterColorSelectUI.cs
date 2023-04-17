using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectUI : MonoBehaviour
{
	[SerializeField] private int colorID;
	[SerializeField] private Image image;
	[SerializeField] private GameObject selectedMarker;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			GameInstanceMultiplayer.Instance.ChangePlayerColor(colorID);
		});
	}

	private void Start()
	{
		GameInstanceMultiplayer.Instance.OnPlayerDataNetworkListChanged += GameInstanceMultiplayer_OnPlayerDataNetworkListChanged;
		image.color = GameInstanceMultiplayer.Instance.GetPlayerColor(colorID);
		UpdateIsSelected();
	}

	private void GameInstanceMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
	{
		UpdateIsSelected();
	}

	private void UpdateIsSelected()
	{
		if (GameInstanceMultiplayer.Instance.GetPlayerData().colorId == colorID)
		{
			selectedMarker.SetActive(true);
		}
		else
		{
			selectedMarker.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		GameInstanceMultiplayer.Instance.OnPlayerDataNetworkListChanged -= GameInstanceMultiplayer_OnPlayerDataNetworkListChanged;
	}
}
