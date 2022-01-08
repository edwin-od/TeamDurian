using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels")]
public class Levels : ScriptableObject
{
    public List<Level> levels = new List<Level>();

    private void OnValidate()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].levelName = "Lave " + (i + 1);
        }
    }
}
