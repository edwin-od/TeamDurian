using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : GridMoveable
{
    private int beat, action;

    public EnemyPattern movementPattern;
    public Vector3 scaleA = new Vector3(.67f, .67f, 1);
    public Vector3 scaleB = new Vector3(.9f, .67f, 1);
    private bool switchScale;
  

    private void Awake()
    {
        beat = 1;
        action = 0;
        LevelManager.Instance.RegisterEnemy(this);
    }

    public override void Beat()
    {
        if (!movementPattern || (Tempo.Instance && Tempo.Instance.IsFirstBeatEver))
            return;

        if (beat == (int)movementPattern.beatEvery)
        {
            Action();
            beat = 1;
        }
        else
            beat++;
    }

    void Action()
    {
        if (movementPattern.directions.Count == 0)
            return;

        Move(movementPattern.directions[action]);

        //testing ....
        var timePerBeat = Tempo.Instance.TempoPeriod / 2;

        transform.GetChild(0).DOScale(scaleA, timePerBeat / 2).SetEase(Ease.InCubic).OnComplete(() => transform.GetChild(0).DOScale(Vector3.one, timePerBeat / 2).SetEase(Ease.OutCubic));

        action++;
        if (action >= movementPattern.directions.Count)
            action = 0;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.UnregisterEnemy(this);
    }
}
