using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class DeathNoteController : MonoBehaviour
{
    //public Vector3 startScale;
    //public Vector3 endScale;

    public Vector3 startScale;
    public float endScale;
    public float duration;

    private MaterialPropertyBlock propertyBlock;
    public string alphaPropertyName = "_Alpha";
    public static DeathNoteController instance;

    public ParticleSystem[] ps;

    private Renderer rend;

    private void Awake()
    {
        instance = this;

        transform.localScale = startScale;

        rend = GetComponent<Renderer>();
    }

    [Button]
    public void FireOnce()
    {
        transform.localScale = startScale;
        transform.DOScaleX(endScale, duration);

        StartCoroutine(UpdateAlpha(0));

        foreach(var p in ps)
        {
            p.Play();
        }
    }

    void SetValue(float value)
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        propertyBlock.SetFloat(alphaPropertyName, value);
        rend.SetPropertyBlock(propertyBlock);
    }

    public IEnumerator UpdateAlpha(float endValue)
    {
        float timeElapsed = 0;
        float startValue = 1;


        while (timeElapsed < duration)
        {
            SetValue(Mathf.Lerp(startValue, endValue, timeElapsed / duration));
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        SetValue(endValue);
    }
}
