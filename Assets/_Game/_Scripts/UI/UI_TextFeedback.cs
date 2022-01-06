using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TextFeedback : MonoBehaviour
{
    public TextMeshProUGUI syncText;

    public static UI_TextFeedback Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }
}
