using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels")]
public class Levels : ScriptableObject
{
    public List<Level> levels = new List<Level>();

    [System.Serializable]
    public class EnemyPatternLevels
    {
        public Level.Pattern type = Level.Pattern.Down_1b;
        public EnemyPattern enemyPattern = null;
    }

    public List<EnemyPatternLevels> enemyMovementPatterns = new List<EnemyPatternLevels>();

    public EnemyPattern GetPattern(Level.Pattern type)
    {
        foreach (EnemyPatternLevels pat in enemyMovementPatterns)
        {
            if (pat.type == type)
                return pat.enemyPattern;
        }
        return null;
    }
    
}
