using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Menu")]
    public Transform mainMenu;
    public Transform levelsMenu;
    public Transform settingsMenu;

    [Header("Return")]
    public GameObject returnButton;

    [Header("Levels")]
    public Levels levels;
    public GameObject levelButtonPrefab;
    public Transform levelButtonContainer;

    [Header("Panel")]
    public Panels currentPanel;
    public enum Panels { Main, Levels, Settings };

    [Header("Game Scene")]
    public string gameSceneName;


    public static MenuController Instance;

    public void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);

        Instance = this;

        _GenerateButtonLevels();

        Toggle(Panels.Main);
    }

    void _GenerateButtonLevels()
    {
        int i = 0;

        foreach(var level in levels.levels)
        {
            i++;
            int a = i;
            GameObject go = Instantiate(levelButtonPrefab, levelButtonContainer);
            go.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadGameScene(a));
            //go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();

            //Je r?cup?rerai le vrai nom dans level, ?tant donn? que j'ai pas encore le push avec la variable dans le scriptable object..
            //go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.name;
        }
    }

    void LoadGameScene(int levelIndex)
    {
        Debug.Log("Start level < " + levelIndex + " >");
        if (SceneSurvivor.Instance) { SceneSurvivor.Instance.level = levelIndex - 1; }
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
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

        returnButton.gameObject.SetActive(currentPanel == Panels.Levels || currentPanel == Panels.Settings);
        mainMenu.gameObject.SetActive(currentPanel == Panels.Main);
        levelsMenu.gameObject.SetActive(currentPanel == Panels.Levels);
        settingsMenu.gameObject.SetActive(currentPanel == Panels.Settings);
    }
    #endregion
}