using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempo : MonoBehaviour
{
    [SerializeField] private float initialDelay = 0f;
    [SerializeField] private int beatsPerMinute = 100;

    private bool isTempoRunning = false;
    private bool isTempoPaused = false;
    private int totalBeats = 0;

    [HideInInspector] public delegate void Beat();
    [HideInInspector] public static event Beat OnBeat;

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
        StartCoroutine(TempoLoop(60.0f / beatsPerMinute));
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

    public float ElapsedTime()
    {
        return totalBeats * (60.0f / beatsPerMinute);
    }

    public int TotalBeats()
    {
        return totalBeats;
    }

    public bool IsTempoActive()
    {
        return isTempoRunning;
    }

    public bool IsTempoPaused()
    {
        return isTempoPaused;
    }

    IEnumerator TempoLoop(float tempoPeriod)
    {
        Debug.Log("Tempo Started");

        yield return new WaitForSeconds(initialDelay);

        while (isTempoRunning)
        {
            if (!isTempoPaused)
            {
                totalBeats++;
                if (OnBeat != null) // Check if there are subscribers to the event OnBeat
                    OnBeat();

                yield return new WaitForSeconds(tempoPeriod);
            }
            else
                yield return null;
        }
        Debug.Log("Tempo Ended");
    }
}
