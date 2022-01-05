using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [System.Serializable] public struct IntVector2 { public int x, y; public IntVector2(int x, int y) { this.x = x; this.y = y; } };
    [System.Serializable]  public struct GridXY { public IntVector2 tiles; public Vector2 tileSize; };
    [SerializeField] private GridXY grid;
    [SerializeField] private GameObject gridAxisPrefab;

    public GridXY Grid { get { return grid; } }

    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        if (Player.Instance)
            Player.Instance.TeleportOnGrid(new IntVector2(grid.tiles.x / 2, grid.tiles.y / 2));

        if (gridAxisPrefab)
        {
            for (int x = 0; x <= grid.tiles.x; x++)
            {
                GameObject axis = Instantiate(gridAxisPrefab, transform);
                axis.transform.position += new Vector3(grid.tileSize.x * x, 0, 0);
                axis.transform.localScale = new Vector3(axis.transform.localScale.x, axis.transform.localScale.y, grid.tileSize.y * grid.tiles.y);
                axis.transform.parent = transform;
                axis.name = "X (" + x + ")";
            }
            for (int y = 0; y <= grid.tiles.y; y++)
            {
                GameObject axis = Instantiate(gridAxisPrefab, transform);
                axis.transform.position += new Vector3(0, 0, grid.tileSize.y * y);
                axis.transform.rotation = transform.rotation * Quaternion.Euler(0f, 90f, 0f);
                axis.transform.localScale = new Vector3(axis.transform.localScale.x, axis.transform.localScale.y, grid.tileSize.x * grid.tiles.x);
                axis.transform.parent = transform;
                axis.name = "Y (" + y + ")";
            }
        }

        if(Camera.main)
            Camera.main.transform.position = new Vector3((grid.tiles.x * grid.tileSize.x) / 2, Camera.main.transform.position.y, (grid.tiles.y * grid.tileSize.y) / 2);
    }
}
