using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TempoTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        Tempo.OnBeat += Beat;
    }

    private void OnDisable()
    {
        Tempo.OnBeat -= Beat;
    }

    public abstract void Beat();
}
