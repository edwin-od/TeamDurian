using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMoveable : TempoTrigger
{
    private bool isMoving = false;
    public bool IsMoving { get { return isMoving; } }

    private GridManager.IntVector2 tile = new GridManager.IntVector2(0, 0);
    public GridManager.IntVector2 Tile { get { return tile; } }

    public static readonly float MOVE_SPEED = 0.075f;

    public static readonly Vector2 UP = new Vector2(0f, 1f);        // 0
    public static readonly Vector2 DOWN = new Vector2(0f, -1f);     // 1
    public static readonly Vector2 RIGHT = new Vector2(1f, 0f);     // 2
    public static readonly Vector2 LEFT = new Vector2(-1f, 0f);     // 3

    public enum DIRECTION { UP = 0, DOWN = 1, RIGHT = 2, LEFT = 3 };

    public void Move(DIRECTION Direction)
    {
        if (!isMoving)
            StartCoroutine(MoveTransition(Direction));
    }

    private IEnumerator MoveTransition(DIRECTION Direction)
    {
        if (GridManager.Instance)
        {

            Vector2 direction = Direction == DIRECTION.UP ? UP : Direction == DIRECTION.DOWN ? DOWN : Direction == DIRECTION.RIGHT ? RIGHT : Direction == DIRECTION.LEFT ? LEFT : Vector2.zero;
            Vector2 targetTile = new Vector2(tile.x, tile.y) + direction;

            if (targetTile.x < 0 || targetTile.y < 0 || targetTile.x >= GridManager.Instance.Grid.tiles.x || targetTile.y >= GridManager.Instance.Grid.tiles.y)
                yield break;

            isMoving = true;
            float tx = Time.realtimeSinceStartup;
            float tpause = 0;
            float elapsedTime = 0f;
            while (elapsedTime < MOVE_SPEED)
            {
                if (Tempo.Instance && !Tempo.Instance.IsTempoPaused)
                {
                    float deltaTime = 0;

                    // Manage Pause Compensation
                    if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; }
                    else { deltaTime = Time.realtimeSinceStartup - tx; }

                    tx = Time.realtimeSinceStartup;

                    Vector2 interm = Vector2.Lerp(Vector2.Scale(new Vector2(tile.x, tile.y), GridManager.Instance.Grid.tileSize), Vector2.Scale(targetTile, GridManager.Instance.Grid.tileSize), (elapsedTime / MOVE_SPEED));
                    transform.position = new Vector3(interm.x, transform.position.y, interm.y);

                    elapsedTime += deltaTime;
                }
                else if(Tempo.Instance && Tempo.Instance.IsTempoPaused && tpause == 0)
                    tpause = Time.realtimeSinceStartup;

                yield return null;
            }

            Teleport(new GridManager.IntVector2((int)targetTile.x, (int)targetTile.y), Vector3.zero);
            isMoving = false;
        }
    }

    public void Teleport(GridManager.IntVector2 newTile, Vector3 Offset)
    {
        if (GridManager.Instance && newTile.x >= 0 && newTile.y >= 0 && newTile.x < GridManager.Instance.Grid.tiles.x && newTile.y < GridManager.Instance.Grid.tiles.y)
        {
            transform.position = Offset + new Vector3(newTile.x * GridManager.Instance.Grid.tileSize.x, 0, newTile.y * GridManager.Instance.Grid.tileSize.y);
            tile = newTile;
        }
    }

    public void TeleportOnGrid(GridManager.IntVector2 newTile)
    {
        if (GridManager.Instance && newTile.x >= 0 && newTile.y >= 0 && newTile.x < GridManager.Instance.Grid.tiles.x && newTile.y < GridManager.Instance.Grid.tiles.y)
        {
            transform.position = GridManager.Instance.transform.position + new Vector3(newTile.x * GridManager.Instance.Grid.tileSize.x, 0, newTile.y * GridManager.Instance.Grid.tileSize.y);
            tile = newTile;
        }
    }

    public override void Beat() { }


}
