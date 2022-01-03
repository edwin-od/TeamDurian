using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMTrigger1 : TempoTrigger
{
    public override void Beat()
    {
        Debug.Log("Beat 1 " + Tempo.ElapsedTime());
    }
}
