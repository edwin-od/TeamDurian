using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GridMoveable
{
    private bool skipBeat = false;

    [HideInInspector] public static Player Get;
    void Awake()
    {
        if (Get == null) Get = this;
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
        /*if (Input.GetKeyDown(KeyCode.W))
            MoveTile(DIRECTION.UP);
        if (Input.GetKeyDown(KeyCode.S))
            MoveTile(DIRECTION.DOWN);*/
        if (Input.GetKeyDown(KeyCode.D))
            MoveTile(DIRECTION.RIGHT);
        if (Input.GetKeyDown(KeyCode.A))
            MoveTile(DIRECTION.LEFT);

        if(Input.GetKeyDown(KeyCode.Q))
            Tempo.Get.BPM -= 5;
        if (Input.GetKeyDown(KeyCode.E))
            Tempo.Get.BPM += 5;
    }

    private void MoveTile(DIRECTION Direction)
    {
        if (!skipBeat)
        {
            if (Tempo.Get)
            {
                if (Tempo.Get.IsTempoPaused || !Tempo.Get.IsTempoRunning)
                    return;
                if (Tempo.Get.IsTempoActive && !Tempo.Get.IsOnBeat)
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
