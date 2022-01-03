using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMoveable : TempoTrigger
{
    private bool isMoving = false;
    public bool IsMoving { get { return isMoving; } }

    private Vector2 tile = Vector3.zero;
    public Vector2 Tile { get { return tile; } }

    public static readonly float TILE_SIZE = 1f;
    public static readonly float MOVE_SPEED = 0.15f;

    public static readonly Vector2 UP = new Vector2(0f, 1f);        // 0
    public static readonly Vector2 DOWN = new Vector2(0f, -1f);     // 1
    public static readonly Vector2 RIGHT = new Vector2(1f, 0f);     // 2
    public static readonly Vector2 LEFT = new Vector2(-1f, 0f);     // 3

    public enum DIRECTION { UP = 0, DOWN = 1, RIGHT = 2, LEFT = 3 };

    protected IEnumerator Move(DIRECTION Direction)
    {
        if (!IsMoving)
        {
            Vector2 direction = Direction == DIRECTION.UP ? UP : Direction == DIRECTION.DOWN ? DOWN : Direction == DIRECTION.RIGHT ? RIGHT : Direction == DIRECTION.LEFT ? LEFT : Vector2.zero;

            isMoving = true;

            float elapsedTime = 0f;
            Vector2 targetTile = tile + direction;
            while(elapsedTime < MOVE_SPEED)
            {
                Vector2 interm = Vector2.Lerp(tile * TILE_SIZE, targetTile * TILE_SIZE, (elapsedTime / MOVE_SPEED));
                transform.position = new Vector3(interm.x, transform.position.y, interm.y);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = new Vector3(targetTile.x * TILE_SIZE, transform.position.y, targetTile.y * TILE_SIZE);
            tile = targetTile;

            isMoving = false;
        }
    }

    public override void Beat() { }


}
