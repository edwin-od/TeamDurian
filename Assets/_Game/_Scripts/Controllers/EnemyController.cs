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
    }

    public override void Beat()
    {
        Debug.Log("Beat");

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
}
