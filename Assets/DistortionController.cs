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


    [Sirenix.OdinInspector.Button]
    public void FireOnce()
    {
        transform.localScale = Vector3.one * startScale;
        transform.DOScaleX(endScale, fireDistortionDuration).SetEase(Ease.InCubic)/*.OnUpdate(() => SetDistortion())*/;
        transform.DOScaleZ(endScale, fireDistortionDuration).SetEase(Ease.InCubic)/*.OnComplete(() => gameObject.SetActive(false))*/.OnComplete(() => CloseOnce());
        transform.DOScaleY(yScale, 0);
    }

    [Sirenix.OdinInspector.Button]
    public void CloseOnce()
    {
        transform.localScale = Vector3.one * endScale;
        transform.DOScaleX(startScale, closeDistortionDuration).SetEase(Ease.InCubic)/*.OnUpdate(() => SetDistortion())*/;
        transform.DOScaleZ(startScale, closeDistortionDuration).SetEase(Ease.InCubic)/*.OnComplete(() => gameObject.SetActive(false))*/;
        transform.DOScaleY(yScale, 0);
    }

    void SetDistortion()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        Renderer renderer = GetComponent<Renderer>();
        //set the color property
        propertyBlock.SetFloat("_Distortion", endDistortion);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
    }
}
