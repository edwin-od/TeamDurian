using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : GridMoveable
{
    private int beat, action;

    public EnemyPattern movementPattern;

  

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

        //Ou je peux r�cup�rer la dur�e des beats ? depuis quel script ? 
        //transform.DOScaleX()

    }

    void Action()
    {
        if (movementPattern.directions.Count == 0)
            return;

        Move(movementPattern.directions[action]);

        action++;
        if (action >= movementPattern.directions.Count)
            action = 0;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.UnregisterEnemy(this);
    }
}
