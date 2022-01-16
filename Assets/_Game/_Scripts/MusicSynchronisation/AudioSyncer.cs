using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncer : MonoBehaviour
{

	public virtual void OnBeat()
	{
		timer = 0;
		isBeat = true;
	}

	public virtual void OnUpdate()
	{
		// update audio value
		previousAudioValue = audioValue;
		audioValue = AudioSpectrum.spectrumValue;

		if (previousAudioValue > bias &&
			audioValue <= bias)
		{
			if (timer > timeStep)
				OnBeat();
		}

		if (previousAudioValue <= bias &&
			audioValue > bias)
		{
			if (timer > timeStep)
				OnBeat();
		}

		timer += Time.deltaTime;
	}

	private void Update()
	{
		if (!JuiceManager.Instance.IsSyncJuiceOn) return;
		OnUpdate();
	}

	public float bias;
	public float timeStep;
	public float timeToBeat;
	public float restSmoothTime;

	private float previousAudioValue;
	private float audioValue;
	private float timer;

	protected bool isBeat;
}