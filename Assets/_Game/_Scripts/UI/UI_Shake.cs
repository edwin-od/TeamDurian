using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Shake : MonoBehaviour
{
	public static IEnumerator Shake(Transform target, float shakeDuration = 0.3f, float shakeAmount = 0.7f)
    {
		float time = 0;
		Vector3 originalPos = target.localPosition;

		while (time < shakeDuration)
		{
			target.position = originalPos + Random.insideUnitSphere * shakeAmount;
			time += Time.deltaTime;
			yield return null;
		}

		target.localPosition = originalPos;
	}

}
