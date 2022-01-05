using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : TempoTrigger
{
    [SerializeField] private SoundLevels soundLevels;

    private int currentLevel = -1;
    private int currentLoop = -1;
    private int loopCount = -1;
    private bool isLevelActive = false;
    private List<AudioSource> instruments;

    public delegate void LevelStart();
    public static event LevelStart OnLevelStart;

    public delegate void LevelEnd();
    public static event LevelEnd OnLevelEnd;

    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        StartLevel(0);
    }

    public override void Beat() 
    {
        if(isLevelActive)
        {
            if(loopCount >= 0)
                PlayInstruments();
            else
            {
                currentLoop++;
                if (currentLoop >= soundLevels.soundLevels[currentLevel].level.soundLoops.Count) { StopLevel(); OnLevelEnd?.Invoke(); return; }

                loopCount = soundLevels.soundLevels[currentLevel].level.soundLoops[currentLoop].loopCount - 1;

                if(Tempo.Instance)
                    Tempo.Instance.BPM = soundLevels.soundLevels[currentLevel].level.soundLoops[currentLoop].loop.BPM;

                RemoveAllAudioSources();
                for (int i = 0; i < soundLevels.soundLevels[currentLevel].level.soundLoops[currentLoop].loop.instruments.Count; i++)
                {
                    AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = soundLevels.soundLevels[currentLevel].level.soundLoops[currentLoop].loop.instruments[i];
                    audioSource.playOnAwake = false;
                    instruments.Add(audioSource);
                }

                PlayInstruments();
            }
        }
    }

    public void StartLevel(int levelIndex)
    {
        if(levelIndex < 0 || levelIndex >= soundLevels.soundLevels.Count) { StopLevel(); return; }

        currentLevel = levelIndex;
        currentLoop = 0;
        loopCount = soundLevels.soundLevels[levelIndex].level.soundLoops[0].loopCount - 1;

        instruments = new List<AudioSource>();
        for (int i = 0; i < soundLevels.soundLevels[currentLevel].level.soundLoops[0].loop.instruments.Count; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = soundLevels.soundLevels[currentLevel].level.soundLoops[0].loop.instruments[i];
            audioSource.playOnAwake = false;
            instruments.Add(audioSource);
        }

        Tempo.Instance?.StartTempo(soundLevels.soundLevels[currentLevel].level.soundLoops[0].loop.BPM, soundLevels.soundLevels[currentLevel].level.initialDelayTempo);
        isLevelActive = true;
        OnLevelStart?.Invoke();
    }

    public void StopLevel()
    {
        Tempo.Instance?.StopTempo();
        isLevelActive = false;
        currentLevel = -1;
        currentLoop = -1;
        loopCount = -1;
        RemoveAllAudioSources();
    }

    private void RemoveAllAudioSources()
    {
        foreach (AudioSource instrument in instruments) { Destroy(instrument); }
        instruments = new List<AudioSource>();
    }

    private void PlayInstruments()
    {
        for (int i = 0; i < instruments.Count; i++)
        {
            instruments[i].Play();
        }

        loopCount--;
    }
}
