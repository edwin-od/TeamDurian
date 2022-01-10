using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : GridMoveable
{
    private int beat, action;

    public EnemyPattern movementPattern;
    public Vector3 scaleA = new Vector3(.5f, 1, .8f);
    public Vector3 scaleB = new Vector3(.8f, 1, .5f);
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

        beatLength = Tempo.Instance.TempoPeriod * .98f;

        //testing ....
        var timePerBeat = Tempo.Instance.TempoPeriod / 2;

        transform.GetChild(0).DOScale(scaleA, timePerBeat).SetEase(Ease.OutExpo).OnComplete(() => transform.GetChild(0).DOScale(scaleB, timePerBeat).SetEase(Ease.InExpo));

        action++;
        if (action >= movementPattern.directions.Count)
            action = 0;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.UnregisterEnemy(this);
    }
}
