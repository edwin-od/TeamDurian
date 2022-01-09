using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    [System.Serializable]
    public class Wave
    {
        [HideInInspector] public string waveName = "Wave 0";
        public bool isTransition = false;
        public Loop loop = null;
        [HideInInspector] public List<EnemySpawn> spawns = new List<EnemySpawn>();
    }

    [System.Serializable]
    public class EnemySpawn
    {
        public bool spawn = false;
        public EnemyPattern pattern = null;
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
        int waveIndex = 0;
        for(int i = 0; i < waves.Count; i++)
        {
            if (!waves[i].isTransition) { waves[i].waveName = "Wave " + (waveIndex + 1); waveIndex++; }
            else { waves[i].waveName = "Transition"; }
        }
    }
}

#region Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    private string noNonTranstionalWavesFoundWarning = "You have no waves to display! Add some in the \"Level Settings\" tab, in the \"Waves\" list.";

    void Start() { PopulateWaveSpawns(target as Level, false); }

    override public void OnInspectorGUI()
    {
        Level level = target as Level;
        SerializedObject serializedLevel = new SerializedObject(target);

        GUILayout.Space(Level.VAR_SPACE);
        GUILayout.Space(Level.VAR_SPACE);
        GUILayout.Space(Level.VAR_SPACE);

        level.tab = GUILayout.Toolbar(level.tab, new string[] { "Level Settings", "Wave Settings" });
        switch (level.tab)
        {
            case 0:
                {
                    level.levelName = EditorGUILayout.TextField("Level Name", level.levelName);

                    DrawUILine(Color.grey);

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
                    int nonTransitionalWaves = 0;
                    for (int i = 0; i < level.waves.Count; i++) { if (!level.waves[i].isTransition) { nonTransitionalWaves++; } }
                    if (level.waves == null || level.waves.Count == 0 || nonTransitionalWaves == 0) { Debug.LogWarning(noNonTranstionalWavesFoundWarning); goto default; }

                    if (level.selectedWave < 0 || level.selectedWave >= level.waves.Count || level.waves[level.selectedWave].isTransition)
                    {
                        bool found = false;
                        for(int i = 0; i < level.waves.Count; i++) { if (!level.waves[i].isTransition) { level.selectedWave = i; found = true; break; } }
                        if (!found) { Debug.LogWarning(noNonTranstionalWavesFoundWarning); goto default; }
                    }

                    EnemyPattern[] patterns = EnemyPattern.GetPatterns();

                    GUILayout.Space(Level.VAR_SPACE);
                    GUILayout.Space(Level.VAR_SPACE);
                    GUILayout.Space(Level.VAR_SPACE);

                    List<string> options = new List<string>();
                    List<int> optionsIndices = new List<int>();
                    for (int i = 0; i < level.waves.Count; i++) { if (!level.waves[i].isTransition) { options.Add(level.waves[i].waveName); optionsIndices.Add(i); } }
                    level.selectedWave = EditorGUILayout.Popup("Wave", options.IndexOf(level.waves[level.selectedWave].waveName), options.ToArray());
                    level.selectedWave = optionsIndices[level.selectedWave];

                    DrawUILine(Color.grey);

                    float width = level.gridWidth * 28;
                    float height = level.gridHeight * 21;

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    level.scrollPos = EditorGUILayout.BeginScrollView(level.scrollPos, GUILayout.Width(width), GUILayout.Height(height));
                    List<Level.EnemySpawn> bottomSpawns = new List<Level.EnemySpawn>();
                    int spawnIndex = 0;
                    for (int y = 0; y < level.gridHeight; y++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        for (int x = 0; x < level.gridWidth; x++)
                        {
                            bool isBottom = y == level.gridHeight - 1;
                            bool isTop = y == 0;
                            bool isLeft = x == 0;
                            bool isRight = x == level.gridWidth - 1;
                            bool isNotSpawnable = (!isLeft && !isRight && !isTop && !isBottom) || (isTop && isLeft) || (isLeft && isBottom) || (isRight && isTop) || (isBottom && isRight);
                            
                            EditorGUI.BeginDisabledGroup(isNotSpawnable);

                            if (!isNotSpawnable && level.waves != null && level.waves[level.selectedWave].spawns != null && spawnIndex < level.waves[level.selectedWave].spawns.Count)
                            {
                                if (isTop)
                                {
                                    EditorGUILayout.BeginVertical();
                                    EnemyTypeButton(level.waves[level.selectedWave].spawns[spawnIndex], patterns);
                                    level.waves[level.selectedWave].spawns[spawnIndex].spawn = GUILayout.Toggle(level.waves[level.selectedWave].spawns[spawnIndex].spawn, "");
                                    EditorGUILayout.EndVertical();
                                }
                                else if (isLeft)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.BeginVertical();
                                    GUILayout.Space(4f);
                                    EnemyTypeButton(level.waves[level.selectedWave].spawns[spawnIndex], patterns);
                                    EditorGUILayout.EndVertical();
                                    GUILayout.Space(2.25f);
                                    level.waves[level.selectedWave].spawns[spawnIndex].spawn = GUILayout.Toggle(level.waves[level.selectedWave].spawns[spawnIndex].spawn, "");
                                    EditorGUILayout.EndHorizontal();
                                }
                                else if (isBottom)
                                {
                                    EditorGUILayout.BeginVertical();
                                    level.waves[level.selectedWave].spawns[spawnIndex].spawn = GUILayout.Toggle(level.waves[level.selectedWave].spawns[spawnIndex].spawn, "");
                                    EnemyTypeButton(level.waves[level.selectedWave].spawns[spawnIndex], patterns);
                                    EditorGUILayout.EndVertical();
                                }
                                else if (isRight)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    level.waves[level.selectedWave].spawns[spawnIndex].spawn = GUILayout.Toggle(level.waves[level.selectedWave].spawns[spawnIndex].spawn, "");
                                    EditorGUILayout.BeginVertical();
                                    GUILayout.Space(4f);
                                    EnemyTypeButton(level.waves[level.selectedWave].spawns[spawnIndex], patterns);
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            else
                            {
                                if ((isTop || isBottom) && isLeft)
                                {
                                    EditorGUILayout.BeginVertical();
                                    GridUnusableTile(true);    
                                    GridUnusableTile(true);    
                                    EditorGUILayout.EndVertical();
                                    GUILayout.Space(2.25f);
                                    EditorGUILayout.BeginVertical();
                                    if (isTop)
                                    {
                                        GridUnusableTile(true);
                                        EditorGUILayout.BeginVertical();
                                        GUILayout.Space(4f);
                                        GridUnusableTile(false);
                                        EditorGUILayout.EndVertical();
                                    }
                                    else
                                    {
                                        EditorGUILayout.BeginVertical();
                                        GUILayout.Space(4f);
                                        GridUnusableTile(false);
                                        EditorGUILayout.EndVertical();
                                        GridUnusableTile(true);
                                    }
                                    EditorGUILayout.EndVertical();
                                    GUILayout.Space(2.25f);
                                }
                                else if ((isTop || isBottom) && isRight)
                                {
                                    EditorGUILayout.BeginVertical();
                                    EditorGUILayout.BeginHorizontal();
                                    if (isTop) 
                                    { 
                                        GridUnusableTile(true); 
                                    }
                                    else 
                                    {
                                        EditorGUILayout.BeginVertical();
                                        GUILayout.Space(4f);
                                        GridUnusableTile(false);
                                        EditorGUILayout.EndVertical();
                                    }
                                    GridUnusableTile(true);    
                                    GUILayout.Space(2.5f);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    if (isTop) 
                                    {
                                        EditorGUILayout.BeginVertical();
                                        GUILayout.Space(4f);
                                        GridUnusableTile(false);
                                        EditorGUILayout.EndVertical();
                                    }
                                    else 
                                    {
                                        GridUnusableTile(true);
                                    }
                                    GridUnusableTile(true);    
                                    GUILayout.Space(2.5f);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.EndVertical();
                                }
                                else
                                {
                                    GridUnusableTile(false);
                                    GUILayout.Space(2.5f);
                                }
                            }

                            if (isBottom) { bottomSpawns.Add(isNotSpawnable ? null : level.waves[level.selectedWave].spawns[spawnIndex]); }
                            EditorGUI.EndDisabledGroup();

                            if (!isNotSpawnable)
                                spawnIndex++;
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    DrawUILine(Color.grey);

                    EditorGUILayout.BeginVertical();
                    LegendPatternColorAndChange(null);
                    foreach (EnemyPattern pattern in patterns) { LegendPatternColorAndChange(pattern); }
                    EditorGUILayout.EndVertical();

                    break;
                }
            default:
                level.tab = 0;
                break;
        }

    }

    public void EnemyTypeButton(Level.EnemySpawn spawn, EnemyPattern[] patterns)
    {
        Color previousColor = GUI.color;
        GUI.color = spawn != null && spawn.pattern != null ? spawn.pattern.patternColor : new Color(0f, 0f, 0f, 1f);

        if (GUILayout.Button("", GUILayout.Width(14.5f), GUILayout.Height(12f)))
        {
            if (spawn != null)
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("None"), spawn.pattern == null, delegate { spawn.pattern = null; });
                menu.AddItem(new GUIContent(""), false, delegate { });
                foreach (EnemyPattern pattern in patterns) { menu.AddItem(new GUIContent(pattern.patternName), spawn.pattern == pattern, delegate { spawn.pattern = pattern; }); }

                menu.ShowAsContext();
            }
        }

        GUI.color = previousColor;
    }
    
    public void LegendPatternColorAndChange(EnemyPattern pattern)
    {
        EditorGUILayout.BeginHorizontal();
        if (pattern)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ColorField(GUIContent.none, pattern.patternColor, false, false, false, GUILayout.Width(16f), GUILayout.Height(16f));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.LabelField(pattern.patternName);
        }
        else
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ColorField(GUIContent.none, new Color(0f, 0f, 0f, 0f), false, false, false, GUILayout.Width(16f), GUILayout.Height(16f));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.LabelField("None");
        }
                EditorGUILayout.EndHorizontal();
    }

    public void GridUnusableTile(bool invisible)
    {
        Color previousColor = GUI.color;
        GUI.color = invisible ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 0.25f);
        EditorGUI.BeginDisabledGroup(true);
        if (GUILayout.Button("", GUILayout.Width(14.5f), GUILayout.Height(12f))) { }
        EditorGUI.EndDisabledGroup();
        GUI.color = previousColor;
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

    private void OnDisable() { EditorUtility.SetDirty(target as Level); }
}
#endif
#endregion