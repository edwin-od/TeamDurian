using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Levels levels;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField, Range(0f, 5f)] private float initialTempoDelay = 0.1f;

    private int waveIndex = 0;
    private int levelIndex = 0;

    private List<EnemyController> enemies = new List<EnemyController>();
    private List<AudioSource> clips = new List<AudioSource>();

    public delegate void LevelStarted();
    public static event LevelStarted OnLevelStarted;

    public delegate void LevelEnded();
    public static event LevelEnded OnLevelEnded;

    public void RegisterEnemy(EnemyController enemy) { enemies.Add(enemy); }

    public void UnregisterEnemy(EnemyController enemy) { enemies.Remove(enemy); }

    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }
    void Awake() { _instance = this; }

    void Start() { StartLevel(0); }

    private void StartLevel(int level)
    {
        levelIndex = level;
        waveIndex = 0;

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

        EmptyClips();
        FillClips();
        SpawnEnemies();
        StartLoop();
        Tempo.Instance.StartTempo(levels.levels[levelIndex].waves[waveIndex].loop.BPM, initialTempoDelay) ;
    }

    private void StartLoop()
    {
        if(levels.levels[levelIndex].waves[waveIndex].loop.loops.Count > 0)
            StartCoroutine(PlayLoop(levels.levels[levelIndex].waves[waveIndex].loop.loops[0].loop.length));
    }

    IEnumerator PlayLoop(float length)
    {
        foreach (AudioSource clip in clips) { clip.Play(); }

        bool nextWave = false;
        float tx = Time.realtimeSinceStartup;
        float tpause = 0;
        float elapsedTime = 0f;
        while (elapsedTime < length)
        {
            if (Tempo.Instance && !Tempo.Instance.IsTempoPaused)
            {
                if (enemies.Count == 0) { nextWave = true; elapsedTime = length; }

                float deltaTime = 0f;

                // Manage Pause Compensation
                if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; UnPauseLoop(); }
                else { deltaTime = Time.realtimeSinceStartup - tx; }

                tx = Time.realtimeSinceStartup;
                elapsedTime += deltaTime;
            }
            else if (Tempo.Instance && Tempo.Instance.IsTempoPaused && tpause == 0) { tpause = Time.realtimeSinceStartup; PauseLoop(); }

            yield return null;
        }

        if (nextWave) { NextWave(); }
        else { StartLoop(); }
    }

    private void FillClips()
    {
        foreach (Loop.LoopClip loop in levels.levels[levelIndex].waves[waveIndex].loop.loops)
        {
            clips.Add(gameObject.AddComponent<AudioSource>());
            clips[clips.Count - 1].clip = loop.loop;
            clips[clips.Count - 1].volume = loop.volume;
            clips[clips.Count - 1].playOnAwake = false;
            clips[clips.Count - 1].loop = false;
        }
    }

    private void NextWave()
    {
        foreach (AudioSource clip in clips) { clip.Stop(); }

        waveIndex++;
        if (waveIndex >= levels.levels[levelIndex].waves.Count)
        {
            Tempo.Instance.StopTempo();
            OnLevelEnded?.Invoke();
            return;
        }

        Tempo.Instance.BPM = levels.levels[levelIndex].waves[waveIndex].loop.BPM;
        EmptyClips();
        FillClips();
        SpawnEnemies();
        StartLoop();
    }

    private void EmptyClips()
    {
        foreach(AudioSource clip in clips) { Destroy(clip); }
        clips = new List<AudioSource>();
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
                if (x == 0 || x == levels.levels[levelIndex].gridWidth - 1 || y == 0 || y == levels.levels[levelIndex].gridHeight - 1)
                {
                    if (levels.levels[levelIndex].waves[waveIndex].spawns[spawnIndex])
                    {
                        GridManager.IntVector2 spawnTile = new GridManager.IntVector2(x, levels.levels[levelIndex].gridHeight - y - 1);
                        GameObject enemy = Instantiate(enemyPrefab, transform);
                        enemy.GetComponent<EnemyController>().movementPattern = null;
                        enemy.transform.parent = transform;
                        enemy.GetComponent<EnemyController>().TeleportOnGrid(spawnTile);
                    }

                    spawnIndex++;
                }
            }
        }
    }
}