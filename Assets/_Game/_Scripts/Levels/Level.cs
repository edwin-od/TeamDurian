using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    public enum EnemyType { Default };

    [System.Serializable]
    public class Wave
    {
        [HideInInspector] public string waveName = "Wave 0";
        public Loop loop = null;
        [HideInInspector] public List<EnemySpawn> spawns = new List<EnemySpawn>();
    }

    [System.Serializable]
    public class EnemySpawn
    {
        public bool spawn = false;
        public EnemyType enemyType = EnemyType.Default;
    }

    public string levelName = "Level 0";

    public List<Wave> waves = new List<Wave>();

    public int gridWidth = 13;
    public int gridHeight = 13;

    public float gridTileSizeX = 1;
    public float gridTileSizeY = 1;

    public float cameraHeight = 3f;

    public Color tileColor1 = Color.white;
    public Color tileColor2 = Color.black;

#if UNITY_EDITOR
    public const int VAR_SPACE = 6;
    public int tab = 0;
    public Vector2 scrollPos;
    public int selectedWave = 0;
#endif

    private void OnValidate()
    {
        for(int i = 0; i < waves.Count; i++)
        {
            waves[i].waveName = "Wave " + (i + 1);
        }
    }
}

#region Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{

    void Start() { PopulateWaveSpawns(target as Level, false); }

    override public void OnInspectorGUI()
    {
        Level level = target as Level;
        SerializedObject serializedLevel = new SerializedObject(target);

        GUILayout.Space(Level.VAR_SPACE);

        level.tab = GUILayout.Toolbar(level.tab, new string[] { "Level Settings", "Wave Settings" });
        switch (level.tab)
        {
            case 0:
                {

                    GUILayout.Space(Level.VAR_SPACE);
                    GUILayout.Space(Level.VAR_SPACE);
                    GUILayout.Space(Level.VAR_SPACE);

                    EditorGUI.BeginChangeCheck();
                    level.gridWidth = EditorGUILayout.IntSlider("Grid Width", level.gridWidth, 3, 100);
                    level.gridHeight = EditorGUILayout.IntSlider("Grid Height", level.gridHeight, 3, 100);
                    if (EditorGUI.EndChangeCheck()) { PopulateWaveSpawns(level, true); }

                    GUILayout.Space(Level.VAR_SPACE);
                    GUILayout.Space(Level.VAR_SPACE);

                    level.gridTileSizeX = EditorGUILayout.Slider("Tile Size X", level.gridTileSizeX, 0.1f, 10f);
                    level.gridTileSizeY = EditorGUILayout.Slider("Tile Size Y", level.gridTileSizeY, 0.1f, 10f);

                    GUILayout.Space(Level.VAR_SPACE);
                    GUILayout.Space(Level.VAR_SPACE);

                    level.tileColor1 = EditorGUILayout.ColorField("Tile Color 1", level.tileColor1);
                    level.tileColor2 = EditorGUILayout.ColorField("Tile Color 2", level.tileColor2);

                    DrawUILine(Color.grey);

                    level.cameraHeight = EditorGUILayout.Slider("Camera Height", level.cameraHeight, 0.1f, 10f);

                    DrawUILine(Color.grey);

                    EditorGUI.BeginChangeCheck();
                    serializedLevel.Update();
                    EditorGUILayout.PropertyField(serializedLevel.FindProperty("waves"), true);
                    serializedLevel.ApplyModifiedProperties();
                    if (EditorGUI.EndChangeCheck()) { PopulateWaveSpawns(level, false); }

                    break;
                }
            case 1:
                {
                    if (level.waves == null || level.waves.Count == 0)
                    {
                        Debug.LogWarning("You have no waves to display! Add some in the \"Level Settings\" tab, in the \"Waves\" list.");
                        goto default;
                    }

                    GUILayout.Space(Level.VAR_SPACE);
                    GUILayout.Space(Level.VAR_SPACE);
                    GUILayout.Space(Level.VAR_SPACE);

                    string[] options = new string[level.waves.Count];
                    for (int i = 0; i < level.waves.Count; i++) { options[i] = level.waves[i].waveName; }
                    level.selectedWave = EditorGUILayout.Popup("Wave", level.selectedWave, options);
                    if (level.selectedWave >= level.waves.Count)
                        level.selectedWave = level.waves.Count - 1;

                    DrawUILine(Color.grey);

                    level.scrollPos = EditorGUILayout.BeginScrollView(level.scrollPos, GUILayout.Width(level.gridWidth * 21), GUILayout.Height(level.gridHeight * 19));
                    int spawnIndex = 0;
                    for (int y = 0; y < level.gridHeight; y++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        for (int x = 0; x < level.gridWidth; x++)
                        {
                            bool isNotSpawnable = 
                                (x != 0 && x != level.gridWidth - 1 && y != 0 && y != level.gridHeight - 1) || 
                                (x == 0 && y == 0) || 
                                (x == 0 && y == level.gridHeight - 1) || 
                                (x == level.gridWidth - 1 && y == 0) ||
                                (x == level.gridWidth - 1 && y == level.gridHeight - 1);
                            
                            EditorGUI.BeginDisabledGroup(isNotSpawnable);

                            if(!isNotSpawnable && level.waves != null && level.waves[level.selectedWave].spawns != null && spawnIndex < level.waves[level.selectedWave].spawns.Count)
                                level.waves[level.selectedWave].spawns[spawnIndex].spawn = GUILayout.Toggle(level.waves[level.selectedWave].spawns[spawnIndex].spawn, "");
                            else
                                GUILayout.Toggle(false, "");

                            EditorGUI.EndDisabledGroup();

                            if (!isNotSpawnable)
                                spawnIndex++;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();

                    break;
                }
            default:
                level.tab = 0;
                break;
        }

    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        GUILayout.Space(Level.VAR_SPACE);
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
        GUILayout.Space(Level.VAR_SPACE);
    }

    public void PopulateWaveSpawns(Level level, bool reset)
    {
        if (level.waves != null)
        {
            for (int wave = 0; wave < level.waves.Count; wave++)
            {
                if (level.waves[wave].spawns == null || level.waves[wave].spawns.Count == 0 || reset)
                {
                    level.waves[wave].spawns = new List<Level.EnemySpawn>();
                    if (level.waves[wave].spawns != null)
                    {
                        for (int y = 0; y < level.gridHeight; y++)
                        {
                            for (int x = 0; x < level.gridWidth; x++)
                            {
                                if ((x != 0 && x != level.gridWidth - 1 && y != 0 && y != level.gridHeight - 1) ||
                                    (x == 0 && y == 0) ||
                                    (x == 0 && y == level.gridHeight - 1) ||
                                    (x == level.gridWidth - 1 && y == 0) ||
                                    (x == level.gridWidth - 1 && y == level.gridHeight - 1))
                                { level.waves[wave].spawns.Add(new Level.EnemySpawn()); }
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnDisable() { EditorUtility.SetDirty(target as Level); }
}
#endif
#endregion