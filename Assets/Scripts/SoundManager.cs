using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffects";
	public float Volume { get; private set; } = 1f;

	public static SoundManager Instance { get; private set; }
	[SerializeField] private AudioClipsSO audioClipsSO;

	private void Awake()
	{
		Instance = this;

		Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
	}

	private void Start()
	{
		DeliveryManager.Instance.OnRecipeFailure += DeliveryManager_OnRecipeFailure;
		DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
		CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
		Player.Instance.OnPickedSomething += Player_OnPickedSomething;
		BaseCounter.OnAnyObjectPlaced += AnyCounter_OnAnyObjectPlaced;
		TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
	}

	private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
	{
		TrashCounter trashCounter = sender as TrashCounter;
		PlaySound(audioClipsSO.trash, trashCounter.transform.position);
	}

	private void AnyCounter_OnAnyObjectPlaced(object sender, System.EventArgs e)
	{
		BaseCounter counter = sender as BaseCounter;
		PlaySound(audioClipsSO.objectDrop, counter.transform.position);
	}

	private void Player_OnPickedSomething(object sender, System.EventArgs e)
	{
		PlaySound(audioClipsSO.objectPickup, Player.Instance.transform.position);
	}

	private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
	{
		CuttingCounter cuttingCounter = sender as CuttingCounter;
		PlaySound(audioClipsSO.chop, cuttingCounter.transform.position);
	}

	private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
	{
		PlaySound(audioClipsSO.deliverySuccess, DeliveryCounter.Instance.transform.position);
	}

	private void DeliveryManager_OnRecipeFailure(object sender, System.EventArgs e)
	{
		PlaySound(audioClipsSO.deliveryFailed, DeliveryCounter.Instance.transform.position);
	}
	private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1.0f)
	{
		PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
	}

	private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1.0f)
	{
		AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * Volume);
	}

	public void PlayFootstepsSound(Vector3 position)
	{
		PlaySound(audioClipsSO.footstep, position);
	}

	public void PlayCountdownSound()
	{
		PlaySound(audioClipsSO.warning, Vector3.zero);
	}

	public void PlayWarningSound(Vector3 position)
	{
		PlaySound(audioClipsSO.warning, position);
	}

	public void ChangeVolume()
	{
		Volume += .1f;
		if (Volume > 1f)
		{
			Volume = 0f;
		}
		PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, Volume);
		PlayerPrefs.Save();
	}
}
