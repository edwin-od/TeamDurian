using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    public RectTransform TopProgressBar;
    public RectTransform BottomProgressBar;

    public Color OutOfBeatColor;
    public Color OnBeatColor;

    private bool overshoot = false;

    private void Beat()
    {
        overshoot = true;
        TopProgressBar.gameObject.GetComponent<Image>().color = OutOfBeatColor;

        Debug.Log("BEAT ");
    }

    private void BeatIntervalStart()
    {
        TopProgressBar.gameObject.GetComponent<Image>().color = OnBeatColor;

        Debug.Log("START");
    }

    private void BeatIntervalEnd()
    {
        overshoot = false;
        BottomProgressBar.localScale = new Vector3(1, 0, 1);

        Debug.Log("END");
    }

    private void OnEnable()
    {
        Tempo.OnBeat += Beat;
        Tempo.OnIntervalBeatStart += BeatIntervalStart;
        Tempo.OnIntervalBeatEnd += BeatIntervalEnd;
    }

    private void OnDisable()
    {
        Tempo.OnBeat -= Beat;
        Tempo.OnIntervalBeatStart -= BeatIntervalStart;
        Tempo.OnIntervalBeatEnd -= BeatIntervalEnd;
    }


    void Start()
    {
        if (TopProgressBar && BottomProgressBar)
        {
            TopProgressBar.localScale = Vector3.one;
            BottomProgressBar.localScale = new Vector3(1, 0, 1);

            TopProgressBar.gameObject.GetComponent<Image>().color = OutOfBeatColor;
            BottomProgressBar.gameObject.GetComponent<Image>().color = OnBeatColor;
        }
    }

    void Update()
    {
        if(TopProgressBar && BottomProgressBar && Tempo.Get)
        {
            if(overshoot)
                BottomProgressBar.localScale = new Vector3(1, Mathf.Clamp(1 - Tempo.Get.PercentageToBeat, 0, 1), 1);

            TopProgressBar.localScale = new Vector3(1, Mathf.Clamp(Tempo.Get.PercentageToBeat, 0 ,1), 1);
        }
    }
}
