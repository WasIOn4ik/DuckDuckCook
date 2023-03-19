using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
	private const string POPUP = "Popup";
	private Animator animator;
	[SerializeField] private Image backgroundImage;
	[SerializeField] private Image iconImage;
	[SerializeField] private TextMeshProUGUI messageText;
	[SerializeField] private Color successColor;
	[SerializeField] private Color failureColor;
	[SerializeField] private Sprite successSprite;
	[SerializeField] private Sprite failureSprite;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}
	private void Start()
	{
		DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
		DeliveryManager.Instance.OnRecipeFailure += DeliveryManager_OnRecipeFailure;

		Hide();
	}

	private void DeliveryManager_OnRecipeFailure(object sender, System.EventArgs e)
	{
		Show();
		animator.SetTrigger(POPUP);
		backgroundImage.color = failureColor;
		iconImage.sprite = failureSprite;
		messageText.text = "DELIVERY\nFAILED";
	}

	private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
	{
		Show();
		animator.SetTrigger(POPUP);
		backgroundImage.color = successColor;
		iconImage.sprite = successSprite;
		messageText.text = "DELIVERY\nSUCCESS";
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
