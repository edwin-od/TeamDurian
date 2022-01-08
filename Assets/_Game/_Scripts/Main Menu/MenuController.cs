using UnityEngine.SceneManagement;
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

    [Header("Game Scene")]
    public string gameSceneName;

    [Header("Level")]
    public int currentLevel;

    public void Awake()
    {
        _GenerateButtonLevels();

        Toggle(Panels.Main);

        DontDestroyOnLoad(this);
    }

    void _GenerateButtonLevels()
    {
        int i = 0;

        foreach(var level in levels.levels)
        {
            i++;
            GameObject go = Instantiate(levelButtonPrefab, levelButtonContainer);
            go.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadGameScene(i));
            go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();

            //Je récupèrerai le vrai nom dans level, étant donné que j'ai pas encore le push avec la variable dans le scriptable object..
            //go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.name;
        }
    }

    void LoadGameScene(int levelIndex)
    {
        Debug.Log("Start level < " + levelIndex + " >");
        currentLevel = levelIndex;
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

        mainMenu.gameObject.SetActive(currentPanel == Panels.Main ? true : false);
        levelsMenu.gameObject.SetActive(currentPanel == Panels.Levels ? true : false);
        settingsMenu.gameObject.SetActive(currentPanel == Panels.Settings ? true : false);
    }
    #endregion
}