using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Bind = GameInput.Binding;

public class TutorialUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI moveUpText;
	[SerializeField] private TextMeshProUGUI moveDownText;
	[SerializeField] private TextMeshProUGUI moveLeftText;
	[SerializeField] private TextMeshProUGUI moveRightText;
	[SerializeField] private TextMeshProUGUI interactText;
	[SerializeField] private TextMeshProUGUI interactAltText;
	[SerializeField] private TextMeshProUGUI pauseText;

	private void Start()
	{
		GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
		GameInstance.Instance.OnStateChanged += GameInstance_OnStateChanged;
		UpdateBindingsVisual();
		Show();
	}

	private void GameInstance_OnStateChanged(object sender, System.EventArgs e)
	{
		if(GameInstance.Instance.IsCountdownToStartActive())
		{
			Hide();
		}
	}

	private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
	{
		UpdateBindingsVisual();
	}

	private void UpdateBindingsVisual()
	{
		var gi = GameInput.Instance;
		moveUpText.text = gi.GetBindingText(Bind.Move_up);
		moveDownText.text = gi.GetBindingText(Bind.Move_down);
		moveLeftText.text = gi.GetBindingText(Bind.Move_left);
		moveRightText.text = gi.GetBindingText(Bind.Move_right);
		interactText.text = gi.GetBindingText(Bind.Interact);
		interactAltText.text = gi.GetBindingText(Bind.Interact_Alt);
		pauseText.text = gi.GetBindingText(Bind.Pause);
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
