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

    public AudioClip backgroundMusic;
    [Range(0f, 120f)] public float initialDelayTempo = 0f;
    public List<Loop> soundLoops = new List<Loop>();

}
