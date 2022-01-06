using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "SoundLoop")]
public class SoundLoop : ScriptableObject
{
    [System.Serializable]
    public class Loop
    {
        public AudioClip loop;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [SerializeField, Range(30, 300)] public int BPM = 130;
    public List<Loop> loops = new List<Loop>();
}
