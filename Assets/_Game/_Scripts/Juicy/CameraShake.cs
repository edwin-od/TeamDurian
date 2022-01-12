using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraShake : MonoBehaviour
{
    public Transform camera;

    public ShakePreset[] presets;
    public int actualPreset = -1;
    float _FadeOut = 1f;
    private bool _fading;
    private Vector3 _originalCamPos;
    public enum ShakeForce { Little, Medium, Strong };

    void Reset()
    {
        camera = transform;
    }
    void OnValidate()
    {
        presets[0].direction.Normalize();
        enabled = false;
    }

    void Start()
    {
        _originalCamPos = camera.localPosition;
    }


    void Update()
    {
        var sin = Mathf.Sin(presets[actualPreset].speed * (presets[actualPreset].seed.x + presets[actualPreset].seed.y + Time.time));
        var direction = presets[actualPreset].direction + GetNoise();
        direction.Normalize();
        var delta = direction * sin;
        Vector3 localPos = delta * presets[actualPreset].maxMagnitude * _FadeOut;
        camera.localPosition = localPos + _originalCamPos;
    }


    [Button]
    public void FireOnce(ShakeForce force)
    {
        if (_fading)
        {
            if (actualPreset <= UpdatePreset(force))
                StartShake(force);
        }
        else
            StartShake(force);
    }

    [Button]
    public void FireOnceWithIndex(int index)
    {
        index++;
        if (!_fading)
        {
            actualPreset = index;
            StopAllCoroutines();
            _fading = true;
            StartCoroutine(ShakeAndFade(0.5f));
        }
    }

    void StartShake(ShakeForce force)
    {
        actualPreset = UpdatePreset(force);
        StopAllCoroutines();
        _fading = true;
        StartCoroutine(ShakeAndFade(0.5f));
    }

    int UpdatePreset(ShakeForce force)
    {
        switch (force)
        {
            case ShakeForce.Little:
                return 0;
            case ShakeForce.Medium:
                return 1;
            case ShakeForce.Strong:
                return 2;
        }

        return -1;
    }

    public void ContinuousShake()
    {
        enabled = true;
        _FadeOut = 1f;
    }

    public IEnumerator ShakeAndFade(float fade_duration)
    {
        enabled = true;
        _FadeOut = 1f;
        var fade_out_start = Time.time;
        var fade_out_complete = fade_out_start + fade_duration;
        while (Time.time < fade_out_complete)
        {
            yield return null;
            var t = 1f - Mathf.InverseLerp(fade_out_start, fade_out_complete, Time.time);
            _FadeOut = t;
        }
        _fading = false;
        enabled = false;
        actualPreset = -1;
    }

    Vector2 GetNoise()
    {
        var time = Time.time;
        return (presets[actualPreset].noiseMagnitude
            * new Vector2(
                Mathf.PerlinNoise(presets[actualPreset].seed.x, time) - 0.5f,
                Mathf.PerlinNoise(presets[actualPreset].seed.y, time) - 0.5f
                ));
    }
}
