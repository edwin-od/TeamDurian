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
    public int currentWave = 1;

    void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        //StartLevel(0);
        StartMusicLevel(0);
        StartLevel(0);
        Tempo.Instance.StartTempo();
    }

    public void StartLevel(int levelIndex)
    {
        if (GridManager.Instance)
        {
            for (int i = 0; i < levels.levels[levelIndex].waves.Length; i++)
            {
                for (int j = 0; j < levels.levels[levelIndex].waves[i].lines.Count; j++)
                {
                    int startingTile = (GridManager.Instance.Grid.tiles.x - levels.levels[levelIndex].waves[i].lines[j].lineLength) / 2;
                    for (int k = 0; k < levels.levels[levelIndex].waves[i].lines[j].lineLength; k++)
                    {
                        GridManager.IntVector2 spawnTile = new GridManager.IntVector2(startingTile + k, GridManager.Instance.Grid.tiles.y - j - 1);
                        GameObject enemy = Instantiate(enemyPrefab, transform);
                        enemy.transform.parent = transform;
                        enemy.GetComponent<EnemyController>().TeleportOnGrid(spawnTile);
                    }
                }
            }
        }
    }

    public void StartMusicLevel(int levelIndex)
    {
        Tempo.Instance.BPM = levels.levels[levelIndex].waves[currentWave - 1].soundLoop.BPM;

        foreach (SoundLoop.Loop loop in levels.levels[levelIndex].waves[currentWave - 1].soundLoop.loops)
        {
            Debug.Log("loop : " + loop.loop.name);
            audioSource.PlayOneShot(loop.loop);
        }

        StartCoroutine(CallbackEndLoop(levels.levels[levelIndex].waves[currentWave - 1].soundLoop.loops[0].loop.length,
        delegate
        {
            StartMusicLevel(levelIndex);
        }));
    }

    IEnumerator CallbackEndLoop(float length, System.Action action)
    {
        yield return new WaitForSeconds(length);

        action.Invoke();
    }
}
