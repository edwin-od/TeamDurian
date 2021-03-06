using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Levels levels;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Range(0f, 5f)] private float dropDelay = 0f;

    public GameObject winScreen;
    public bool isWin = false;

    private int waveIndex = 0;
    private int levelIndex = 0;

    private List<EnemyController> enemies = new List<EnemyController>();
    private List<AudioSource> clips = new List<AudioSource>();
    private AudioSource transitionOut = null;
    private AudioSource transitionIn = null;
    private AudioSource dropDelaySource = null;

    public delegate void LevelStarted();
    public static event LevelStarted OnLevelStarted;

    public delegate void LevelEnded();
    public static event LevelEnded OnLevelEnded;

    private Coroutine loopCoroutine = null;

    public void RegisterEnemy(EnemyController enemy) { enemies.Add(enemy); }

    public void UnregisterEnemy(EnemyController enemy) { enemies.Remove(enemy); }

    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }
    void Awake()  {  _instance = this; }

    void Start() { if (levels.levels.Count > 0) { StartLevel(SceneSurvivor.Instance && SceneSurvivor.Instance.level >= 0 && SceneSurvivor.Instance.level < levels.levels.Count ? SceneSurvivor.Instance.level : 0); } }

    private void StartLevel(int level)
    {
        if(Tempo.Instance)
        {
            winScreen.SetActive(false);
            isWin = false;
            if (PlayerController.Instance) { PlayerController.Instance.RestartPlayer(); }

            levelIndex = level;

            if (levels && levelIndex >= 0 && levelIndex < levels.levels.Count)
            {
                OnLevelStarted?.Invoke();

                if (GridManager.Instance)
                {
                    GridManager.Instance.StartLevel(
                        levels.levels[levelIndex].gridWidth,
                        levels.levels[levelIndex].gridHeight,
                        levels.levels[levelIndex].gridTileSizeX,
                        levels.levels[levelIndex].gridTileSizeY,
                        levels.levels[levelIndex].cameraHeight,
                        levels.levels[levelIndex].tileColor1,
                        levels.levels[levelIndex].tileColor2
                        );
                }

                waveIndex = -1;
                NextWave();
            }
        }
    }

    public void StopLevel()
    {
        EmptyClips();
        foreach (EnemyController enemy in enemies) { Destroy(enemy.gameObject); }
        enemies = new List<EnemyController>();
        StopLevelLoop();
    }

    public void RestartCurrentLevel()
    {
        StopLevel();
        StartLevel(levelIndex);
    }

    private void StartPassiveLoop()
    {
        if (loopCoroutine == null && levels.levels[levelIndex].waves[waveIndex].loop.loops.Count > 0) { loopCoroutine = StartCoroutine(PlayPassiveWave()); }
    }

    private void StartLoop()
    {
        if (loopCoroutine == null && levels.levels[levelIndex].waves[waveIndex].loop.loops.Count > 0) { loopCoroutine = StartCoroutine(PlayLoop()); }
    }

    private void StopLoop()
    {
        if (loopCoroutine != null) { StopCoroutine(loopCoroutine); loopCoroutine = null;  }
    }

    IEnumerator PlayLoop()
    {
        if (clips.Count == 0) { NextWave(); }

        float length = 0;
        foreach (Loop.LoopClip loop in levels.levels[levelIndex].waves[waveIndex].loop.loops) { if (length < loop.clip.length) { length = loop.clip.length; } }

        foreach (AudioSource clip in clips) { clip.Play(); }

        float transitionInLength = 0f;
        if (transitionIn && transitionIn.clip) { transitionInLength = transitionIn.clip.length; transitionIn.Play(); }

        float transOutBeatOffset = 0;
        if(transitionOut && transitionOut.clip)
        {
            float transOutBeats = levels.levels[levelIndex].waves[waveIndex].loop.transitionOut.clip.length / Tempo.Instance.TempoPeriod;
            transOutBeatOffset = Tempo.Instance.TempoPeriod * (1 - (transOutBeats - Mathf.Floor(transOutBeats)));
        }
        bool startedTransition = false;
        float tx = Time.realtimeSinceStartup;
        float tpause = 0;
        float elapsedTime = 0f;
        if (Tempo.Instance)
        { 
            while (true)
            {
                if (!Tempo.Instance.IsTempoPaused)
                {
                    if (enemies.Count == 0 && !startedTransition)
                    {
                        float currentBeat = elapsedTime / Tempo.Instance.TempoPeriod;
                        float beatOffset = currentBeat - Mathf.Floor(currentBeat);

                        if (beatOffset >= transOutBeatOffset - 0.1f) { startedTransition = true; StartCoroutine(PlayTransitionOut(dropDelay)); }
                    }

                    float deltaTime = 0f;

                    if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; UnPauseLoop(); }
                    else { deltaTime = Time.realtimeSinceStartup - tx; }

                    tx = Time.realtimeSinceStartup;

                    elapsedTime += deltaTime;
                    if(elapsedTime >= length) { foreach (AudioSource clip in clips) { clip.Play(); } elapsedTime = 0f; }

                    if (transitionIn && elapsedTime >= transitionInLength) { transitionIn.Stop(); }
                }
                else if (Tempo.Instance.IsTempoPaused && tpause == 0) { tpause = Time.realtimeSinceStartup; PauseLoop(); }

                yield return null;
            }
        }
    }

    IEnumerator PlayTransitionOut(float dropDelay)
    {
        if (transitionOut && transitionOut.clip)
        {
            float length = levels.levels[levelIndex].waves[waveIndex].loop.transitionOut.clip.length;

            transitionOut.Play();

            float tx = Time.realtimeSinceStartup;
            float tpause = 0;
            float elapsedTime = 0f;
            if (Tempo.Instance)
            {
                while (elapsedTime < length)
                {
                    if (!Tempo.Instance.IsTempoPaused)
                    {
                        float deltaTime = 0f;

                        if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; transitionOut.UnPause(); }
                        else { deltaTime = Time.realtimeSinceStartup - tx; }

                        tx = Time.realtimeSinceStartup;
                        elapsedTime += deltaTime;
                    }
                    else if (Tempo.Instance.IsTempoPaused && tpause == 0) { tpause = Time.realtimeSinceStartup; transitionOut.Pause(); }

                    yield return null;
                }
            }
            transitionOut.Stop();
        }

        foreach (AudioSource clip in clips) { clip.Stop(); }
        if (dropDelaySource && dropDelaySource.clip) { dropDelaySource.Play(); Tempo.Instance.CanGenerateBeatEvents = false; yield return new WaitForSeconds(dropDelaySource.clip.length); Tempo.Instance.CanGenerateBeatEvents = true; }
        else { Tempo.Instance.CanGenerateBeatEvents = false; yield return new WaitForSeconds(dropDelay); Tempo.Instance.CanGenerateBeatEvents = true; }
        NextWave();
    }

    IEnumerator PlayPassiveWave()
    {
        if (clips.Count == 0) { NextWave(); }

        float length = 0;
        foreach (Loop.LoopClip loop in levels.levels[levelIndex].waves[waveIndex].loop.loops) { if (length < loop.clip.length) { length = loop.clip.length; } }

        foreach (AudioSource clip in clips) { clip.Play(); }

        float tx = Time.realtimeSinceStartup;
        float tpause = 0;
        float elapsedTime = 0f;
        if (Tempo.Instance)
        {
            Tempo.Instance.CanGenerateBeatEvents = false;
            while (elapsedTime < length)
            {
                if (!Tempo.Instance.IsTempoPaused)
                {
                    float deltaTime = 0f;

                    if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; UnPauseLoop(); }
                    else { deltaTime = Time.realtimeSinceStartup - tx; }

                    tx = Time.realtimeSinceStartup;
                    elapsedTime += deltaTime;
                }
                else if (Tempo.Instance.IsTempoPaused && tpause == 0) { tpause = Time.realtimeSinceStartup; PauseLoop(); }

                yield return null;
            }
            Tempo.Instance.CanGenerateBeatEvents = true;
        }

        foreach (AudioSource clip in clips) { clip.Stop(); }
        NextWave();
    }

    private void FillClips()
    {
        foreach (Loop.LoopClip loop in levels.levels[levelIndex].waves[waveIndex].loop.loops)
        {
            if (loop.clip != null)
            {
                clips.Add(gameObject.AddComponent<AudioSource>());
                clips[clips.Count - 1].clip = loop.clip;
                clips[clips.Count - 1].volume = loop.volume;
                clips[clips.Count - 1].playOnAwake = false;
                clips[clips.Count - 1].loop = false;
            }
        }
        if (levels.levels[levelIndex].waves[waveIndex].loop.transitionOut.clip != null) 
        { 
            transitionOut = gameObject.AddComponent<AudioSource>();
            transitionOut.clip = levels.levels[levelIndex].waves[waveIndex].loop.transitionOut.clip;
            transitionOut.volume = levels.levels[levelIndex].waves[waveIndex].loop.transitionOut.volume;
            transitionOut.playOnAwake = false;
            transitionOut.loop = false;
        }
        if (levels.levels[levelIndex].waves[waveIndex].loop.transitionIn.clip != null)
        {
            transitionIn = gameObject.AddComponent<AudioSource>();
            transitionIn.clip = levels.levels[levelIndex].waves[waveIndex].loop.transitionIn.clip;
            transitionIn.volume = levels.levels[levelIndex].waves[waveIndex].loop.transitionIn.volume;
            transitionIn.playOnAwake = false;
            transitionIn.loop = false;
        }
        if (levels.levels[levelIndex].waves[waveIndex].loop.dropDelay.clip != null)
        {
            dropDelaySource = gameObject.AddComponent<AudioSource>();
            dropDelaySource.clip = levels.levels[levelIndex].waves[waveIndex].loop.dropDelay.clip;
            dropDelaySource.volume = levels.levels[levelIndex].waves[waveIndex].loop.dropDelay.volume;
            dropDelaySource.playOnAwake = false;
            dropDelaySource.loop = false;
        }
    }

    private void NextWave()
    {
        StopLoop();

        waveIndex++;
        if (waveIndex >= levels.levels[levelIndex].waves.Count)
        {
            Debug.Log("Level named \"" + levels.levels[levelIndex].levelName + "\" has ended.");

            winScreen.SetActive(true);
            isWin = true;
            StopLevelLoop();
            OnLevelEnded?.Invoke();
            return;
        }

        Tempo.Instance.StopTempo();
        Tempo.Instance.StartTempo(levels.levels[levelIndex].waves[waveIndex].loop.BPM, Mathf.Abs(levels.levels[levelIndex].waves[waveIndex].loop.beatDelay));

        EmptyClips();
        FillClips();

        if (!levels.levels[levelIndex].waves[waveIndex].isPassive) { SpawnEnemies(); StartLoop(); }
        else { StopLoop(); StartPassiveLoop();  }
        
    }

    private void EmptyClips()
    {
        foreach(AudioSource clip in clips) { Destroy(clip); }
        clips = new List<AudioSource>();
        if (transitionOut) { Destroy(transitionOut); transitionOut = null; }
        if (transitionIn) { Destroy(transitionIn); transitionOut = null; }
        if (dropDelaySource) { Destroy(dropDelaySource); dropDelaySource = null; }
    }

    private void PauseLoop()
    {
        foreach (AudioSource clip in clips) { clip.Pause(); }
    }

    private void UnPauseLoop()
    {
        foreach (AudioSource clip in clips) { clip.UnPause(); }
    }

    private void SpawnEnemies()
    {
        int spawnIndex = 0;
        for (int y = 0; y < levels.levels[levelIndex].gridHeight; y++)
        {
            for (int x = 0; x < levels.levels[levelIndex].gridWidth; x++)
            {
                bool isNotSpawnable = (x != 0 && x != levels.levels[levelIndex].gridWidth - 1 && y != 0 && y != levels.levels[levelIndex].gridHeight - 1) ||
                                        (x == 0 && y == 0) ||
                                        (x == 0 && y == levels.levels[levelIndex].gridHeight - 1) ||
                                        (x == levels.levels[levelIndex].gridWidth - 1 && y == 0) ||
                                        (x == levels.levels[levelIndex].gridWidth - 1 && y == levels.levels[levelIndex].gridHeight - 1);

                if (!isNotSpawnable)
                {
                    if (levels.levels[levelIndex].waves[waveIndex].spawns[spawnIndex].spawn)
                    {
                        GridManager.IntVector2 spawnTile = new GridManager.IntVector2(x, levels.levels[levelIndex].gridHeight - y - 1);
                        GameObject enemy = Instantiate(enemyPrefab, transform);
                        enemy.transform.parent = transform;
                        enemy.GetComponent<EnemyController>().movementPattern = levels.levels[levelIndex].waves[waveIndex].spawns[spawnIndex].pattern;
                        enemy.GetComponent<EnemyController>().TeleportOnGrid(spawnTile);
                    }

                    spawnIndex++;
                }
            }
        }
    }

    private void StopLevelLoop()
    {
        StopLoop();
        Tempo.Instance.StopTempo();
    }
}
