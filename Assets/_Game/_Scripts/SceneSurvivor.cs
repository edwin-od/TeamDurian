using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSurvivor : MonoBehaviour
{
    [HideInInspector] public int level = -1;

    private static SceneSurvivor _instance;
    public static SceneSurvivor Instance { get { return _instance; } }
    void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);

        _instance = this;

        DontDestroyOnLoad(this);
    }
}
