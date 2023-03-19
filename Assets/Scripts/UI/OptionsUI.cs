using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Bind = GameInput.Binding;

public class OptionsUI : MonoBehaviour
{
	public static OptionsUI Instance { get; private set; }

	[SerializeField] private Button soundEffectButton;
	[SerializeField] private Button musicEffectButton;
	[SerializeField] private Button closeButton;
	[SerializeField] private TextMeshProUGUI soundEffectsText;
	[SerializeField] private TextMeshProUGUI musicText;

	[SerializeField] private TextMeshProUGUI moveUpText;
	[SerializeField] private TextMeshProUGUI moveDownText;
	[SerializeField] private TextMeshProUGUI moveRightText;
	[SerializeField] private TextMeshProUGUI moveLeftText;
	[SerializeField] private TextMeshProUGUI interactText;
	[SerializeField] private TextMeshProUGUI interactAltText;
	[SerializeField] private TextMeshProUGUI pauseText;

	[SerializeField] private Button moveUpButton;
	[SerializeField] private Button moveDownButton;
	[SerializeField] private Button moveRightButton;
	[SerializeField] private Button moveLeftButton;
	[SerializeField] private Button InteractButton;
	[SerializeField] private Button InteractAltButton;
	[SerializeField] private Button pauseButton;

	[SerializeField] private Transform pressKeyToRebindTransform;

	private void Awake()
	{
		Instance = this;
		soundEffectButton.onClick.AddListener(() =>
		{
			SoundManager.Instance.ChangeVolume();
			UpdateVisual();
		});
		musicEffectButton.onClick.AddListener(() =>
		{
			MusicManager.Instance.ChangeVolume();
			UpdateVisual();
		});
		closeButton.onClick.AddListener(() => { Hide(); });

		moveUpButton.onClick.AddListener(() => { HandleRebinding(Bind.Move_up); });
		moveDownButton.onClick.AddListener(() => { HandleRebinding(Bind.Move_down); });
		moveLeftButton.onClick.AddListener(() => { HandleRebinding(Bind.Move_left); });
		moveRightButton.onClick.AddListener(() => { HandleRebinding(Bind.Move_right); });
		InteractButton.onClick.AddListener(() => { HandleRebinding(Bind.Interact); });
		InteractAltButton.onClick.AddListener(() => { HandleRebinding(Bind.Interact_Alt); });
		pauseButton.onClick.AddListener(() => { HandleRebinding(Bind.Pause); });
	}

	private void Start()
	{
		GameInstance.Instance.OnGameUnpaused += GameInstance_OnGameUnpaused;
		UpdateVisual();
		UpdateBindingsVisual();

		Hide();
	}

	private void GameInstance_OnGameUnpaused(object sender, System.EventArgs e)
	{
		Hide();
	}

	private void UpdateVisual()
	{
		soundEffectsText.text = $"Sound effects: {Mathf.Round(SoundManager.Instance.Volume * 10f)}";
		musicText.text = $"Music: {Mathf.Round(MusicManager.Instance.Volume * 10f)}";
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
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

	private void ShowPressToRebindKey()
	{
		pressKeyToRebindTransform.gameObject.SetActive(true);
	}
	private void HIdePressToRebindKey()
	{
		pressKeyToRebindTransform.gameObject.SetActive(false);
	}

	private void HandleRebinding(Bind binding)
	{
		ShowPressToRebindKey();
		GameInput.Instance.RebindBinding(binding, () =>
		{
			UpdateBindingsVisual();
			HIdePressToRebindKey();
		});
	}
}
