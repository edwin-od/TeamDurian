using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName =("Enemy Pattern"))]
public class EnemyPattern : ScriptableObject
{
    public string patternName;
    public Color patternColor = new Color(0f, 0f, 0f, 0f);
    public enum BeatEvery { OneBeat = 1, TwoBeat = 2, ThreeBeat = 3, FourthBeat = 4 };
    public BeatEvery beatEvery = BeatEvery.OneBeat;

    public List<GridMoveable.DIRECTION> directions;

#if UNITY_EDITOR
    public const int VAR_SPACE = 6;

    void Awake() 
    {
        if (patternName == null || patternName == "")
            patternName = "New Pattern";

        if (patternColor == new Color(0f, 0f, 0f, 0f)) { patternColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f); } 
    }

    public static EnemyPattern[] GetPatterns() { return Resources.FindObjectsOfTypeAll<EnemyPattern>(); }
#endif
}
#region Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(EnemyPattern))]
public class EnemyPatternEditor : Editor
{
    override public void OnInspectorGUI()
    {
        EnemyPattern pattern = target as EnemyPattern;
        SerializedObject serializedPattern = new SerializedObject(target);

        GUILayout.Space(EnemyPattern.VAR_SPACE);
        GUILayout.Space(EnemyPattern.VAR_SPACE);
        GUILayout.Space(EnemyPattern.VAR_SPACE);

        EditorGUI.BeginChangeCheck();
        string patternName = pattern.patternName;
        pattern.patternName = EditorGUILayout.TextField("Pattern Name", pattern.patternName);
        if (pattern.patternName == null || pattern.patternName == "")
            pattern.patternName = patternName != null && patternName != "" ? patternName : "New Pattern";

        GUILayout.Space(EnemyPattern.VAR_SPACE);
        pattern.patternColor = EditorGUILayout.ColorField(new GUIContent("Pattern Color"), pattern.patternColor, true, true, false);

        DrawUILine(Color.grey);

        pattern.beatEvery = (EnemyPattern.BeatEvery)EditorGUILayout.EnumPopup("Move Every", pattern.beatEvery);

        GUILayout.Space(EnemyPattern.VAR_SPACE);

        serializedPattern.Update();
        EditorGUILayout.PropertyField(serializedPattern.FindProperty("directions"), true);
        serializedPattern.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck()) { AssetDatabase.SaveAssets(); }
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        GUILayout.Space(EnemyPattern.VAR_SPACE);
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
        GUILayout.Space(EnemyPattern.VAR_SPACE);
    }

    private void OnDisable() { EditorUtility.SetDirty(target as EnemyPattern); }
}
#endif
#endregion