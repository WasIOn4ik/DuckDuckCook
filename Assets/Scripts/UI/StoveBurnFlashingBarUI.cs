using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
	private const string WARNING = "Warning";
	[SerializeField] private StoveCounter stoveCounter;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}
	// Start is called before the first frame update
	void Start()
	{
		stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
		animator.SetBool(WARNING, false);
	}

	private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
	{
		float burnShowProgressAmount = 0.5f;
		bool warning = e.progressNormalized > burnShowProgressAmount && stoveCounter.IsFried();

		animator.SetBool(WARNING, warning);
	}
}
