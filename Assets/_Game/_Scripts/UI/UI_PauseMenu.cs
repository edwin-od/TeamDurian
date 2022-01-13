using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour
{

    public GameObject gameplayUI;
    public GameObject pauseUI;

    private static UI_PauseMenu _instance;
    public static UI_PauseMenu Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);

        gameplayUI.SetActive(true);
        pauseUI.SetActive(false);
    }
    private void OnEnable()
    {
        Tempo.OnPause += Pause;
        Tempo.OnUnpause += UnPause;
    }

    private void OnDisable()
    {
        Tempo.OnPause -= Pause;
        Tempo.OnUnpause -= UnPause;
    }

    public void Pause()
    {
        gameplayUI.SetActive(false);
        pauseUI.SetActive(true);
    }

    public void UnPause()
    {
        gameplayUI.SetActive(true);
        pauseUI.SetActive(false);
    }

    public void PauseButton()
    {
        Pause();
        if (Tempo.Instance) { Tempo.Instance.PauseTempo(); }
    }

    public void UnPauseButton()
    {
        UnPause();
        if (Tempo.Instance) { Tempo.Instance.UnpauseTempo(); }
    }

    public void RestartLevelButton()
    {
        if (LevelManager.Instance) { UnPauseButton(); LevelManager.Instance.RestartCurrentLevel(); }
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
