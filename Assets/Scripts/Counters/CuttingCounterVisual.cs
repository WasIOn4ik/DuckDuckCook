using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
	private const string CUT = "Cut";
	[SerializeField] private CuttingCounter cuttingCounter;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		cuttingCounter.OnCut += CuttingCounter_OnProgressChanged;
	}

	private void CuttingCounter_OnProgressChanged(object sender, EventArgs e)
	{
		animator.SetTrigger(CUT);
	}
}
