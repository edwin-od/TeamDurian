using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("Enemy Pattern"))]
public class EnemyPattern : ScriptableObject
{
    public string patternName;
    public enum BeatEvery { OneBeat = 1, TwoBeat = 2, ThreeBeat = 3, FourthBeat = 4 };
    public BeatEvery beatEvery;

    public GridMoveable.DIRECTION[] directions;
}
