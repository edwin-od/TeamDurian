using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;

public class UI_ScoreManager : MonoBehaviour
{
    public GameObject scoreTextPrefab;
    public Transform textContainer;
    public float textLifetime = 3;
    public float textFadeOutDuration = 1;
    public float popAnimationDuration = .5f;
    public float heightAnimation = 30;
    public float heightAnimationDuration = 0.5f;
    public float scaleAnimationMultiplicator = 1.5f;
    public float rotateAnimationForce = 15f;
    public float xPos;
    public float yPos;

    public List<TextMeshProUGUI> texts = new List<TextMeshProUGUI> ();

    public static UI_ScoreManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Button]
    public void FireTextAnimation(string textContent)
    {
        if (!JuiceManager.Instance.ScoreAdd) { return; }

        textContent = "+" + textContent;
        if (texts.Count > 0)
        {
            foreach (var t in texts)
            {
                t.transform.DOLocalMoveY(t .transform.localPosition.y - heightAnimation, heightAnimationDuration).SetEase(Ease.OutCubic);
            }
        }

        TextMeshProUGUI text = Instantiate(scoreTextPrefab, textContainer).GetComponent<TextMeshProUGUI>();
        text.transform.DOLocalMoveX(xPos, 0);
        text.transform.DOLocalMoveY(yPos, 0);
        texts.Add(text);
        text.text = textContent;
        Vector3 originalScale = text.transform.localScale;
        text.transform.DOScale(text.transform.localScale * scaleAnimationMultiplicator, popAnimationDuration / 2).OnComplete(() => text.transform.DOScale(originalScale, popAnimationDuration / 2));

        text.transform.DOLocalRotate(Vector3.one * rotateAnimationForce, popAnimationDuration / 4)
            .OnComplete(() => text.transform.DOLocalRotate(Vector3.zero, popAnimationDuration / 4))
            .OnComplete(() => text.transform.DOLocalRotate(Vector3.one * -rotateAnimationForce, popAnimationDuration / 4))
            .OnComplete(() => text.transform.DOLocalRotate(Vector3.zero, popAnimationDuration / 4));

        StartCoroutine(DestroyText(text));
    }

    public IEnumerator DestroyText(TextMeshProUGUI text)
    {
        yield return new WaitForSeconds(textLifetime);

        text.DOFade(0, textFadeOutDuration).OnComplete(delegate {
            texts.Remove(text);
            Destroy(text.gameObject);
        });
    }

    
}
