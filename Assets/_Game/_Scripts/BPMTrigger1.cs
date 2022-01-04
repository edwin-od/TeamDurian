using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMTrigger1 : GridMoveable
{
    public override void Beat()
    {
        Debug.Log("Beat 1");
    }

    private void Update()
    {
        if (!IsMoving)
        {
            if (Input.GetKey(KeyCode.W))
                StartCoroutine(Move(DIRECTION.UP));
            if (Input.GetKey(KeyCode.S))
                StartCoroutine(Move(DIRECTION.DOWN));
            if (Input.GetKey(KeyCode.D))
                StartCoroutine(Move(DIRECTION.RIGHT));
            if (Input.GetKey(KeyCode.A))
                StartCoroutine(Move(DIRECTION.LEFT));
        }
    }

}
