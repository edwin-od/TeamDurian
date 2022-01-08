using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "Loop")]
public class Loop : ScriptableObject
{
    [System.Serializable]
    public class LoopClip
    {
        public AudioClip loop;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [SerializeField, Range(30, 300)] public int BPM = 130;
    public List<LoopClip> loops = new List<LoopClip>();
}
