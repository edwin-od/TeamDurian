using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempo : MonoBehaviour
{
    [SerializeField, Range(0f, 120f)] private float initialDelay = 0f;
    [SerializeField, Range(1, 1000)] private int beatsPerMinute = 60;
    [SerializeField, Range(0.02f, 1f)] private float onBeatAcceptablePercentage = 0.25f;

    private bool isTempoRunning = false;
    private bool isTempoPaused = false;
    private bool isOnBeat = false;
    private int totalBeats = 0;
    private double elapsedTime = 0f;

    [HideInInspector] public delegate void Beat();
    [HideInInspector] public static event Beat OnBeat;

    [HideInInspector] public delegate void IntervalBeatStart();
    [HideInInspector] public static event Beat OnIntervalBeatStart;

    [HideInInspector] public delegate void IntervalBeatEnd();
    [HideInInspector] public static event Beat OnIntervalBeatEnd;

    [HideInInspector] public static Tempo Get;
    void Awake()
    {
        if (Get == null) Get = this;
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
        isTempoRunning = true;
        isTempoPaused = false;
        totalBeats = 0;
        elapsedTime = 0f;
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
        set { beatsPerMinute = value; }
    }

    public float PercentageToBeat
    {
        get { return ((totalBeats * (60f / beatsPerMinute)) - (float)elapsedTime) / (60f / beatsPerMinute); }
    }

    IEnumerator TempoLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        int beatIntervalCurrentBeats = 0;

        double t0 = Time.realtimeSinceStartupAsDouble; // Used to get to elapsed time
        double tx = t0;    // t(x) -> Current time in-between beats
        double t_pause = 0; // Time after pause

        int prevBPM = beatsPerMinute;
        float tempoPeriod = 60f / prevBPM;

        bool paused = false;

        while (isTempoRunning)
        {
            if(prevBPM != beatsPerMinute)   // BPM changed dynamically
            {
                prevBPM = beatsPerMinute;
                tempoPeriod = 60f / prevBPM;
            }

            if (!isTempoPaused)
            {
                // Manage Pause Interval Compensation
                if (paused)
                {
                    double pause_delay = Time.realtimeSinceStartupAsDouble - t_pause;
                    t0 += pause_delay;
                    tx += pause_delay;

                    t_pause = 0f;
                    paused = false;
                }

                double oldElapsedTime = tx - t0;

                // Manage OnBeat Interval
                float currentBeatTimeline = beatIntervalCurrentBeats * tempoPeriod;
                float halfBeatInterval = tempoPeriod * (onBeatAcceptablePercentage / 2);
                if (oldElapsedTime >= currentBeatTimeline - halfBeatInterval && oldElapsedTime < currentBeatTimeline + halfBeatInterval) { if (!isOnBeat) { isOnBeat = true; OnIntervalBeatStart?.Invoke(); } }
                else { if (isOnBeat) { isOnBeat = false; OnIntervalBeatEnd?.Invoke(); beatIntervalCurrentBeats++; } }

                // Manage Beat Broadcast Event
                if (oldElapsedTime >= totalBeats * tempoPeriod) { totalBeats++; OnBeat?.Invoke(); }

                tx = Time.realtimeSinceStartupAsDouble;
                elapsedTime = tx - t0;

                yield return null;
            }
            else
            {
                if (!paused)
                {
                    paused = true;
                    t_pause = Time.realtimeSinceStartupAsDouble;
                }
                yield return null;
            }
        }
    }
}
