using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Menu")]
    public Transform mainMenu;
    public Transform levelsMenu;
    public Transform settingsMenu;

    [Header("Levels")]
    public Levels levels;
    public GameObject levelButtonPrefab;
    public Transform levelButtonContainer;

    [Header("Panel")]
    public Panels currentPanel;
    public enum Panels { Main, Levels, Settings };

    public void Awake()
    {
        _GenerateButtonLevels();

        Toggle(Panels.Main);
    }

    void _GenerateButtonLevels()
    {
        foreach(var level in levels.levels)
        {
            Instantiate(levelButtonPrefab, levelButtonContainer);
        }
    }

    #region Buttons funcs
    public void OnClickLevel() => Toggle(Panels.Levels);

    public void OnClickSettings() => Toggle(Panels.Settings);

    public void OnClickQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void BackToMainMenu() => Toggle(Panels.Main);

    public void Toggle(Panels newPanel)
    {
        this.currentPanel = newPanel;

        mainMenu.gameObject.SetActive(currentPanel == Panels.Main ? true : false);
        levelsMenu.gameObject.SetActive(currentPanel == Panels.Levels ? true : false);
        settingsMenu.gameObject.SetActive(currentPanel == Panels.Settings ? true : false);
    }
    #endregion
}