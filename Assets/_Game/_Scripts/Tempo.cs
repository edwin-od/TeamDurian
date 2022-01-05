using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempo : MonoBehaviour
{
    [SerializeField, Range(0f, 120f)] private float initialDelay = 0f;
    [SerializeField, Range(1, 150)] private int beatsPerMinute = 60;
    [SerializeField, Range(0.02f, 1f)] private float onBeatAcceptablePercentage = 0.25f;

    private bool isTempoRunning = false;
    private bool isTempoPaused = false;
    private bool isOnBeat = false;
    private int totalBeats = 0;
    private double elapsedTime = 0f;
    private float currentPeriod = 0f;
    private float tempoPeriod = 0f;

    [HideInInspector] public delegate void Beat();
    [HideInInspector] public static event Beat OnBeat;

    [HideInInspector] public delegate void IntervalBeatStart();
    [HideInInspector] public static event Beat OnIntervalBeatStart;

    [HideInInspector] public delegate void IntervalBeatEnd();
    [HideInInspector] public static event Beat OnIntervalBeatEnd;

    private static Tempo _instance;
    public static Tempo Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleTempo();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseTempo();
        }
    }

    public bool ToggleTempo()
    {
        if (isTempoRunning)
            StopTempo();
        else
            StartTempo();

        return isTempoRunning;
    }

    public void StartTempo()
    {
        StartCoroutine(TempoLoop());
    }

    public void StopTempo()
    {
        isTempoRunning = false;
    }

    public bool TogglePauseTempo()
    {
        if (isTempoPaused)
            UnpauseTempo();
        else
            PauseTempo();

        return isTempoPaused;
    }

    public void PauseTempo()
    {
        isTempoPaused = true;
    }

    public void UnpauseTempo()
    {
        isTempoPaused = false;
    }

    public double ElapsedTime
    {
        get { return elapsedTime; }
    }

    public int TotalBeats
    {
        get { return totalBeats; }
    }

    public bool IsTempoRunning  // Started and NOT Ended Yet
    {
        get { return isTempoRunning; }
    }

    public bool IsTempoPaused  // Started and Paused
    {
        get { return isTempoRunning && isTempoPaused; }
    }

    public bool IsTempoActive  // Started and NOT Ended Yet and NOT Paused
    {
        get { return isTempoRunning && !isTempoPaused; }
    }

    public bool IsOnBeat
    {
        get { return isOnBeat; }
    }

    public int BPM
    {
        get { return beatsPerMinute; }
        set { beatsPerMinute = Mathf.Clamp(value, 1, 150); ; }
    }

    public float PercentageToBeat
    {
        get { if (tempoPeriod == 0) { return 0f; } else { return 1 - (currentPeriod / tempoPeriod); } }
    }

    IEnumerator TempoLoop()
    {
        isTempoRunning = true;
        isTempoPaused = false;
        elapsedTime = 0f;
        currentPeriod = 0f;

        yield return new WaitForSeconds(initialDelay);

        float tx = Time.realtimeSinceStartup;

        int prevBPM = beatsPerMinute;
        tempoPeriod = 60f / prevBPM;

        while (isTempoRunning)
        {

            if (!isTempoPaused)
            {
                float deltaTime = Time.realtimeSinceStartup - tx;
                tx = Time.realtimeSinceStartup;
                currentPeriod += deltaTime;
                elapsedTime += deltaTime;

                // Manage Dynamic BPM
                if (prevBPM != beatsPerMinute)
                {
                    prevBPM = beatsPerMinute;
                    tempoPeriod = 60f / prevBPM;
                    while (currentPeriod - tempoPeriod >= 0) { currentPeriod -= tempoPeriod; }
                }

                // Manage OnBeat Interval Start and End Events
                float halfBeatInterval = tempoPeriod * Mathf.Clamp(onBeatAcceptablePercentage / 2, 0.01f, 0.499f);
                if(currentPeriod >= tempoPeriod - halfBeatInterval) { if (!isOnBeat) { isOnBeat = true; OnIntervalBeatStart?.Invoke(); } }
                else if(currentPeriod >= halfBeatInterval) { if (isOnBeat) { isOnBeat = false; OnIntervalBeatEnd?.Invoke(); } }

                // Manage Beat Broadcast Event
                if (currentPeriod >= tempoPeriod) { currentPeriod -= tempoPeriod; OnBeat?.Invoke(); }
            }

            yield return null;
        }
    }
}