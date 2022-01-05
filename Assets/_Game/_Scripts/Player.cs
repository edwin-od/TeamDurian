using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GridMoveable
{
    private bool skipBeat = false;

    private static Player _instance;
    public static Player Instance { get { return _instance; } }
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
    }

    private void OnDisable()
    {
        Tempo.OnBeat -= Beat;
        Tempo.OnIntervalBeatStart -= BeatIntervalStart;
        Tempo.OnIntervalBeatEnd -= BeatIntervalEnd;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Z))
            MoveTile(DIRECTION.UP);
        if (Input.GetKeyDown(KeyCode.S))
            MoveTile(DIRECTION.DOWN);
        if (Input.GetKeyDown(KeyCode.D))
            MoveTile(DIRECTION.RIGHT);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q))
            MoveTile(DIRECTION.LEFT);

        if(Input.GetKeyDown(KeyCode.Q))
            Tempo.Instance.BPM -= 5;
        if (Input.GetKeyDown(KeyCode.E))
            Tempo.Instance.BPM += 5;
    }

    private void MoveTile(DIRECTION Direction)
    {
        if (!skipBeat)
        {
            if (Tempo.Instance)
            {
                if (Tempo.Instance.IsTempoPaused || !Tempo.Instance.IsTempoRunning)
                    return;
                if (Tempo.Instance.IsTempoActive && !Tempo.Instance.IsOnBeat)
                {
                    skipBeat = true;
                    return;
                }
            }

            Move(Direction);
            skipBeat = true;
        }
    }

    public override void Beat() { }

    private void BeatIntervalStart() { }

    private void BeatIntervalEnd() { skipBeat = false; }
}
