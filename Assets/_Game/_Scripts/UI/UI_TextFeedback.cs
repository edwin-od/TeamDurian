using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TextFeedback : MonoBehaviour
{
    public TextMeshProUGUI startingTimer;
    public TextMeshProUGUI syncText;

    public static UI_TextFeedback Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(DisplayStartingTimer());
    }

    IEnumerator DisplayStartingTimer()
    {
        for(int i = Tempo.Instance.InitialDelay; i < 0; i--)
        {
            startingTimer.text = "" + i;
            yield return new WaitForSeconds(1);
        }
    }
}
