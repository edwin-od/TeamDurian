using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "SoundLoop")]
public class SoundLoop : ScriptableObject
{
    [SerializeField, Range(1, 500)] public int BPM = 130;
    public List<AudioClip> instruments = new List<AudioClip>();
}
