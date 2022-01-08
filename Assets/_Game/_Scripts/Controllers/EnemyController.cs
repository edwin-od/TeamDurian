using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : GridMoveable
{
    private int beat;

    public EnemyPattern movementPattern;

    private void Awake()
    {
        beat = 1;
        LevelManager.Instance.RegisterEnemy(this);
    }

    public override void Beat()
    {
        if (!movementPattern)
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
        Move(DIRECTION.DOWN);
    }

    private void OnDestroy()
    {
        LevelManager.Instance.UnregisterEnemy(this);
    }
}
