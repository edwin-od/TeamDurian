using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BeatFeedback : MonoBehaviour
{

    [SerializeField] private RectTransform targetArea;
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
        if(Tempo.Instance && beatFeedback && targetArea)
        {
            if (Tempo.Instance.isActiveAndEnabled && Tempo.Instance.CanGenerateBeatEvents)
            {
                if (periodsDisplayable == 0) { BPMChange(); }
                
                for(int i = 0; i < beatsRight.Count; i++)
                {
                    float position = (currentPeriodAreaPercentage * (Tempo.Instance.PercentageToBeat + i));

                    beatsRight[i].anchorMin = new Vector2(position - (beatFeedbackWidth / 2) + 0.5f, 0f);
                    beatsRight[i].anchorMax = new Vector2(position + (beatFeedbackWidth / 2) + 0.5f, 1f);
                    beatsRight[i].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beatsRight[i].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)

                    beatsLeft[i].anchorMin = new Vector2(-position - (beatFeedbackWidth / 2) + 0.5f, 0f);
                    beatsLeft[i].anchorMax = new Vector2(-position + (beatFeedbackWidth / 2) + 0.5f, 1f);
                    beatsLeft[i].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beatsLeft[i].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)
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
        if (Tempo.Instance && beatFeedback && targetArea)
        {
            if (currentPeriodAreaPercentage == 0f) { EmptyBeats(); }

            currentPeriodAreaPercentage = Tempo.Instance.BPM * periodAreaPercentage60BPM / 60f;
            float halfTarget = currentPeriodAreaPercentage * Tempo.Instance.BeatAcceptablePercentage;
            targetArea.anchorMin = new Vector2(0.5f - halfTarget, 0f);
            targetArea.anchorMax = new Vector2(0.5f + halfTarget, 1f);
            periodsDisplayable = Mathf.CeilToInt(1 / currentPeriodAreaPercentage) * 2;

            int delta = periodsDisplayable - beatsRight.Count;
            if (delta > 0)
            {
                for (int i = 0; i < delta; i++)
                {
                    RectTransform beatRight = Instantiate(beatFeedback).GetComponent<RectTransform>();
                    beatRight.localScale = Vector3.Scale(beatRight.localScale,  new Vector3(-1, 1, 1));
                    beatRight.SetParent(targetArea.parent, false);
                    beatsRight.Add(beatRight);
                    beatsRight[beatsRight.Count - 1].anchorMin = Vector2.zero;
                    beatsRight[beatsRight.Count - 1].anchorMax = Vector2.zero;
                    beatsRight[beatsRight.Count - 1].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beatsRight[beatsRight.Count - 1].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)

                    RectTransform beatLeft = Instantiate(beatFeedback).GetComponent<RectTransform>();
                    beatLeft.SetParent(targetArea.parent, false);
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
