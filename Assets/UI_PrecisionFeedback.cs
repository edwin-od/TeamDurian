using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_PrecisionFeedback : MonoBehaviour
{
    public static UI_PrecisionFeedback Instance;
    public GameObject feedbackTextPrefab;
    public Transform feedbackContainer;
    public float scaleAnimationMultiplicator = 2f;
    public float popAnimationDuration = 0.5f;
    public float rotateAnimForce = 10;
    public float popupLifetime;
    public float imageFadeOutDuration = 0.5f;

    public float xPos;
    public float yPos;

    private Image lastImage;

    private void Awake()
    {
        Instance = this;
    }

    void FireOnceFeedback()
    {
        if(lastImage != null)
            Destroy(lastImage.gameObject);

        Image image = Instantiate(feedbackTextPrefab, feedbackContainer).GetComponent<Image>();
        image.transform.DOLocalMoveX(xPos, 0);
        image.transform.DOLocalMoveY(yPos, 0);
        Vector3 originalScale = image.transform.localScale;
        image.transform.DOScale(image.transform.localScale * scaleAnimationMultiplicator, popAnimationDuration / 2).OnComplete(() => image.transform.DOScale(originalScale, popAnimationDuration / 2));

        image.transform.DOLocalRotate(Vector3.one * rotateAnimForce, popAnimationDuration / 4)
            .OnComplete(() => image.transform.DOLocalRotate(Vector3.zero, popAnimationDuration / 4))
            .OnComplete(() => image.transform.DOLocalRotate(Vector3.one * -rotateAnimForce, popAnimationDuration / 4))
            .OnComplete(() => image.transform.DOLocalRotate(Vector3.zero, popAnimationDuration / 4));

        lastImage = image;

        StartCoroutine(DestroyText(image));
    }

    public IEnumerator DestroyText(Image image)
    {
        yield return new WaitForSeconds(popupLifetime);

        image.DOFade(0, imageFadeOutDuration).OnComplete(delegate {
            Destroy(image.gameObject);
        });
    }
}
