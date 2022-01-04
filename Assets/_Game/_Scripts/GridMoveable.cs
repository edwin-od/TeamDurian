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

    protected IEnumerator Move(DIRECTION Direction)
    {
        if (!IsMoving && GridManager.Get)
        {
            Vector2 direction = Direction == DIRECTION.UP ? UP : Direction == DIRECTION.DOWN ? DOWN : Direction == DIRECTION.RIGHT ? RIGHT : Direction == DIRECTION.LEFT ? LEFT : Vector2.zero;
            Vector2 targetTile = new Vector2(tile.x, tile.y) + direction;

            if (targetTile.x < 0 || targetTile.y < 0 || targetTile.x >= GridManager.Get.Grid.tiles.x || targetTile.y >= GridManager.Get.Grid.tiles.y)
                yield break;

            isMoving = true;
            float elapsedTime = 0f;
            while (elapsedTime < MOVE_SPEED)
            {
                Vector2 interm = Vector2.Lerp(Vector2.Scale(new Vector2(tile.x, tile.y), GridManager.Get.Grid.tileSize), Vector2.Scale(targetTile, GridManager.Get.Grid.tileSize), (elapsedTime / MOVE_SPEED));
                transform.position = new Vector3(interm.x, transform.position.y, interm.y);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = new Vector3(targetTile.x * GridManager.Get.Grid.tileSize.x, transform.position.y, targetTile.y * GridManager.Get.Grid.tileSize.y);
            tile = new GridManager.IntVector2(Mathf.FloorToInt(targetTile.x), Mathf.FloorToInt(targetTile.y));

            isMoving = false;
        }
    }

    public void Teleport(GridManager.IntVector2 newTile, Vector3 Offset)
    {
        if (GridManager.Get)
        {
            transform.position = Offset + new Vector3(newTile.x * GridManager.Get.Grid.tileSize.x, 0, newTile.y * GridManager.Get.Grid.tileSize.y);
            tile = newTile;
        }
    }

    public override void Beat() { }


}
