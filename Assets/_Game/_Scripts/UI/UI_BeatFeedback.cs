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
    private List<RectTransform> beats = new List<RectTransform>();

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
                
                for(int i = 0; i < beats.Count; i++)
                {
                    float position = currentPeriodAreaPercentage * (Tempo.Instance.PercentageToBeat + Tempo.Instance.BeatAcceptablePercentage + i - 1);
                    beats[i].anchorMin = new Vector2(position - beatFeedbackWidth, 0f);
                    beats[i].anchorMax = new Vector2(position, 1f);
                    beats[i].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beats[i].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)
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
            targetArea.anchorMin = new Vector2(0f, 0f);
            targetArea.anchorMax = new Vector2(currentPeriodAreaPercentage * Tempo.Instance.BeatAcceptablePercentage * 2, 1f);
            periodsDisplayable = Mathf.CeilToInt(1 / currentPeriodAreaPercentage) + 1;

            int delta = periodsDisplayable - beats.Count;
            if (delta > 0)
            {
                for (int i = 0; i < delta; i++)
                {
                    RectTransform beat = Instantiate(beatFeedback).GetComponent<RectTransform>();
                    beat.SetParent(targetArea.parent, false);
                    beats.Add(beat);
                    beats[beats.Count - 1].anchorMin = Vector2.zero;
                    beats[beats.Count - 1].anchorMax = Vector2.zero;
                    beats[beats.Count - 1].offsetMin = Vector2.zero; // offsetMin -> Vector2(left, bottom)
                    beats[beats.Count - 1].offsetMax = Vector2.zero; // offsetMax -> Vector2(-right, -top)
                }
            }
            else if(delta != 0) { for (int i = 0; i < Mathf.Abs(delta); i++) { Destroy(beats[beats.Count - 1].gameObject); beats.RemoveAt(beats.Count - 1); } }
        }
    }

    private void EmptyBeats()
    {
        if (beats != null) { foreach (RectTransform beat in beats) { Destroy(beat.gameObject); } periodsDisplayable = 0; }
        if (beats.Count != 0) { beats = new List<RectTransform>(); }
    }
}
