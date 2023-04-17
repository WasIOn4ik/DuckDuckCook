using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
	private const string PLAYER_PREFS_BINDINGS = "Bindings";
	private PlayerInputActions playerInputActions;
	public event EventHandler OnInteractAction;
	public event EventHandler OnInteractAlternateAction;
	public event EventHandler OnPauseAction;
	public event EventHandler OnBindingRebind;

	public static GameInput Instance { get; private set; }

	public enum Binding
	{
		Move_up,
		Move_down,
		Move_right,
		Move_left,
		Interact,
		Interact_Alt,
		Pause
	}

	private void Awake()
	{
		Instance = this;
		playerInputActions = new PlayerInputActions();

		if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
		{
			string loadString = PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS);
			playerInputActions.LoadBindingOverridesFromJson(loadString);
		}

		playerInputActions.Player.Enable();

		playerInputActions.Player.Interact.performed += Interact_performed;
		playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;

		playerInputActions.Player.Pause.performed += Pause_performed;
	}

	private void OnDestroy()
	{

		playerInputActions.Player.Interact.performed -= Interact_performed;
		playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;

		playerInputActions.Player.Pause.performed -= Pause_performed;

		playerInputActions.Dispose();
	}

	private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnPauseAction?.Invoke(this, EventArgs.Empty);
	}

	private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
	}

	private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAction?.Invoke(this, EventArgs.Empty);
	}

	public Vector2 GetMovementVectorNormalized()
	{
		Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

		return inputVector;
	}

	public string GetBindingText(Binding binding)
	{
		switch (binding)
		{
			case Binding.Move_up:
				return playerInputActions.Player.Move.bindings[1].ToDisplayString();
			case Binding.Move_down:
				return playerInputActions.Player.Move.bindings[2].ToDisplayString();
			case Binding.Move_right:
				return playerInputActions.Player.Move.bindings[4].ToDisplayString();
			case Binding.Move_left:
				return playerInputActions.Player.Move.bindings[3].ToDisplayString();
			case Binding.Interact:
				return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
			case Binding.Interact_Alt:
				return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
			case Binding.Pause:
				return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
		}
		return "";
	}

	public void RebindBinding(Binding binding, Action onActionRebound)
	{
		playerInputActions.Player.Disable();

		InputAction action = null;
		int index = 0;

		switch (binding)
		{
			case Binding.Move_up:
				action = playerInputActions.Player.Move;
				index = 1;
				break;
			case Binding.Move_down:
				action = playerInputActions.Player.Move;
				index = 2;
				break;
			case Binding.Move_right:
				action = playerInputActions.Player.Move;
				index = 4;
				break;
			case Binding.Move_left:
				action = playerInputActions.Player.Move;
				index = 3;
				break;
			case Binding.Interact:
				action = playerInputActions.Player.Interact;
				index = 0;
				break;
			case Binding.Interact_Alt:
				action = playerInputActions.Player.InteractAlternate;
				index = 0;
				break;
			case Binding.Pause:
				action = playerInputActions.Player.Pause;
				index = 0;
				break;
		}

		action.PerformInteractiveRebinding(index).OnComplete(callback =>
		{
			callback.Dispose();
			playerInputActions.Player.Enable();
			onActionRebound();

			string saveString = playerInputActions.SaveBindingOverridesAsJson();
			PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, saveString);
			PlayerPrefs.Save();
			OnBindingRebind?.Invoke(this, EventArgs.Empty);
		}).Start();
	}
}
