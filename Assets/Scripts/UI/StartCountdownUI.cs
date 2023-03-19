using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartCountdownUI : MonoBehaviour
{
	private const string NUMBER_POPUP = "Popup";
	[SerializeField] private TextMeshProUGUI countdownText;
	private Animator animator;

	private int previousCountdownNumber;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		GameInstance.Instance.OnStateChanged += GameInstance_OnStateChanged;

		Hide();
	}

	private void GameInstance_OnStateChanged(object sender, System.EventArgs e)
	{
		if (GameInstance.Instance.IsCountdownToStartActive())
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Update()
	{
		int countdownNumber = Mathf.CeilToInt(GameInstance.Instance.GetCountDownToStartTimer());
		countdownText.text = countdownNumber.ToString();

		if(previousCountdownNumber != countdownNumber)
		{
			previousCountdownNumber = countdownNumber;
			animator.SetTrigger(NUMBER_POPUP);
			SoundManager.Instance.PlayCountdownSound();
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
