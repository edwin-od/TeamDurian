using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Image))]
public class AudioSyncColor : AudioSyncer
{

	private IEnumerator MoveToColor(Color _target)
	{
		Color _curr = img.color;
		Color _initial = _curr;
		float _timer = 0;

		while (_curr != _target)
		{
			_curr = Color.Lerp(_initial, _target, _timer / timeToBeat);
			_timer += Time.deltaTime;

			img.color = _curr;

			yield return null;
		}

		isBeat = false;
	}

	private Color RandomColor()
	{
		if (beatColors == null || beatColors.Length == 0) return Color.white;
		randomIndx = Random.Range(0, beatColors.Length);
		return beatColors[randomIndx];
	}

	public override void OnUpdate()
	{
		base.OnUpdate();

		if (isBeat) return;

		img.color = Color.Lerp(img.color, restColor, restSmoothTime * Time.deltaTime);
	}

	public override void OnBeat()
	{
		base.OnBeat();

		Color _c = RandomColor();

		StopCoroutine("MoveToColor");
		StartCoroutine("MoveToColor", _c);
	}

	private void Start()
	{
		img = GetComponent<SpriteRenderer>();
	}

	public Color[] beatColors;
	public Color restColor;

	private int randomIndx;
	private SpriteRenderer img;
}