using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Levels levels;
    public GameObject enemyPrefab;

    public AudioSource audioSource;

    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

    public bool changeWave;
    public int currentWave = 0;
    public float initialDelay = 0.1f;
    public int levelIndex = 0;

    public List<EnemyController> enemies = new List<EnemyController>();

    public void RegisterEnemy(EnemyController nmi)
    {
        enemies.Add(nmi);
    }

    public void UnregisterEnemy(EnemyController nmi)
    {
        enemies.Remove(nmi);
    }

    void Awake()
    {
        _instance = this;
    }

    IEnumerator Start()
    {
        StartMusicLevel();

        yield return new WaitForSecondsRealtime(initialDelay);
        Tempo.Instance.StartTempo();
    }

    public void StartLevel()
    {
        if (GridManager.Instance)
        {
            for (int j = 0; j < levels.levels[levelIndex].waves[currentWave - 1].lines.Count; j++)
            {
                int startingTile = (GridManager.Instance.Grid.tiles.x - levels.levels[levelIndex].waves[currentWave - 1].lines[j].lineLength) / 2;
                for (int k = 0; k < levels.levels[levelIndex].waves[currentWave - 1].lines[j].lineLength; k++)
                {
                    GridManager.IntVector2 spawnTile = new GridManager.IntVector2(startingTile + k, GridManager.Instance.Grid.tiles.y - j - 1);
                    GameObject enemy = Instantiate(enemyPrefab, transform);
                    enemy.transform.parent = transform;
                    enemy.GetComponent<EnemyController>().TeleportOnGrid(spawnTile);
                }
            }
        }
    }

    public void StartMusicLevel()
    {
        if(enemies.Count <= 0)
        {
            currentWave++;
            StartLevel();
            Debug.Log("Next wave..");

            if (levels.levels[levelIndex].waves.Length < currentWave)
            {
                Debug.Log("FINISH");
                return;
            }
        }

        Tempo.Instance.BPM = levels.levels[levelIndex].waves[currentWave - 1].soundLoop.BPM;

        foreach (SoundLoop.Loop loop in levels.levels[levelIndex].waves[currentWave - 1].soundLoop.loops)
        {
            Debug.Log("loop : " + loop.loop.name);
            audioSource.PlayOneShot(loop.loop);
        }

        StartCoroutine(CallbackEndLoop(levels.levels[levelIndex].waves[currentWave - 1].soundLoop.loops[0].loop.length,
        delegate
        {
            StartMusicLevel();
        }));
    }

    IEnumerator CallbackEndLoop(float length, System.Action action)
    {
        yield return new WaitForSeconds(length);

        action.Invoke();
    }
}
