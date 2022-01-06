using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "SoundLoop")]
public class SoundLoop : ScriptableObject
{
    [System.Serializable]
    public class Instrument
    {
        public AudioClip instrument;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [SerializeField, Range(1, 500)] public int BPM = 130;
    public List<Instrument> instruments = new List<Instrument>();
}
