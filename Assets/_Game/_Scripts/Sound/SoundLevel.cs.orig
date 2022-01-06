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

    public SoundLoop.Instrument backgroundMusic;
    [Range(0f, 120)] public int initialDelayTempo = 0;
    public List<Loop> soundLoops = new List<Loop>();

}
