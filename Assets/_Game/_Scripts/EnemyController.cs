using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : GridMoveable
{
    public override void Beat()
    {
        StartCoroutine(Move(DIRECTION.DOWN));
    }
}
