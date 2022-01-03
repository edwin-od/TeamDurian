using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempo : MonoBehaviour
{
    [SerializeField, Range(0f, 120f)] private float initialDelay = 0f;
    [SerializeField, Range(1, 1000)] private int beatsPerMinute = 60;

    private static bool isTempoRunning = false;
    private static bool isTempoPaused = false;
    private static int totalBeats = 0;
    private static double elapsedTime = 0f;

    [HideInInspector] public delegate void Beat();
    [HideInInspector] public static event Beat OnBeat;

    [HideInInspector] public static Tempo Get;
    void Awake()
    {
        if (Get == null) Get = this;
        else Destroy(this.gameObject);
    }

    #region TEST
    ///////////    TEST    ///////////
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
    ///////////////////////////////////
    #endregion

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

    public static double ElapsedTime
    {
        get { return elapsedTime; }
    }

    public static int TotalBeats
    {
        get { return totalBeats; }
    }

    public static bool IsTempoActive  // Started and NOT Ended Yet
    {
        get { return isTempoRunning; }
    }

    public static bool IsTempoPaused  // Started and Paused
    {
        get { return isTempoRunning && isTempoPaused; }
    }

    public int BPM
    {
        get { return beatsPerMinute; }
        set { beatsPerMinute = value; }
    }

    IEnumerator TempoLoop()
    {
        yield return new WaitForSeconds(initialDelay);

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
                if (paused)
                {
                    double pause_delay = Time.realtimeSinceStartupAsDouble - t_pause;
                    t0 += pause_delay;
                    tx += pause_delay;

                    t_pause = 0f;
                    paused = false;
                }

                if (tx - t0 >= totalBeats * tempoPeriod)
                {
                    totalBeats++;
                    if (OnBeat != null) // Check if there are subscribers to the event OnBeat
                        OnBeat();
                }

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
