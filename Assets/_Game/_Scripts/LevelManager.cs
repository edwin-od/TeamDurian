using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Levels levels;
    public int highestTile = 6;
    public int gridLength = 13;
    public GameObject enemyPrefab;

    private void Start()
    {
        StartLevel(0);
    }

    public void StartLevel(int levelIndex)
    {
        for (int i = 0; i < levels.levels[levelIndex].waves.Length; i++)
        {
            for(int j = 0; j < levels.levels[levelIndex].waves[i].lines.Count; j++)
            {
                int startingTile = gridLength - levels.levels[levelIndex].waves[i].lines[j].lineLength;
                startingTile = (startingTile - gridLength) / 2;

                for (int k = 0; k < levels.levels[levelIndex].waves[i].lines[j].lineLength; k++)
                {
                    Instantiate(enemyPrefab, new Vector3(startingTile + k, 0, highestTile - j), Quaternion.identity);
                }
            }
        }
    }
}
