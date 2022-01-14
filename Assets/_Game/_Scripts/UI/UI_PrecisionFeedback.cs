using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class UI_PrecisionFeedback : MonoBehaviour
{
    public static UI_PrecisionFeedback Instance;
    public Sprite normalTextPrefab;
    public Color normalColor = Color.white;
    public float normalEmission = 2.5f;

    public Sprite greatTextPrefab;
    public Color greatColor = Color.white;
    public float greatEmission = 2.5f;

    public Sprite perfectTextPrefab;
    public Color perfectColor = Color.white;
    public float perfectEmission = 2.5f;
    /*
    public Sprite madnessTextPrefab;
    public Color madnessColor = Color.white;
    public float madnessEmission = 2.5f;
    */
    public Sprite missedTextPrefab;
    public Color missedColor = Color.white;
    public float missedEmission = 2.5f;

    public enum Precision { Missed, Normal, Great, Perfect, Madness };

    public GameObject feedbackContainer;
    public GameObject feedbackPrefab;

    public float scaleAnimationMultiplicator = 2f;
    public float popAnimationDuration = 0.5f;
    //public float rotateAnimForce = 10;
    public float popupLifetime = 0.75f;
    public float imageFadeOutDuration = 0.5f;

    public float xPos;
    public float yPos;

    private SpriteRenderer instance;
    private Coroutine coroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void PopPrecision(Precision p)
    {
        if(JuiceManager.Instance.PrecisionFeedback) { return; }

        if (instance) { Destroy(instance.gameObject); if (coroutine != null) { StopCoroutine(coroutine); } }

        GameObject _instance = Instantiate(feedbackPrefab, feedbackContainer.transform);
        instance = _instance.GetComponent<SpriteRenderer>();
        instance.material = new Material(instance.material);
        instance.transform.parent = null;

        float emission = 2.5f;
        Color color = Color.white;

        switch(p)
        {
            case Precision.Missed:
                emission = missedEmission;
                color = missedColor;
                instance.sprite = missedTextPrefab;
                break;
            case Precision.Normal:
                emission = normalEmission;
                color = normalColor;
                instance.sprite = normalTextPrefab;
                break;
            case Precision.Great:
                emission = greatEmission;
                color = greatColor;
                instance.sprite = greatTextPrefab;
                break;
            case Precision.Perfect:
                emission = perfectEmission;
                color = perfectColor;
                instance.sprite = perfectTextPrefab;
                break;
            /*case Precision.Madness:
                emission = madnessEmission;
                color = madnessColor;
                instance.sprite = madnessTextPrefab;
                break;*/
            default:
                instance.sprite = null;
                break;
        }
        
        float a = 1f;
        MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
        instance.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", new Color(color.r * emission, color.b * emission, color.g * emission, a));
        _propBlock.SetColor("_EmissionColor", new Color(color.r * emission, color.b * emission, color.g * emission));
        instance.SetPropertyBlock(_propBlock);

        /*instance.transform.DOLocalMoveX(xPos, 0);
        instance.transform.DOLocalMoveY(yPos, 0);
        Vector3 originalScale = instance.transform.localScale;
        instance.transform.DOScale(instance.transform.localScale * scaleAnimationMultiplicator, popAnimationDuration / 2).OnComplete(() => instance.transform.DOScale(originalScale, popAnimationDuration / 2));
        */
        Vector3 originalScale = instance.transform.localScale;
        instance.transform.DOScale(instance.transform.localScale * scaleAnimationMultiplicator, popAnimationDuration / 2).OnComplete(() => instance.transform.DOScale(originalScale, popAnimationDuration / 2));

        float rotateAnimForce = Random.Range(10, 360);
        /*
        instance.transform.DOLocalRotate(Vector3.one * rotateAnimForce, popAnimationDuration / 4)
            .OnComplete(() => instance.transform.DOLocalRotate(Vector3.zero, popAnimationDuration / 4))
            .OnComplete(() => instance.transform.DOLocalRotate(Vector3.one * -rotateAnimForce, popAnimationDuration / 4))
            .OnComplete(() => instance.transform.DOLocalRotate(Vector3.zero, popAnimationDuration / 4));
        */
        coroutine = StartCoroutine(DestroyText());
    }

    public IEnumerator DestroyText()
    {
        
        yield return new WaitForSeconds(popupLifetime);
        if (instance)
        {
            float timeElapsed = 0;

            MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
            instance.GetPropertyBlock(_propBlock);

            while (timeElapsed < imageFadeOutDuration)
            {
                if (!instance) { yield break; }
                float t = timeElapsed / imageFadeOutDuration;


                float emission = Mathf.Lerp(1.7f, 0f, t);
                float a = Mathf.Lerp(1f, 0f, t);
                _propBlock.SetColor("_Color", new Color(0.75f * 1.6f, 0.17f * 1.6f, 0, a));
                _propBlock.SetColor("_EmissionColor", new Color(0.75f * emission, 0, 0));
                instance.SetPropertyBlock(_propBlock);

                timeElapsed += Time.deltaTime;

                yield return null;
            }
            if (instance) { Destroy(instance.gameObject); }
        }
    }
}
