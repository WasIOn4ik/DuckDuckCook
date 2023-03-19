using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
	private const string PLAYER_PREFS_MOSIC_VOLUME = "MusicVolume";
	public static MusicManager Instance;

	public float Volume { get; private set; } = 0.3f;

	private AudioSource audioSource;

	private void Awake()
	{
		Instance = this;
		audioSource = GetComponent<AudioSource>();
		Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MOSIC_VOLUME, 0.3f);
		audioSource.volume = Volume;
	}
	public void ChangeVolume()
	{
		Volume += .1f;
		if (Volume > 1f)
		{
			Volume = 0f;
		}
		audioSource.volume = Volume;

		PlayerPrefs.SetFloat(PLAYER_PREFS_MOSIC_VOLUME, Volume);
		PlayerPrefs.Save();
	}
}
