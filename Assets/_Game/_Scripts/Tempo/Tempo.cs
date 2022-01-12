using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempo : MonoBehaviour
{
    [SerializeField, Range(1, 500)] private int beatsPerMinute = 60;
    [SerializeField, Range(0.01f, 0.499f)] private float onBeatAcceptablePercentage = 0.25f;

    private bool isTempoRunning = false;
    private bool isTempoPaused = false;
    private bool canGenerateBeatEvents = true;
    private bool isFirstBeatEver = true;
    private bool isOnBeat = false;
    private int totalBeats = 0;
    private double elapsedTime = 0f;
    private float currentPeriod = 0f;
    private float tempoPeriod = 0f;

    private Coroutine tempoCoroutine = null;

    public delegate void Beat();
    public static event Beat OnBeat;

    public delegate void SilentBeat();
    public static event SilentBeat OnSilentBeat;

    public delegate void BPMChange();
    public static event BPMChange OnBPMChange;

    public delegate void IntervalBeatStart();
    public static event Beat OnIntervalBeatStart;

    public delegate void IntervalBeatEnd();
    public static event Beat OnIntervalBeatEnd;

    public delegate void Pause();
    public static event Pause OnPause;

    public delegate void Unpause();
    public static event Unpause OnUnpause;

    private static Tempo _instance;
    public static Tempo Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Tab))
        //    ToggleTempo();
        if(Input.GetKeyDown(KeyCode.Escape))
            TogglePauseTempo();
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
        if (tempoCoroutine != null) { StopCoroutine(tempoCoroutine); }
        StartCoroutine(TempoLoop(0f));
    }

    public void StartTempo(float initialDelay)
    {
        if (tempoCoroutine != null) { StopCoroutine(tempoCoroutine); }
        StartCoroutine(TempoLoop(initialDelay));
    }

    public void StartTempo(int BPM)
    {
        this.BPM = BPM;
        if (tempoCoroutine != null) { StopCoroutine(tempoCoroutine); }
        tempoCoroutine = StartCoroutine(TempoLoop(0f));
    }

    public void StartTempo(int BPM, float initialDelay)
    {
        this.BPM = BPM;
        if (tempoCoroutine != null) { StopCoroutine(tempoCoroutine); }
        tempoCoroutine = StartCoroutine(TempoLoop(initialDelay));
    }

    public void StopTempo()
    {
        isTempoRunning = false;
        if (tempoCoroutine != null) { StopCoroutine(tempoCoroutine); }
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
        Time.timeScale = 0f;
    }

    public void UnpauseTempo()
    {
        isTempoPaused = false;
        Time.timeScale = 1f;
    }

    public bool CanGenerateBeatEvents
    {
        get { return canGenerateBeatEvents; }
        set { canGenerateBeatEvents = value; }
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

    public bool IsFirstBeatEver
    {
        get { return isFirstBeatEver; }
    }

    public bool IsOnBeat
    {
        get { return isOnBeat; }
    }

    public int BPM
    {
        get { return beatsPerMinute; }
        set { beatsPerMinute = Mathf.Clamp(value, 1, 500); }
    }

    public float PercentageToBeat
    {
        get { if (tempoPeriod == 0) { return 0f; } else { return 1 - (currentPeriod / tempoPeriod); } }
    }

    public float TempoPeriod
    {
        get { return tempoPeriod; }
    }

    public float BeatAcceptablePercentage
    {
        get { return onBeatAcceptablePercentage; }
    }

    IEnumerator TempoLoop(float initialDelay)
    {
        int prevBPM = beatsPerMinute;
        tempoPeriod = 60f / prevBPM;

        yield return new WaitForSeconds(initialDelay);

        isTempoRunning = true;
        isTempoPaused = false;
        isFirstBeatEver = true;
        elapsedTime = 0f;
        currentPeriod = 0f;

        float tx = Time.realtimeSinceStartup - tempoPeriod;
        float tpause = 0;

        while (isTempoRunning)
        {

            if (!isTempoPaused)
            {
                float deltaTime = 0;

                // Manage Pause Compensation
                if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; OnUnpause?.Invoke(); }
                else { deltaTime = Time.realtimeSinceStartup - tx; }

                tx = Time.realtimeSinceStartup;
                currentPeriod += deltaTime;
                elapsedTime += deltaTime;

                // Manage Dynamic BPM
                if (prevBPM != beatsPerMinute)
                {
                    prevBPM = beatsPerMinute;
                    tempoPeriod = 60f / prevBPM;
                    currentPeriod = tempoPeriod;
                    OnBPMChange?.Invoke();
                }

                // Manage OnBeat Interval -> Start and End Events
                float halfBeatInterval = tempoPeriod * onBeatAcceptablePercentage;
                if (currentPeriod >= tempoPeriod - halfBeatInterval) { if (!isOnBeat) { isOnBeat = true; if (canGenerateBeatEvents) { OnIntervalBeatStart?.Invoke(); } } }
                else if (currentPeriod > halfBeatInterval) { if (isOnBeat) { isOnBeat = false; if (canGenerateBeatEvents) { OnIntervalBeatEnd?.Invoke(); } } }

                // Manage Beat Broadcast Event
                if (currentPeriod >= tempoPeriod) 
                { 
                    currentPeriod -= tempoPeriod; 
                    if (isFirstBeatEver && elapsedTime > 2 * tempoPeriod) { isFirstBeatEver = false; } // first beat passed

                    if (canGenerateBeatEvents) { OnBeat?.Invoke(); } 
                    else { OnSilentBeat?.Invoke(); } 
                }
            }
            else if (tpause == 0) { tpause = Time.realtimeSinceStartup; OnPause?.Invoke(); }

            yield return null;
        }
    }
}
