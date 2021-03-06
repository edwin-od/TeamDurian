using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMoveable : TempoTrigger
{
    //[SerializeField] private AnimationCurve movement = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField, Range(1f, 20f)] private float beatSpeed = 5f;

    private bool isMoving = false;
    public bool IsMoving { get { return isMoving; } }

    private GridManager.IntVector2 tile = new GridManager.IntVector2(0, 0);
    public GridManager.IntVector2 Tile { get { return tile; } }

    private Vector2 targetTile;
    protected Vector2 loopTargetTile;
    private Vector2 loopTargetTilePrevious;

    public static readonly Vector2 UP = new Vector2(0f, 1f);        // 0
    public static readonly Vector2 DOWN = new Vector2(0f, -1f);     // 1
    public static readonly Vector2 RIGHT = new Vector2(1f, 0f);     // 2
    public static readonly Vector2 LEFT = new Vector2(-1f, 0f);     // 3

    public enum DIRECTION { UP = 0, DOWN = 1, RIGHT = 2, LEFT = 3 };

    private float elapsedTime = 0f;
    protected float beatLength = 0f;


    public void Move(DIRECTION Direction, System.Action callback = null)
    {
        if(isMoving) { TeleportOnGrid(new GridManager.IntVector2((int)loopTargetTile.x, (int)loopTargetTile.y)); elapsedTime = 0f; }

        Vector2 direction = Direction == DIRECTION.UP ? UP : Direction == DIRECTION.DOWN ? DOWN : Direction == DIRECTION.RIGHT ? RIGHT : Direction == DIRECTION.LEFT ? LEFT : Vector2.zero;
        targetTile = new Vector2(tile.x, tile.y) + direction;

        loopTargetTile = targetTile;
        loopTargetTilePrevious = targetTile;
        if (targetTile.x < 0)
        {
            loopTargetTile.x = GridManager.Instance.Grid.tiles.x - 1;
            loopTargetTilePrevious.x = GridManager.Instance.Grid.tiles.x;
        }
        else if (targetTile.x >= GridManager.Instance.Grid.tiles.x)
        {
            loopTargetTile.x = 0;
            loopTargetTilePrevious.x = -1;
        }

        if (targetTile.y < 0)
        {
            loopTargetTile.y = GridManager.Instance.Grid.tiles.y - 1;
            loopTargetTilePrevious.y = GridManager.Instance.Grid.tiles.y;
        }
        else if (targetTile.y >= GridManager.Instance.Grid.tiles.y)
        {
            loopTargetTile.y = 0;
            loopTargetTilePrevious.y = -1;
        }

        if (!isMoving) 
        {
            if (JuiceManager.Instance.IsMovementOn)
            {

            isMoving = true; StartCoroutine(MoveTransition(callback));
            }
            else
            {
                isMoving = false; TeleportOnGrid(new GridManager.IntVector2((int)loopTargetTile.x, (int)loopTargetTile.y));
            }
        }
    }

    private IEnumerator MoveTransition(System.Action callback = null)
    {
        if (GridManager.Instance && Tempo.Instance)
        {
            float tx = Time.realtimeSinceStartup;
            float tpause = 0;
            elapsedTime = 0f;

            beatLength = Tempo.Instance.TempoPeriod * 0.98f / beatSpeed;

            while (elapsedTime < beatLength)
            {
                if (!Tempo.Instance.IsTempoPaused)
                {
                    float deltaTime = 0;

                    // Manage Pause Compensation
                    if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; }
                    else { deltaTime = Time.realtimeSinceStartup - tx; }

                    tx = Time.realtimeSinceStartup;
                    Vector2 interm = Vector2.zero;

                    float realT = elapsedTime / beatLength;
                    //float t = movement.Evaluate(realT);
                    float t = EasingFuncs.EaseInOut(realT);

                    if (targetTile == loopTargetTile)
                        interm = Vector2.Lerp(Vector2.Scale(new Vector2(tile.x, tile.y), GridManager.Instance.Grid.tileSize), Vector2.Scale(targetTile, GridManager.Instance.Grid.tileSize), t);
                    else
                    {
                        if(t < 0.5)
                            interm = Vector2.Lerp(Vector2.Scale(new Vector2(tile.x, tile.y), GridManager.Instance.Grid.tileSize), Vector2.Scale(targetTile, GridManager.Instance.Grid.tileSize), t / 0.5f);
                        else
                            interm = Vector2.Lerp(Vector2.Scale(loopTargetTilePrevious, GridManager.Instance.Grid.tileSize), Vector2.Scale(loopTargetTile, GridManager.Instance.Grid.tileSize), (t - 0.5f) / 0.5f);
                    }

                    transform.position = new Vector3(interm.x, transform.position.y, interm.y);

                    elapsedTime += deltaTime;
                }
                else if(Tempo.Instance.IsTempoPaused && tpause == 0)
                    tpause = Time.realtimeSinceStartup;

                yield return null;
            }

            TeleportOnGrid(new GridManager.IntVector2((int)loopTargetTile.x, (int)loopTargetTile.y));

            callback?.Invoke();

            elapsedTime = 0f;
            isMoving = false;
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
