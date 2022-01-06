using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundLevel")]
public class SoundLevels : ScriptableObject
{
    [System.Serializable]
    public class Level
    {
        public string levelName = "";
        public SoundLevel level;
    }

    public List<Level> soundLevels = new List<Level>();
}
