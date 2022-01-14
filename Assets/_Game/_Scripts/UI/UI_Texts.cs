using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Texts : MonoBehaviour
{

    public TMPro.TextMeshProUGUI scoreText;

    private static UI_Texts _instance;
    public static UI_Texts Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }


    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

}
