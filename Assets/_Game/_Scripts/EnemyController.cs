using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : GridMoveable
{
    public override void Beat()
    {
        Move(DIRECTION.DOWN);
    }
}
