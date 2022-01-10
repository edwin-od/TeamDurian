using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : TempoTrigger
{
    [System.Serializable] public struct IntVector2 { public int x, y; public IntVector2(int x, int y) { this.x = x; this.y = y; } };
    [System.Serializable]  public struct GridXY { public IntVector2 tiles; public Vector2 tileSize; };
    [SerializeField] private GridXY grid;
    [SerializeField] private GameObject gridTilePrefab;
    [SerializeField] private Color tileColor1 = Color.white;
    [SerializeField] private Color tileColor2 = Color.black;

    public GridXY Grid { get { return grid; } }

    private List<List<GameObject>> tiles;
    private bool oddBeat = false;

    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Tab))
        //    StartLevel(grid.tiles.x, grid.tiles.y, grid.tileSize.x, grid.tileSize.y, 3f, tileColor1, tileColor2);
    }

    public override void Beat()
    {
        if (tiles != null)
        {
            for (int y = 0; y < grid.tiles.y; y++)
            {
                for (int x = 0; x < grid.tiles.x; x++)
                {
                    if (tiles[y][x])
                    {
                        if(oddBeat)
                            tiles[y][x].GetComponentInChildren<MeshRenderer>().material.color = x % 2 == y % 2 ? tileColor1 : tileColor2;
                        else
                            tiles[y][x].GetComponentInChildren<MeshRenderer>().material.color = x % 2 == y % 2 ? tileColor2 : tileColor1;
                    }
                }
            }
            oddBeat = !oddBeat;
        }
    }

    public void StartLevel(int tilesWidth, int tilesHeight, float tileSizeX, float tileSizeY, float cameraHeight, Color tileColor1, Color tileColor2)
    {
        EndLevel();

        grid.tiles = new IntVector2(tilesWidth, tilesHeight);
        grid.tileSize = new Vector2(tileSizeX, tileSizeY);

        this.tileColor1 = tileColor1;
        this.tileColor2 = tileColor2;

        if (PlayerController.Instance)
        {
            PlayerController.Instance.TeleportOnGrid(new IntVector2(tilesWidth / 2, 0));
            PlayerController.Instance.transform.localScale = new Vector3(
                PlayerController.Instance.transform.localScale.x * tileSizeX, 
                PlayerController.Instance.transform.localScale.y,
                PlayerController.Instance.transform.localScale.z * tileSizeY);
        }

        if (gridTilePrefab)
        {
            tiles = new List<List<GameObject>>();

            for(int y = 0; y < tilesHeight; y++)
            {
                tiles.Add(new List<GameObject>());
                for(int x = 0; x < tilesWidth; x++)
                {
                    tiles[y].Add(Instantiate(gridTilePrefab, transform));
                    tiles[y][x].transform.position += new Vector3(tileSizeX * x, 0, tileSizeY * y);
                    tiles[y][x].transform.localScale = new Vector3(tiles[y][x].transform.localScale.x * tileSizeX, tiles[y][x].transform.localScale.y, tiles[y][x].transform.localScale.z * tileSizeY);
                    tiles[y][x].transform.parent = transform;
                    tiles[y][x].GetComponentInChildren<MeshRenderer>().material.color = (x % 2 == y % 2 ? tileColor1 : tileColor2);
                    tiles[y][x].name = "(" + x + ", " + y + ")";
                }
            }
        }

        if(Camera.main)
            Camera.main.transform.position = new Vector3((tilesWidth * tileSizeX) / 2, cameraHeight, (tilesHeight * tileSizeY) / 2);
    }

    private void EndLevel()
    {
        if (tiles != null)
        {
            for (int y = 0; y < grid.tiles.y; y++)
            {
                for (int x = 0; x < grid.tiles.x; x++)
                {
                    Destroy(tiles[y][x]);
                }
            }
        }
    }
}
