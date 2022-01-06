using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundLevel")]
public class SoundLevel : ScriptableObject
{
    
    [System.Serializable]
    public class Loop
    {
        public string loopName = "Loop 01";
        public SoundLoop loop;
        [Range(1, 1000)] public int loopCount = 1;
    }

    [System.Serializable]
    public class Introduction
    {
        public SoundLoop soundLoop;
        [Range(1, 1000)] public int loopCount = 1;
    }

    [System.Serializable]
    public class Outro
    {
        public SoundLoop soundLoop;
        [Range(1, 1000)] public int loopCount = 1;
    }

    public SoundLoop.Instrument backgroundMusic;
    [Range(0, 120)] public int initialDelayTempo = 0;
    public Introduction intro;
    public List<Loop> soundLoops = new List<Loop>();
    public Outro outro;

}
