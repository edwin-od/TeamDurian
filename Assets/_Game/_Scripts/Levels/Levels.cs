using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels")]
public class Levels : ScriptableObject
{
    public List<Level> levels = new List<Level>();
}
