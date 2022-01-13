using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BeatFeedback : MonoBehaviour
{
    [SerializeField] private AnimationCurve bounce = AnimationCurve.Constant(0f, 1f, 1f);
    [SerializeField] private AnimationCurve fade = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private RectTransform normalIntervalArea;
    [SerializeField] private RectTransform greatIntervalArea;
    [SerializeField] private RectTransform perfectIntervalArea;
    [SerializeField] private GameObject beatFeedback;
    [SerializeField, Range(0.1f, 0.9f)] private float periodAreaPercentage60BPM = 0.25f;
    [SerializeField, Range(0.01f, 0.99f)] private float beatFeedbackWidth = 0.01f;

    private float currentPeriodAreaPercentage = 0f;
    private List<RectTransform> beatsRight = new List<RectTransform>();
    private List<RectTransform> beatsLeft = new List<RectTransform>();

    private int periodsDisplayable = 0;

    private static UI_BeatFeedback _instance;
    public static UI_BeatFeedback Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }


    private void OnEnable()
    {
        Tempo.OnBeat += Beat;
        Tempo.OnIntervalBeatStart += BeatIntervalStart;
        Tempo.OnIntervalBeatEnd += BeatIntervalEnd;
        Tempo.OnBPMChange += BPMChange;
    }

    private void OnDisable()
    {
        Tempo.OnBeat -= Beat;
        Tempo.OnIntervalBeatStart -= BeatIntervalStart;
        Tempo.OnIntervalBeatEnd -= BeatIntervalEnd;
        Tempo.OnBPMChange -= BPMChange;
    }

    private void Update()
    {
        if(Tempo.Instance && beatFeedback && normalIntervalArea)
        {
            if (Tempo.Instance.isActiveAndEnabled && Tempo.Instance.CanGenerateBeatEvents)
            {
                if (periodsDisplayable == 0) { BPMChange(); }
                
                for(int i = 0; i < beatsRight.Count; i++)
                {
                    float position = currentPeriodAreaPercentage * (Tempo.Instance.PercentageToBeat + i);

                    float bounceAmount = Mathf.Abs(Mathf.Sin(bounce.Evaluate(Tempo.Instance.PercentageToBeat))) / 2f;

                    float distance = fade.Evaluate(1 - (position / 2));
                    float alpha = Mathf.Clamp(distance, 0.1f, 1f);
                    float emission = Mathf.Clamp(distance * 16, 0, 16);

                    beatsRight[i].anchorMin = new Vector2(position - (beatFeedbackWidth / 2) + 0.5f, bounceAmount);
                    beatsRight[i].anchorMax = new Vector2(position + (beatFeedbackWidth / 2) + 0.5f, 1f - bounceAmount);
                    beatsRight[i].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beatsRight[i].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)

                    beatsRight[i].gameObject.GetComponent<Image>().material.color = new Color(0.75f * emission, 0.12f * emission, 0.03f * emission, alpha);

                    beatsLeft[i].anchorMin = new Vector2(-position - (beatFeedbackWidth / 2) + 0.5f, bounceAmount);
                    beatsLeft[i].anchorMax = new Vector2(-position + (beatFeedbackWidth / 2) + 0.5f, 1f - bounceAmount);
                    beatsLeft[i].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beatsLeft[i].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)

                    beatsLeft[i].gameObject.GetComponent<Image>().material.color = new Color(0.75f * emission, 0.12f * emission, 0.03f * emission, alpha);


                }
            }
            else if (!Tempo.Instance.IsTempoRunning && periodsDisplayable != 0) { periodsDisplayable = 0; }
            else if (!Tempo.Instance.CanGenerateBeatEvents) { EmptyBeats(); }
        }
    }

    private void Beat() 
    {
        
    }

    private void BeatIntervalStart() 
    {
        
    }

    private void BeatIntervalEnd() 
    {
        
    }

    private void BPMChange()
    {
        if (Tempo.Instance && PlayerController.Instance && beatFeedback && normalIntervalArea)
        {
            if (currentPeriodAreaPercentage == 0f) { EmptyBeats(); }

            currentPeriodAreaPercentage = periodAreaPercentage60BPM / Tempo.Instance.TempoPeriod;
            periodsDisplayable = Mathf.CeilToInt(1 / currentPeriodAreaPercentage) * 2;

            float halfNormalTarget = currentPeriodAreaPercentage * Tempo.Instance.BeatAcceptablePercentage;
            normalIntervalArea.anchorMin = new Vector2(0.5f - halfNormalTarget, 0f);
            normalIntervalArea.anchorMax = new Vector2(0.5f + halfNormalTarget, 1f);
            normalIntervalArea.offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
            normalIntervalArea.offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)

            float halfGreatTarget = currentPeriodAreaPercentage * Tempo.Instance.BeatAcceptablePercentage * PlayerController.Instance.GreatThreshold;
            greatIntervalArea.anchorMin = new Vector2(0.5f - halfGreatTarget, 0f);
            greatIntervalArea.anchorMax = new Vector2(0.5f + halfGreatTarget, 1f);
            greatIntervalArea.offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
            greatIntervalArea.offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)

            //float halfPerfectTarget = currentPeriodAreaPercentage * Tempo.Instance.BeatAcceptablePercentage * PlayerController.Instance.PerfectThreshold;
            //perfectIntervalArea.anchorMin = new Vector2(0.5f - halfPerfectTarget, 0f);
            //perfectIntervalArea.anchorMax = new Vector2(0.5f + halfPerfectTarget, 1f);
            //perfectIntervalArea.offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
            //perfectIntervalArea.offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)

            int delta = periodsDisplayable - beatsRight.Count;
            if (delta > 0)
            {
                for (int i = 0; i < delta; i++)
                {
                    RectTransform beatRight = Instantiate(beatFeedback).GetComponent<RectTransform>();
                    beatRight.localScale = Vector3.Scale(beatRight.localScale,  new Vector3(-1, 1, 1));
                    beatRight.GetComponent<Image>().material = new Material(beatRight.GetComponent<Image>().material);
                    beatRight.SetParent(normalIntervalArea.parent, false);
                    beatsRight.Add(beatRight);
                    beatsRight[beatsRight.Count - 1].anchorMin = Vector2.zero;
                    beatsRight[beatsRight.Count - 1].anchorMax = Vector2.zero;
                    beatsRight[beatsRight.Count - 1].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beatsRight[beatsRight.Count - 1].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)

                    RectTransform beatLeft = Instantiate(beatFeedback).GetComponent<RectTransform>();
                    beatLeft.GetComponent<Image>().material = new Material(beatLeft.GetComponent<Image>().material);
                    beatLeft.SetParent(normalIntervalArea.parent, false);
                    beatsLeft.Add(beatLeft);
                    beatsLeft[beatsLeft.Count - 1].anchorMin = Vector2.zero;
                    beatsLeft[beatsLeft.Count - 1].anchorMax = Vector2.zero;
                    beatsLeft[beatsLeft.Count - 1].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beatsLeft[beatsLeft.Count - 1].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)
                }
            }
            else if(delta != 0) 
            { 
                for (int i = 0; i < Mathf.Abs(delta); i++) 
                { 
                    Destroy(beatsRight[beatsRight.Count - 1].gameObject); 
                    beatsRight.RemoveAt(beatsRight.Count - 1);
                    Destroy(beatsLeft[beatsLeft.Count - 1].gameObject);
                    beatsLeft.RemoveAt(beatsLeft.Count - 1);
                } 
            }
        }
    }

    private void EmptyBeats()
    {
        if (beatsRight != null && beatsLeft != null) 
        { 
            foreach (RectTransform beat in beatsRight) { Destroy(beat.gameObject); }
            if (beatsRight.Count != 0) { beatsRight = new List<RectTransform>(); }
            foreach (RectTransform beat in beatsLeft) { Destroy(beat.gameObject); }
            if (beatsLeft.Count != 0) { beatsLeft = new List<RectTransform>(); }
            periodsDisplayable = 0;

        }
    }
}
