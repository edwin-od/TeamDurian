using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DistortionController : MonoBehaviour
{
    public float endScale = 22;
    public float startScale = 0;
    public float yScale = 15;

    public float fireDistortionDuration = 1;
    public float closeDistortionDuration = 1;

    private MaterialPropertyBlock propertyBlock;
    public float startingDistortion = 3;
    public float endDistortion = 0;

    public static DistortionController instance;

    private bool isOpen;

    private void Awake()
    {
        instance = this;
    }


    [Sirenix.OdinInspector.Button]
    public void FireOnce(float fireDuration = 0, float closeDuration = 0)
    {
        transform.localScale = Vector3.one * startScale;
        transform.DOScaleX(endScale, fireDuration == 0 ? fireDistortionDuration : fireDuration).SetEase(Ease.InCubic)/*.OnUpdate(() => SetDistortion())*/;
        transform.DOScaleZ(endScale, fireDuration == 0 ? fireDistortionDuration : fireDuration).SetEase(Ease.InCubic)/*.OnComplete(() => gameObject.SetActive(false))*/.OnComplete(() => SetDistortion(0));
        transform.DOScaleY(yScale, 0);
        isOpen = true;
        SetDistortion(-0.3f);
    }

    [Sirenix.OdinInspector.Button]
    public void CloseOnce(float duration = 0)
    {
        if (!isOpen)
            return;
        isOpen = false;

        Debug.Log("Close once");
        transform.localScale = Vector3.one * endScale;
        transform.DOScaleX(startScale, duration == 0 ? closeDistortionDuration : duration).SetEase(Ease.InCubic)/*.OnUpdate(() => SetDistortion())*/;
        transform.DOScaleZ(startScale, duration == 0 ? closeDistortionDuration : duration).SetEase(Ease.InCubic)/*.OnComplete(() => gameObject.SetActive(false))*/;
        transform.DOScaleY(yScale, 0);
    }

    void SetDistortion(float distortion)
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        Renderer renderer = GetComponent<Renderer>();
        //set the color property
        propertyBlock.SetFloat("_Distortion", distortion);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
    }
}
