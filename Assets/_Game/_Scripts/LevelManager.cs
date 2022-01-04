using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Levels levels;
    public GameObject enemyPrefab;

    [HideInInspector] public static LevelManager Get;
    void Awake()
    {
        if (Get == null) Get = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        StartLevel(0);
    }

    public void StartLevel(int levelIndex)
    {
        if (GridManager.Get)
        {
            for (int i = 0; i < levels.levels[levelIndex].waves.Length; i++)
            {
                for (int j = 0; j < levels.levels[levelIndex].waves[i].lines.Count; j++)
                {
                    int startingTile = (GridManager.Get.Grid.tiles.x - levels.levels[levelIndex].waves[i].lines[j].lineLength) / 2;
                    for (int k = 0; k < levels.levels[levelIndex].waves[i].lines[j].lineLength; k++)
                    {
                        GridManager.IntVector2 spawnTile = new GridManager.IntVector2(startingTile + k, GridManager.Get.Grid.tiles.y - j - 1);
                        GameObject enemy = Instantiate(enemyPrefab, transform);
                        enemy.transform.parent = transform;
                        enemy.GetComponent<EnemyController>().TeleportOnGrid(spawnTile);
                    }
                }
            }
        }
    }
}
