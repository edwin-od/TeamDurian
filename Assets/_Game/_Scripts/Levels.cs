using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class Levels : ScriptableObject
{
    [System.Serializable]
    public class Level
    {
        public string levelName = "1";
        public Wave[] waves;
    }

    public List<Level> levels = new List<Level>();
}
