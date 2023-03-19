using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DDC/AudioClips")]
public class AudioClipsSO : ScriptableObject
{
	public AudioClip[] chop;
	public AudioClip[] deliveryFailed;
	public AudioClip[] deliverySuccess;
	public AudioClip[] footstep;
	public AudioClip[] objectDrop;
	public AudioClip[] objectPickup;
	public AudioClip stoveSizzle;
	public AudioClip[] trash;
	public AudioClip[] warning;

}
