using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave")]
public class Wave : ScriptableObject
{
    public enum EnemyType { Default };

    public string waveName = "0";
    
    [System.Serializable]
    public class Line
    {
        public int lineNumber;
        public int lineLength;
        public EnemyType enemyType;
    }

    public List<Line> lines = new List<Line>();

    public SoundLoop soundLoop;

    private void OnValidate()
    {
        for(int i = 0; i < lines.Count; i++)
        {
            lines[i].lineNumber = i;

            if (lines[i].lineLength % 2 == 0)
                lines[i].lineLength++;
        }
    }
}
